using Telegram.Bot;
using Telegram.Bot.Types;
using Crewly.Data;
using Crewly.RegistrationProcess;

namespace Crewly;

public class CallbackQueryHandler
{
    private readonly TelegramBotClient _bot;
    private readonly Dictionary<string, Func<long, Task>> _handlers;
    private readonly ResponsesForProcessing _responsesForProcessing;
    
    public CallbackQueryHandler(TelegramBotClient bot)
    {
        _bot = bot;
        _handlers = new Dictionary<string, Func<long, Task>>
        {
            {"executor", userId => RegisterUser<ExecutorData>(userId, UserRole.Executor, UserState.ExecutorRegistrationStart)},
            {"client", userId => RegisterUser<ClientData>(userId, UserRole.Client, UserState.ClientRegistrationStart)}
        }; 

        _responsesForProcessing = new ResponsesForProcessing(_bot);
    }

    public async Task HandleCallbackQuery(CallbackQuery query)
    {
        await _bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
        
        if (_handlers.TryGetValue(query.Data!, out var handler))
        {
            await handler(query.Message!.Chat.Id);
        }
        else
        {
            await _bot.SendMessage(query.Message!.Chat, $"Неизвестная команда: {query.Data}");
        }
    }

    private async Task RegisterUser<T>(long userId, UserRole role, UserState userState)
        where T : UserData, new()
    {
        var user = SessionManager.GetSession<UserData>(userId);

        if (user is not T)
        {
            user = new T
            {
                Role = role,
                State = userState
            };
            SessionManager.SetSession(userId, user);
        }
        
        await _responsesForProcessing.ResponseRegistrationProcess( userId, "Registration Start", user.Role);
    }
}