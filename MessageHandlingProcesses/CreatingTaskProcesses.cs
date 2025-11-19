using Crewly.Buttons;
using Crewly.CommandsHandler;
using Crewly.Data;
using Crewly.Manager;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Crewly.MessageHandlingProcesses;

public enum TasksInputType
{
    Text,
    MultiText,
}

public class TaskStage
{
    public string Question { get; init; } = "";
    public TasksInputType InputType { get; init; } =  TasksInputType.Text;
    public int? MaxLength { get; init; } = 100;
    public int? MaxItems { get; init; } = 10;
}

public static class TaskQuestions
{
    public static readonly Dictionary<UserState, TaskStage> Questions = new()
    {
        [UserState.TaskTitle] = new()
        {
            Question = "Заголовок",
            MaxLength = 20
        },
        
        [UserState.TaskSpecification] = new()
        {
            Question = "Пишите краткое ТЗ?(до 1000 символов)",
            MaxLength = 1000
        },
        
        [UserState.TaskTags] = new()
        {
            Question = "Отметите теги",
            InputType = TasksInputType.MultiText,
            MaxItems = 5,
        },
        
        [UserState.TaskBudget] = new()
        {
            Question = "Бюджет (диапазон/фикс)?",
        },
        
        [UserState.TaskDeadline] = new()
        {
            Question = "Дедлайн/период?"
        },
        
        [UserState.TaskAttachments] = new()
        {
            Question = "Есть ли у вас вложение?(до 500 символов)",
            MaxLength = 500
        }
    };

    public static bool ValidateInput(UserState userState, Message message, out string? error)
    {
        error = null;

        if (!Questions.TryGetValue(userState, out var stage))
        {
            return true;
        }

        switch (stage.InputType)
        {
            case TasksInputType.Text:
                if (string.IsNullOrWhiteSpace(message.Text))
                {
                    error = "❗ Пожалуйста, введите текст.";
                    return false;
                }

                if (stage.MaxLength.HasValue && message.Text.Length > stage.MaxLength.Value)
                {
                    error = $"❗ Текст слишком длинный. Максимум {stage.MaxLength} символов.";
                    return false;
                }
                break;
            
            case TasksInputType.MultiText:
                var items = message.Text?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (items == null || items.Length == 0)
                {
                    error = "❗ Укажите хотя бы одну специализацию.";
                    return false;
                }

                if (stage.MaxItems.HasValue && items.Length > stage.MaxItems.Value)
                {
                    error = $"❗ Можно указать максимум {stage.MaxItems} специализаций.";
                    return false;
                }
                break;
        }
        
        return true;
    }

    public static UserState GetNext(UserState current)
    {
        return current switch
        {
            UserState.Menu => UserState.TaskTitle,
            UserState.TaskTitle => UserState.TaskSpecification,
            UserState.TaskSpecification => UserState.TaskTags,
            UserState.TaskTags => UserState.TaskBudget,
            UserState.TaskBudget => UserState.TaskDeadline,
            UserState.TaskDeadline => UserState.TaskAttachments,
            _ => UserState.TaskCreationCompleted,
        };
    }

    public static void SetValue(TaskData session, UserState state,string value)
    {
        switch (state)
        {
            case UserState.TaskTitle: session.Title = value; break;
            case UserState.TaskSpecification: session.Specification = value; break;
            case UserState.TaskTags: session.Tags = value; break;
            case UserState.TaskBudget: session.Budget = value; break;
            case UserState.TaskDeadline: session.Deadline = value; break;
            case UserState.TaskAttachments: session.Attachments = value; break;
        }
    }
}

public class ResponseCreatingTaskProcessesHandler
{
    public async Task CreatingTaskProcess(long userId, Message message)
    {
        var bot = BotHolder.Bot!;
        var session = await SessionManager.GetSession(userId);

        if (session.State == UserState.Menu)
        {
            await bot.SendMessage(userId, "Создание заказа!👇", replyMarkup:BotButtons.CancelMenu());
        }
        
        var task = await TaskSession.GetActivateTaskSession<TaskData>(userId);    
        
        if (!TaskQuestions.ValidateInput(session.State, message, out var error))
        {
            await bot.SendMessage(userId, error!);
            return;
        }

        if (message.Text != null)
        { 
            TaskQuestions.SetValue(task, session.State, message.Text);
        }
        
        session.State = TaskQuestions.GetNext(session.State);

        if (session.State == UserState.TaskCreationCompleted)
        {
            await SqlDataBaseSave.TaskSaveAsync(task);

            string text = $"✅ Задача {task.Title} успешно создана!";
            
            await ViewTasks.CancelOperation(userId);
            await TaskSession.Remove(userId);
            await CancelOperation.CancelOrReturnToMenu(text, userId);
            
            return;
        }
        else
        {
            await bot.SendMessage(userId, TaskQuestions.Questions[session.State].Question);
        }
        
        await SessionManager.SetSession(session);
        await TaskSession.SetActivateTaskSession(task);
    }
}

public static class ResponseEditingTaskProcessesHandler
{
    private static class TaskFieldNames
    {
        public static readonly Dictionary<string, string> Map = new()
        {
            ["Заголовок"]  = "Title",
            ["Краткое ТЗ"] = "Specification",
            ["Теги"]       = "Tags",
            ["Бюджет"]     = "Budget",
            ["Дедлайн"]    = "Deadline",
            ["Вложение"]   = "Attachments"
        };
    }
    
    public static async Task EditingTaskProcess(long userId, string data)
    {
        var bot = BotHolder.Bot!;
        var session = await SessionManager.GetSession(userId);
        
        //Preparation
        if (!UserStateGroup.IsTaskEditing(session.State))
        {
            await ViewTasks.CancelOperation(userId);
            
            Guid taskId = Guid.Parse(data.Split('.', StringSplitOptions.RemoveEmptyEntries)[1]);

            var taskEdit = new TaskEditData(){OwnerId = userId, taskId = taskId};
            await TaskSession.SetActivateTaskSession(taskEdit);
        
            session.State = UserState.PreparationEditTask;
            await bot.SendMessage(userId, "В каком часте задачи вам нужно внести поправки?", replyMarkup:BotButtons.TaskEditing());
            await SessionManager.SetSession(session);
            
            return;
        }

        //Editing
        var taskEditData  = await TaskSession.GetActivateTaskSession<TaskEditData>(userId);
        
        if (session.State == UserState.PreparationEditTask)
        {
            TaskFieldNames.Map.TryGetValue(data, out var field);
            
            taskEditData.field = field;
            await TaskSession.SetActivateTaskSession(taskEditData);
            
            session.State = UserState.EditTask;
            await SessionManager.SetSession(session);

            await bot.SendMessage(userId, "Внесите поправки!", replyMarkup: new ReplyKeyboardRemove());
            
            return;
        }
        
        var task = await TaskSession.GetTask(taskEditData.taskId);
        var property = typeof(TaskData).GetProperty(taskEditData.field!);
        
        property!.SetValue(task, data);
        await SqlDataBaseSave.TaskSaveAsync(task);

        await bot.SendMessage(userId, "Изменени были прменены", replyMarkup:BotButtons.ClientProfileMenu());
        
        session.State = UserState.ClientProfileMenu;
        
        await SessionManager.SetSession(session);
        await TaskSession.Remove(userId);
    }
}