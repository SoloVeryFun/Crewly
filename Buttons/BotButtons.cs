using Telegram.Bot.Types.ReplyMarkups;

namespace Crewly.Buttons;


public static class BotButtons
{
    public static InlineKeyboardMarkup CreateRoleSelectionKeyboard()
    {
        return KeyboardAndButtons.CreateKeyboard(("Я клиент", "client"), ("Я исполнитель", "executor"));
    }

    public static ReplyKeyboardMarkup ClientUsageMenu()
    {
        return KeyboardAndButtons.CreateClientUsageMenu();
    }

    public static ReplyKeyboardMarkup ExecutorUsageMenu()
    {
        return KeyboardAndButtons.CreateExecutorClientUsageMenu();
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

public static class KeyboardAndButtons
{
    public static InlineKeyboardMarkup CreateKeyboard(params (string text, string callback)[] buttons)
    {
        return new InlineKeyboardMarkup(
            buttons.Select(b => InlineKeyboardButton.WithCallbackData(b.text, b.callback))
        );
    }
    
    public static ReplyKeyboardMarkup CreateClientUsageMenu()
    {
        var replyKeyboard = new ReplyKeyboardMarkup(
        [
            [ new KeyboardButton("Моя анкета"), new KeyboardButton("Искать фрилансеров") ],
            [ new KeyboardButton("Создание заказа")],
            [ new KeyboardButton("Настройки") ]
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }
    
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