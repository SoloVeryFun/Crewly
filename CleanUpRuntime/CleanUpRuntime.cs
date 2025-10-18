namespace Crewly.CleanUpRuntime;

public static class CleanUpRuntime
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
    private static bool _cleanupRunning = false;
    
    public static void StartCleanupTask(Func<Task<IEnumerable<long>>> getExpiredKeysAsync, Func<long, Task> removeActionAsync)
    {
        if (_cleanupRunning) return;
        _cleanupRunning = true;

        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(CleanupInterval);

                Console.WriteLine("Cleaning up...");
                var expiredKeys = (await getExpiredKeysAsync()).ToList();
                foreach (var key in expiredKeys)
                    await removeActionAsync(key);
            }
        });
    }
}