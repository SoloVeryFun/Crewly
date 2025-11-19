using StackExchange.Redis;

namespace Crewly.Manager;

public static class RedisManager
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection =
        new(() => ConnectionMultiplexer.Connect("localhost"));

    private static ConnectionMultiplexer Connection => LazyConnection.Value;

    public static IDatabase GetDatabase(int db = -1) => Connection.GetDatabase(db);
}