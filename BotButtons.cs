using Telegram.Bot.Types.ReplyMarkups;

namespace Crewly;


public static class BotButtons
{
    public static InlineKeyboardMarkup CreateRoleSelectionKeyboard()
    {
        return KeyboardAndButtons.CreateKeyboard(("Я клиент", "client"), ("Я исполнитель", "executor"));
    }

    public static ReplyKeyboardMarkup CreateClientUsageMenu()
    {
        return KeyboardAndButtons.CreateClientUsageMenu();
    }

    public static ReplyKeyboardMarkup ExecutorClientUsageMenu()
    {
        return KeyboardAndButtons.CreateExecutorClientUsageMenu();
    }

    public static ReplyKeyboardMarkup SettingMenu()
    {
        return KeyboardAndButtons.SettingButtons();
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
            [ new KeyboardButton("Моя анкета"), new KeyboardButton("📄 Мои заявки") ],
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
            [ new KeyboardButton("Моя анкета"), new KeyboardButton("📄 Мои заявки") ],
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
            [ new KeyboardButton("Удалить аккаунт" )] 
        ])
        {
            ResizeKeyboard = true,   
            OneTimeKeyboard = false 
        };
        
        return replyKeyboard; 
    }
    
    
}