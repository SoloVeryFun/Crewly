using Telegram.Bot.Types.ReplyMarkups;

namespace Crewly.Buttons;


public static class BotButtons
{
    public static InlineKeyboardMarkup CreateTasksEditKeyboard(string taskId)
    {
        return KeyboardAndButtons.CreateKeyboard(("Изменить", $"edit.{taskId}"), ("Удалить", $"delete.{taskId}"));
    }
    
    public static InlineKeyboardMarkup CreateRoleSelectionKeyboard()
    {
        return KeyboardAndButtons.CreateKeyboard(("Я клиент", "client"), ("Я исполнитель", "executor"));
    }

    //Client buttons
    public static ReplyKeyboardMarkup ClientUsageMenu()
    {
        return KeyboardAndButtons.CreateClientUsageMenu();
    }

    public static ReplyKeyboardMarkup ClientProfileMenu()
    {
        return KeyboardAndButtons.CreateClientProfileMenu();
    }

    public static ReplyKeyboardMarkup TaskEditing()
    {
        return KeyboardAndButtons.CreateTaskEditingKeyboard();
    }

    //Executors buttons
    public static ReplyKeyboardMarkup ExecutorUsageMenu()
    {
        return KeyboardAndButtons.CreateExecutorClientUsageMenu();
    }
    
    public static ReplyKeyboardMarkup ExecutorProfileMenu()
    {
        return KeyboardAndButtons.ExecutorClientProfileMenu();
    }

    public static ReplyKeyboardMarkup SettingMenu()
    {
        return KeyboardAndButtons.SettingButtons();
    }

    public static ReplyKeyboardMarkup CancelMenu()
    {
        return KeyboardAndButtons.CancelButtons();
    }
}

internal static class KeyboardAndButtons
{
    public static InlineKeyboardMarkup CreateKeyboard(params (string text, string callback)[] buttons)
    {
        return new InlineKeyboardMarkup(
            buttons.Select(b => InlineKeyboardButton.WithCallbackData(b.text, b.callback))
        );
    }
    
    //Client buttons
    public static ReplyKeyboardMarkup CreateClientUsageMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Мой профиль"), new KeyboardButton("Искать фрилансеров") ],
            [ new KeyboardButton("Создание заказа")],
            [ new KeyboardButton("Настройки") ]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }

    public static ReplyKeyboardMarkup CreateClientProfileMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Моя анкета")],
            [ new KeyboardButton("Посмотреть мои заказы")],
            [ new KeyboardButton("Назад") ]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }

    public static ReplyKeyboardMarkup CreateTaskEditingKeyboard()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Заголовок"), new KeyboardButton("Краткое ТЗ")],
            [ new KeyboardButton("Теги"), new KeyboardButton("Бюджет")],
            [ new KeyboardButton("Дедлайн"), new KeyboardButton("Вложение")]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }
    
    //Executor buttons
    public static ReplyKeyboardMarkup CreateExecutorClientUsageMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Моя анкета"), new KeyboardButton("Искать заказов") ],
            [ new KeyboardButton("Настройки") ]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }
    
    public static ReplyKeyboardMarkup ExecutorClientProfileMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Мой профиль")],
            [ new KeyboardButton("Назад") ]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }

    public static ReplyKeyboardMarkup SettingButtons()
    {   
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Удалить аккаунт" )],
            [ new KeyboardButton("Назад" )] 
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }

    public static ReplyKeyboardMarkup CancelButtons()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [new KeyboardButton("Отмена")]
        ])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };

        return replyKeyboard;
    }
}