namespace Crewly.CleanUpRuntime;

public static class CleanUpRuntime
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(1);
    private static bool _cleanupRunning;
    
    public static void StartCleanupTask(
        Func<Task<IEnumerable<long>>> getExpiredKeysAsync,
        Func<long, Task> removeActionAsync,
        CancellationToken cancellationToken = default)
    {
        if (_cleanupRunning) return;
        _cleanupRunning = true;

        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);

                    var expiredKeys = (await getExpiredKeysAsync()).ToList();
                    
                    var tasks = expiredKeys.Select(removeActionAsync).ToList();
                    await Task.WhenAll(tasks);
                    
                    if (expiredKeys.Count > 0)
                        Console.WriteLine($"Cleaned {expiredKeys.Count} expired sessions");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cleanup error: {ex}");
                }
            }
        }, cancellationToken);
    }
}

