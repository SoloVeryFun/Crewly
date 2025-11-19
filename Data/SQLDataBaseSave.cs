namespace Crewly.Data;

public static class SqlDataBaseSave
{
    public static async Task SaveAsync(UserData data)
    {
        await using var db = new BotDbContext();

        switch (data)
        {
            case ExecutorData executor:
            {
                var existing = await db.Executors.FindAsync(executor.UserId);
                if (existing == null)
                    await db.Executors.AddAsync(executor);
                else
                    db.Entry(existing).CurrentValues.SetValues(executor);
                break;
            }

            case ClientData client:
            {
                var existing = await db.Clients.FindAsync(client.UserId);
                if (existing == null)
                    await db.Clients.AddAsync(client);
                else
                    db.Entry(existing).CurrentValues.SetValues(client);
                break;
            }
        }

        await db.SaveChangesAsync();
    }

    public static async Task TaskSaveAsync(TaskData data)
    {
        await using var db = new BotDbContext();
        
        var task = await db.Tasks.FindAsync(data.TaskId);

        if (task == null)
        {
            await db.Tasks.AddAsync(data);
        }
        else
        {
            db.Entry(task).CurrentValues.SetValues(data);
        }
        await db.SaveChangesAsync();
    }
}