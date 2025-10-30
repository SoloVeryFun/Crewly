using Telegram.Bot;
using Telegram.Bot.Types;

using Crewly.CommandsHandler;
using Crewly.Manager;

namespace Crewly;

public class CommandHandler
{
    private readonly IEnumerable<ICommandHandler> _handlers =  new List<ICommandHandler>()
    {
        new TaskCreatingMessage(),
        new OwnSurveyMessage(),
        new RegisterMessages(),
        new WaitForVerificationMessages(),
        new StartMessages()
    };
    
    public async Task HandleMessage(Message message, TelegramBotClient bot)
    {
        var userId = message.Chat.Id;
        var session = await SessionManager.GetSession(userId);

        await _handlers.FirstOrDefault(h => h.CanExecuteCommand(session.State))
            ?.HandleAsync(userId, message, bot)!;
    }
}