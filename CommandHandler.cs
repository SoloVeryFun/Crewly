using Crewly.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Crewly.RegistrationProcess;

namespace Crewly;

public class CommandHandler(TelegramBotClient bot)
{
    private readonly BotButtons _botButtons = new BotButtons();
    
    public async Task HandleMessage(Message message, TelegramBotClient bot)
    {
        var userId = message.Chat.Id;
        var session = SessionManager.GetDefault(userId);

        if (message.Text == "/start")
        {
            if(session.State == UserState.Start)
            {
                await bot.SendMessage(message.Chat.Id,
                    "Привет! Это Crawly — бот для быстрого поиска проверенных, креативных специалистов.",
                    replyMarkup: _botButtons.CreateRoleSelectionKeyboard());
                return;
            }
        }

        if (UserStateGroup.IsRegistration(session.State))
        {
            await new ResponsesForProcessing(bot).ResponseRegistrationProcess(userId, message.Text!, session.Role);
        }
    }
}