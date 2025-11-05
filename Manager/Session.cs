using System.Text.Json;
using Crewly.Config;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Crewly.Data;

namespace Crewly.Manager;

public static class SessionManager
{
    private static readonly IDatabase Db = RedisManager.GetDatabase();
    
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);
    
    public static async Task<UserData> GetSession(long userId)
    {
        var key = $"{userId}";
        var json = await Db.StringGetAsync(key);

        if (json.HasValue)
        {
            var session = JsonSerializer.Deserialize<UserData>(json!, SerializationConfig.Options);
            await Db.KeyExpireAsync(key, Ttl);
            
            return session!;
        }
        
        await using var db = new BotDbContext();
        
        var executor = await db.Executors.FirstOrDefaultAsync(x => x.UserId == userId);
        if (executor != null)
        {
            Console.WriteLine("Export from Executors DB");
            await SetSession(executor);
            return executor;
        }

        var client = await db.Clients.FirstOrDefaultAsync(x => x.UserId == userId);
        if (client != null)
        {
            Console.WriteLine("Client from Clients DB");
            await SetSession(client);
            return client;
        }
        
        var sessionNew = new UserData() {UserId = userId};
        await SetSession(sessionNew);
        return sessionNew;
    }
    
    public static async Task SetSession(UserData userData)
    {
        var key = $"{userData.UserId}";
        var json = JsonSerializer.Serialize(userData, SerializationConfig.Options);
        
        await Db.StringSetAsync(key, json, Ttl);
    }
}