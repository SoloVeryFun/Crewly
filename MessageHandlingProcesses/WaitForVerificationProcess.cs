using Crewly.Data;
using Telegram.Bot;

namespace Crewly.MessageHandlingProcesses;

public class WaitForVerificationProcessHandler(TelegramBotClient bot, long userId)
{
    private UserRole _role;
    
    readonly string _text =
        "Это Crawly — бот для быстрого поиска проверенных, креативных специалистов.) Ждите пожалуйста пока наши модераторы проверять вас)))";

    public async Task InitAsync()
    {
        var session = await SessionManager.GetSession(userId);
        _role = session.Role;
    }
    
    public async Task WaitForVerificationProcess()
    {
        switch (_role)
        {
            case UserRole.Client:
            await bot.SendMessage(userId, _text,
                replyMarkup: BotButtons.CreateClientUsageMenu());
            break;
            
            case UserRole.Executor:
            await bot.SendMessage(userId, _text,
                replyMarkup: BotButtons.ExecutorClientUsageMenu());
            break;
        }
    }
}