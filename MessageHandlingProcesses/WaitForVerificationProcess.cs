using Telegram.Bot;

using Crewly.Data;
using Crewly.Buttons;
using Crewly.Manager;

namespace Crewly.MessageHandlingProcesses;

public class WaitForVerificationProcessHandler(long userId)
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
        var bot = BotHolder.Bot!;
        
        
        switch (_role)
        {
            case UserRole.Client:
            await bot.SendMessage(userId, _text,
                replyMarkup: BotButtons.ClientUsageMenu());
            break;
            
            case UserRole.Executor:
            await bot.SendMessage(userId, _text,
                replyMarkup: BotButtons.ExecutorUsageMenu());
            break;
        }
    }
}