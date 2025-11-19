using Telegram.Bot;

namespace Crewly;

public static class BotHolder
{
    public static TelegramBotClient? Bot { get; private set; }

    public static void Init(string token, CancellationToken tokenCts)
    {
        Bot ??= new TelegramBotClient(token, cancellationToken: tokenCts);
    }
}