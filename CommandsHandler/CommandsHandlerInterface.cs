using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using Crewly.Data;
using Crewly.MessageHandlingProcesses;
using Crewly.Buttons;
using Crewly.Manager;

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
                    await new ResponseCreatingTaskProcessesHandler(bot).CreatingTaskProcess(userId, message);
                }
                break;
            
            case "Настройки":
                await bot.SendMessage(userId, "Вы перешли на меню настроек", replyMarkup: BotButtons.SettingMenu());
                break;
            
            case "Удалить аккаунт":
                await DeleteProcess.Delete(userId);
                await bot.SendMessage(chatId: userId, text: "Ваш аккаунт удален😔",replyMarkup: new ReplyKeyboardRemove());
                break;
            
            case "Назад":
                await CancelOperation.CancelOrReturnToMenu("Вы вернулись в главное меню)", userId, bot);
                break;
            
            default:
                await bot.SendMessage(userId, "Нет такой команды)");
                break;
        }
    }
}

public class TaskCreatingMessage : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsTaskCreate(state);

    public async Task HandleAsync(long userId, Message message, TelegramBotClient bot)
    {
        switch (message.Text)
        {
            case "Отмена":
                await CancelOperation.CancelOrReturnToMenu("Создание нового заказа отменена", userId, bot);
                break;
            
            default:
                await new ResponseCreatingTaskProcessesHandler(bot).CreatingTaskProcess(userId, message);
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

//Cancel
public static class CancelOperation
{
    public static async Task CancelOrReturnToMenu(string message, long userId, TelegramBotClient bot)
    {
        var session = await SessionManager.GetSession(userId);
        session.State = UserState.Menu;

        ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup();

        switch (session.Role)
        {
            case  UserRole.Client:
                keyboard = BotButtons.ClientUsageMenu();
                break;
            case UserRole.Executor:
                keyboard = BotButtons.ExecutorUsageMenu();
                break;
        }
        
        await bot.SendMessage(userId, message, replyMarkup: keyboard);
        await SessionManager.SetSession(session);
        await TaskSession.Remove(userId);
    }
}