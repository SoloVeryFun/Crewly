using Microsoft.EntityFrameworkCore;

namespace Crewly.Data;

public static class SqlDataBaseDelete
{
    public static async Task DeleteUserAsync(long userId)
    {
        await using var db = new BotDbContext();

        var executor = await db.Executors.FindAsync(userId);
        if (executor != null)
        {
            db.Executors.Remove(executor);
        }
        else
        {
            var client = await db.Clients.FindAsync(userId);
            if (client != null)
            {
                db.Clients.Remove(client);
            }
        }
        
        await db.SaveChangesAsync();
    }

    public static async Task DeleteTasksAsync(long userId)
    {
        await using var db = new BotDbContext();
        
        var tasks = await db.Tasks.Where(x => x.OwnerId == userId).ToListAsync();
        db.Tasks.RemoveRange(tasks);
        
        await db.SaveChangesAsync();
    }
}