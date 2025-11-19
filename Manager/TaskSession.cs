using System.Text.Json;
using Crewly.Config;
using Crewly.Data;
using StackExchange.Redis;

namespace Crewly.Manager;

public static class TaskSession
{
    private static readonly IDatabase ActivateTasks = RedisManager.GetDatabase();

    public static async Task<T> GetActivateTaskSession<T>(long userId)
    where T: new()
    {
        string key = $"task_session_{userId}";
        var json =  await ActivateTasks.StringGetAsync(key);

        if (json.HasValue)
        {
            var task = JsonSerializer.Deserialize<T>(json!, SerializationConfig.Options);
            
            return task!;
        }
        
        var newTask = new T();
        if (newTask is TaskData td)
        {
            td.OwnerId = userId;
        }

        return newTask;
    }

    //Set methode
    public static async Task SetActivateTaskSession(TaskData task)
    {
        string key = $"task_session_{task.OwnerId}";
        var json =  JsonSerializer.Serialize(task, SerializationConfig.Options);
        
        await ActivateTasks.StringSetAsync(key, json);
    }
    public static async Task SetActivateTaskSession(TaskEditData task)
    {
        string key = $"task_session_{task.OwnerId}";
        var json =  JsonSerializer.Serialize(task, SerializationConfig.Options);
        
        await ActivateTasks.StringSetAsync(key, json);
    }

    public static async Task Remove(long userId)
    {
        string key = $"task_session_{userId}";
        await ActivateTasks.KeyDeleteAsync(key);
    }
    
    //Task from DB
    public static async Task<TaskData> GetTask(Guid taskId)
    {
        await using var db = new BotDbContext();
        
        var task = await db.Tasks.FindAsync(taskId);

        return task!;
    }
}