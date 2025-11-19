using Crewly.Data;
using Crewly.Manager;
using Crewly.Buttons;


using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using Telegram.Bot;

namespace Crewly.MessageHandlingProcesses;

public static class ViewTasks
{
    private static readonly IDatabase RedisDb = RedisManager.GetDatabase();
    private static TelegramBotClient? _bot;
    private static long _userId;

    public static async Task SendTasks(long userId)
    {
        await using var db = new BotDbContext();
        _bot = BotHolder.Bot!;
        _userId = userId;

        string key = $"{userId}_view_tasks";

        var readJson = await RedisDb.StringGetAsync(key);

        if (readJson.HasValue)
        {
            var tasksList = JsonSerializer.Deserialize<List<TaskData>>(readJson!);  
            await SendTaskToTelegram(tasksList!);
            return;
        }

        var tasks = await db.Tasks.Where(x => x.OwnerId == userId).ToListAsync();

        if (tasks.Count == 0)
        {
            await _bot.SendMessage(userId, "У вас сейчас нет активных задач,\nвы можете создать их в главном меню 😊");
            return;
        }
        
        var json = JsonSerializer.Serialize(tasks);

        Console.WriteLine("From Tables DB");
        await RedisDb.StringSetAsync(key, json);
        
        await SendTaskToTelegram(tasks);
    }

    private static async Task SendTaskToTelegram(List<TaskData> tasks)
    {
        foreach (var task in tasks)
        {
            var caption = BuildTasksCaption(task);
            string taskId = (task.TaskId).ToString();
            await _bot!.SendMessage(_userId, caption, replyMarkup:BotButtons.CreateTasksEditKeyboard(taskId));
        }
    }

    private static string BuildTasksCaption(TaskData task)
    {
        return $"{task.Title}\n" +
               $"{task.Specification}\n" +
               $"{task.Tags}\n" +
               $"{task.Budget}\n" +
               $"{task.Deadline}\n" +
               $"{task.Attachments}\n";
    }

    public static async Task CancelOperation(long userId)
    {
        string key = $"{userId}_view_tasks";
        await RedisDb.KeyDeleteAsync(key);
    }
}

public static class DeleteThisTask
{
    public static async Task DeleteTask(long userId, string data)
    {
        var bot = BotHolder.Bot!;
        
        Guid taskId = Guid.Parse(data.Split('.', StringSplitOptions.RemoveEmptyEntries)[1]);
        Console.WriteLine($"{taskId.GetType()}: {taskId}");
        
        await using var db = new BotDbContext();
        var deletingTask = await db.Tasks.FindAsync(taskId);

        if (deletingTask != null)
        {
            db.Tasks.Remove(deletingTask);
            await bot.SendMessage(userId, $"{deletingTask.Title} был удален!");
        }
        
        await ViewTasks.CancelOperation(userId);
        await db.SaveChangesAsync();
    }
}

