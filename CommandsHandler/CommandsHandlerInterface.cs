using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using Crewly.Data;
using Crewly.MessageHandlingProcesses;
using Crewly.Buttons;
using Crewly.Manager;
using Microsoft.EntityFrameworkCore;

namespace Crewly.CommandsHandler;

public interface ICommandHandler
{
    bool CanExecuteCommand(UserState state);
    
    Task HandleAsync(long userId, Message message); 
}

public class OwnSurveyMessage : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsMenuAccess(state);

    public async Task HandleAsync(long userId, Message message)
    {
        var bot = BotHolder.Bot!;
        
        UserData session;
        
        switch (message.Text)
        {
            case "Мой профиль":
                session = await SessionManager.GetSession(userId);

                ReplyKeyboardMarkup reply;
                if (session.Role == UserRole.Client)
                {
                    session.State = UserState.ClientProfileMenu;
                    reply = BotButtons.ClientProfileMenu();
                }
                else
                {
                    session.State = UserState.ExecutorProfileMenu;
                    reply = BotButtons.ExecutorProfileMenu();
                }
                
                await SessionManager.SetSession(session);
                await bot.SendMessage(userId, "Вы перешли в меню профиля", replyMarkup: reply);

                break;
            
            case "Создание заказа":
                const int maxCount = 5;
                
                session = await SessionManager.GetSession(userId);

                var db = new BotDbContext();
                var tasksCount = await db.Tasks.CountAsync(x => x.OwnerId == userId);

                if (tasksCount >= maxCount)
                {
                    await bot.SendMessage(userId, "у вас пополнение лимит заказов😥");
                    return;
                }
                
                if (session.Role == UserRole.Client)
                {
                    await new ResponseCreatingTaskProcessesHandler().CreatingTaskProcess(userId, message);
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
                await CancelOperation.CancelOrReturnToMenu("Вы вернулись в главное меню)", userId);
                break;
            
            default:
                await bot.SendMessage(userId, "Нет такой команды)");
                break;
        }
    }
}

public class UserProfileMenu : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsUserProfileMenu(state);

    public async Task HandleAsync(long userId, Message message)
    {
        var session = await SessionManager.GetSession(userId);

        if (session.Role == UserRole.Client)
        {
            switch (message.Text)
            {
                case "Моя анкета":
                    await new SendUserProfileProcessHandler().SendUserProfileProcess(userId);
                    break;
                
                case "Посмотреть мои заказы":
                    await ViewTasks.SendTasks(userId);
                    break;
                
                case "Назад":
                    await ViewTasks.CancelOperation(userId);
                    await CancelOperation.CancelOrReturnToMenu("Вы вернулись в главное меню",  userId);
                    break;
            }
        }
    }
}

public class TaskCreatingMessage : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsTaskCreate(state);

    public async Task HandleAsync(long userId, Message message)
    {
        switch (message.Text)
        {
            case "Отмена":
                await TaskSession.Remove(userId);
                await CancelOperation.CancelOrReturnToMenu("Создание нового заказа отменена", userId);
                break;
            
            default:
                await new ResponseCreatingTaskProcessesHandler().CreatingTaskProcess(userId, message);
                break;
        }
    }
}

public class TaskEditingMessage : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsTaskEditing(state);

    public async Task HandleAsync(long userId, Message message)
    {
        await ResponseEditingTaskProcessesHandler.EditingTaskProcess(userId, message.Text!);
    }
}

public class RegisterMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsRegistration(state);
    
    public async Task HandleAsync(long userId, Message message)
    {
        await new ResponseRegistrationProcessHandler().ResponseRegistrationProcess(userId, message);
    }
}

public class WaitForVerificationMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsWaitForVerification(state);

    public async Task HandleAsync(long userId, Message message)
    {
        var handler = new WaitForVerificationProcessHandler(userId);
        await handler.InitAsync();
        await handler.WaitForVerificationProcess();
    }
}

public class StartMessages : ICommandHandler
{
    public bool CanExecuteCommand(UserState state) => UserStateGroup.IsStart(state);

    public async Task HandleAsync(long userId, Message message)
    {
        var bot = BotHolder.Bot!;
        
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
    public static async Task CancelOrReturnToMenu(string message, long userId)
    {
        var bot = BotHolder.Bot!;
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
    }
}