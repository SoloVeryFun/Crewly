using Crewly.Data;

namespace Crewly;
public enum UserState
{
    Start,
    
    //Executor
    ExecutorRegistrationStart,
    ExecutorName,
    ExecutorBio,
    ExecutorSpecializations,
    ExecutorBid,
    ExecutorExperience,
    ExecutorPortfolio,
    ExecutorAvailability,
    ExecutorAvatar,
    ExecutorContacts,
    ExecutorRegistrationCompleted,
    
    //Client
    ClientRegistrationStart,
    ClientName,
    ClientBio,
    ClientType,
    ClientBudgetary,
    ClientBrandGuide,
    ClientLanguage,
    ClientLocation,
    ClientAvatar,
    ClientRegistrationCompleted,    
    
    //Also
}

public static class SessionManager
{
    private static readonly Dictionary<long, UserData> Sessions = new();

    public static T GetSession<T>(long userId) where T : UserData, new()
    {
        if (!Sessions.ContainsKey(userId))
        {
            Sessions[userId] = new T { UserId = userId };
        }

        return (T)Sessions[userId];
    }

    public static UserData GetDefault(long userId)
    {
        if (!Sessions.ContainsKey(userId))
        {
            Sessions[userId] = new UserData { UserId = userId };
        }

        return Sessions[userId];
    }

    public static void SetSession(long userId, UserData userData) => Sessions[userId] = userData;

    public static void Remove(long userId) => Sessions.Remove(userId);
}