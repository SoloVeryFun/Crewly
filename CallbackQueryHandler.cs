using Telegram.Bot;
using Telegram.Bot.Types;

using Crewly.Data;
using Crewly.MessageHandlingProcesses;
using Crewly.Manager;

namespace Crewly;

public class CallbackQueryHandler
{
    private readonly TelegramBotClient _bot;
    private readonly Dictionary<string, Func<long, string?, Task>> _handlers;
    private readonly ResponseRegistrationProcessHandler _responsesForProcessing;
    
    public CallbackQueryHandler(TelegramBotClient bot)
    {
        _bot = bot;
        
        _handlers = new Dictionary<string, Func<long, string?, Task>>
        {
            {"executor", async (userId, _) => await RegisterUser<ExecutorData>(userId, UserRole.Executor, UserState.ExecutorRegistrationStart)},
            {"client", async (userId, _) => await RegisterUser<ClientData>(userId, UserRole.Client, UserState.ClientRegistrationStart)},
            
            {"edit", async (userId, data) => await EditTask(userId, data!)},
            {"delete", async (userId, data) => await DeleteTask(userId, data!)}
        }; 

        _responsesForProcessing = new ResponseRegistrationProcessHandler();
    }

    public async Task HandleCallbackQuery(CallbackQuery query)
    {
        await _bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
        
        var data = query.Data ?? string.Empty;
        var chatId = query.Message!.Chat.Id;
        
        var handler = _handlers.FirstOrDefault(x => data.StartsWith(x.Key)).Value;
        
        if (handler != null)
        {
            await handler(chatId, data);
        }
        else
        {
            await _bot.SendMessage(chatId, "Нет такого выбора(");
        }
    }

    private async Task RegisterUser<T>(long userId, UserRole role, UserState userState)
        where T : UserData, new()
    {
        var user = await SessionManager.GetSession(userId);

        if (user is not T)
        {
            user = new T()
            {
                UserId = userId,
                Role = role,
                State = userState
            };
            
            Console.WriteLine(typeof(T));
            
            await SessionManager.SetSession(user);
        }
        
        await _responsesForProcessing.ResponseRegistrationProcess( userId, new Message(){Text = "Registration start"});
    }

    private async Task EditTask(long userId, string taskId)
    {
        await ResponseEditingTaskProcessesHandler.EditingTaskProcess(userId, taskId);
    }
    
    private async Task DeleteTask(long userId, string taskId)
    {
        await DeleteThisTask.DeleteTask(userId, taskId);
    }
}