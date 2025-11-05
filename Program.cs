using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Crewly;
using Crewly.Data;
using Crewly.Manager;
using Crewly.CleanUpRuntime;

DataBaseHandler.EnsureMigrated();

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("YOUR_TG_BOT_API", cancellationToken:cts.Token);
var me = await bot.GetMe();

var commandHandler = new CommandHandler();

bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;


Console.WriteLine($"{me.Username} is running...");
Console.ReadLine();
cts.Cancel();
async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}

async Task OnMessage(Message message, UpdateType type)
{
    await commandHandler.HandleMessage(message, bot);
}

async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query })
    {
        var callbackHandler = new CallbackQueryHandler(bot);
        await callbackHandler.HandleCallbackQuery(query);
    }
}
