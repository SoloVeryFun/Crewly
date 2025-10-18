using Crewly.Data;

namespace Crewly.MessageHandlingProcesses;

public static class DeleteProcess
{
    public static async Task Delete(long userId)
    {
        await SqlDataBaseDelete.DeleteAsync(userId);
        var session = new UserData(){UserId = userId};
        await SessionManager.SetSession(session);
    }
}