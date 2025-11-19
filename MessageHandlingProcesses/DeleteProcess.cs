using Crewly.Data;
using Crewly.Manager;

namespace Crewly.MessageHandlingProcesses;

public static class DeleteProcess
{
    public static async Task Delete(long userId)
    {
        await SqlDataBaseDelete.DeleteUserAsync(userId);
        await SqlDataBaseDelete.DeleteTasksAsync(userId);
        var session = new UserData(){UserId = userId};
        await SessionManager.SetSession(session);
    }
}