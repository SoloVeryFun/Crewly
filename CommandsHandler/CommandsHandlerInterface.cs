using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using Crewly.Data;
using Crewly.MessageHandlingProcesses;
using Crewly.Buttons;
using Crewly.Session;

namespace Crewly.CommandsHandler;

public interface ICommandHandler
{
    bool CanExecuteCommand(UserState state);
    
    Task HandleAsync(long userId, Message message, TelegramBotClient bot); 
}

public class OwnSurveyMessage : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsMenuAccess(state);

    public async Task HandleAsync(long userId, Message message, TelegramBotClient bot)
    {
        UserData session;
        
        switch (message.Text)
        {
            case "Моя анкета":
                await new SendUserProfileProcessHandler().SendUserProfileProcess(userId, bot);
                break;
            
            case "Создание заказа":
                session = await SessionManager.GetSession(userId);
                if (session.Role == UserRole.Client)
                {
                    //CREAT
                }
                break;
            
            case "Настройки":
                await bot.SendMessage(userId, "Вы перешли на меню настроек", replyMarkup: BotButtons.SettingMenu());
                break;
            
            case "Удалить аккаунт":
                await DeleteProcess.Delete(userId);
                await bot.SendMessage(chatId: userId, text: "Ваш аккаунт удален😔",replyMarkup: new ReplyKeyboardRemove());
                
                session = await SessionManager.GetSession(userId);
                Console.WriteLine(session.Role);
                Console.WriteLine(session.State);
                Console.WriteLine(session.UserId);
                
                break;
            
            default:
                await bot.SendMessage(userId, "Нет такой команды)");
                break;
        }
    }
}

public class RegisterMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsRegistration(state);
    
    public async Task HandleAsync(long userId, Message message, TelegramBotClient bot)
    {
        await new ResponseRegistrationProcessHandler(bot).ResponseRegistrationProcess(userId, message);
    }
}

public class WaitForVerificationMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsWaitForVerification(state);

    public async Task HandleAsync(long userId, Message message, TelegramBotClient bot)
    {
        var handler = new WaitForVerificationProcessHandler(bot, userId);
        await handler.InitAsync();
        await handler.WaitForVerificationProcess();
    }
}

public class StartMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsStart(state);

    public async Task HandleAsync(long userId, Message message, TelegramBotClient bot)
    {
        if (message.Text == "/start")
        {
            await bot.SendMessage(userId,
                    "Привет! Это Crawly — бот для быстрого поиска проверенных, креативных специалистов.",
                    replyMarkup: new ReplyKeyboardRemove());
            await bot.SendMessage(userId,
                "👷 Исполнитель — если ты предлагаешь свои услуги.\n 💼 Клиент — если ты ищешь специалистов.",
                replyMarkup: BotButtons.CreateRoleSelectionKeyboard());
        }
    }
}

