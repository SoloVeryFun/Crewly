using System.Collections.Concurrent;
using Crewly.Data;
using Microsoft.EntityFrameworkCore;

namespace Crewly;

public static class SessionManager
{
    private static readonly ConcurrentDictionary<long, (UserData Session, DateTime Expire)> Sessions = new();
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);
    
    public static async Task<UserData> GetSession(long userId)
    {
        if (Sessions.TryGetValue(userId, out var entry))
        {
            if (entry.Expire > DateTime.UtcNow)
            {
                await SetSession(entry.Session);
            }
            
            Console.WriteLine("--------------------");
            Console.WriteLine(entry.Expire);
            Console.WriteLine(entry.Session.State);
            Console.WriteLine("--------------------");

            return entry.Session;
        }

        await using var db = new BotDbContext();
        
        var executor = await db.Executors.FirstOrDefaultAsync(x => x.UserId == userId);
        if (executor != null)
        {
            Console.WriteLine("Export from Executors DB");
            Sessions[userId] = (executor, DateTime.UtcNow.Add(Ttl));
            return executor;
        }

        var client = await db.Clients.FirstOrDefaultAsync(x => x.UserId == userId);
        if (client != null)
        {
            Console.WriteLine("Export from Clients DB");
            Sessions[userId] = (client, DateTime.UtcNow.Add(Ttl));
            return client;
        }
        
        var session = new UserData { UserId = userId };
        Sessions[userId] = (session, DateTime.UtcNow.Add(Ttl));
        return session;
    }
    
    public static async Task SetSession(UserData userData)
    {
        Sessions[userData.UserId] = (userData, DateTime.UtcNow.Add(Ttl));
    }
    
    public static async Task<IEnumerable<long>> GetExpiredKeys()
    {
        var now = DateTime.UtcNow;
        return Sessions.Where(kvp => kvp.Value.Expire <= now).Select(kvp => kvp.Key);
    }

    public static async Task Remove(long userId)
    {
        await SqlDataBaseSave.SaveAsync(Sessions[userId].Session);
        Sessions.TryRemove(userId, out _);
    }
}