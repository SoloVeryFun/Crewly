using System.Collections.Concurrent;
using Crewly.Data;

namespace Crewly.Manager;

public static class TaskSession
{
    private static readonly ConcurrentDictionary<long, TaskData> ActivateTasks = new();

    public static TaskData GetTaskSession(long userId)
    {
        return ActivateTasks.GetOrAdd(userId, _ => new TaskData(){ OwnerId = userId });
    }

    public static async Task SetTaskSession(long userId, TaskData task)
    {
        ActivateTasks[userId] = task;
    }

    public static async Task Remove(long userId)
    {
        ActivateTasks.TryRemove(userId, out _);
    }
}