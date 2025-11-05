using System.Text.Json;
using Crewly.Config;
using Crewly.Data;
using StackExchange.Redis;

namespace Crewly.Manager;

public static class TaskSession
{
    private static readonly IDatabase ActivateTasks = RedisManager.GetDatabase();

    public static async Task<TaskData> GetTaskSession(long userId)
    {
        string key = $"task_session_{userId}";
        var json =  await ActivateTasks.StringGetAsync(key);

        if (json.HasValue)
        {
            var task = JsonSerializer.Deserialize<TaskData>(json!, SerializationConfig.Options);
            
            return task!;
        }
        
        return new TaskData() { OwnerId = userId };
    }

    public static async Task SetTaskSession(TaskData task)
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
}