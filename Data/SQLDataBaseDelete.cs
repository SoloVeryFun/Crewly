namespace Crewly.Data;

public static class SqlDataBaseDelete
{
    public static async Task DeleteAsync(long userId)
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
}