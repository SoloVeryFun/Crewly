using Telegram.Bot.Types.ReplyMarkups;

namespace Crewly;

public class BotButtons
{
    public InlineKeyboardMarkup CreateRoleSelectionKeyboard()
    {
        return KeyboardAndButtons.CreateKeyboard(("Я клиент", "client"), ("Я исполнитель", "executor"));
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
}