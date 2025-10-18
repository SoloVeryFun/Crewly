namespace Crewly.Data;

public static class SqlDataBaseSave
{
    public static async Task SaveAsync(UserData data)
    {
        await using var db = new BotDbContext();

        if(data is ExecutorData executor)
        {
            await db.Executors.AddAsync(executor);
        }
        else if (data is ClientData client)
        {
            await db.Clients.AddAsync(client);
        }
        
        await db.SaveChangesAsync();
    }
}