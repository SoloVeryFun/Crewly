using Crewly.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

using Crewly.Buttons;
using Crewly.Manager;

namespace Crewly.MessageHandlingProcesses;

public enum InputType
{
    Text,
    Photo,
    Links,
    MultiText
}

public class RegistrationStage
{
    public string Question { get; init; } = "";
    public InputType InputType { get; init; } =  InputType.Text;
    public int? MaxLength { get; init; } = 100;
    public int? MaxItems { get; init; } = 5;
}

public static class RegQuestions
{
    
    public static readonly Dictionary<UserState, RegistrationStage> Questions = new()
    {
        //Executor Steps
        [UserState.ExecutorName] = new() 
        { 
            Question = "Как вас зовут?", 
        },
        
        [UserState.ExecutorBio] = new() 
        { 
            Question = "Коротко расскажите о себе (до 400 символов).", 
            MaxLength = 400     
        },
        
        [UserState.ExecutorSpecializations] = new() 
        { 
            Question = "Укажите ваши специализации (до 10).(Пишите разделно ',' )", 
            InputType = InputType.MultiText, 
            MaxItems = 10
        },
        
        [UserState.ExecutorBid] = new() 
        {        
            Question = "Какая у вас ставка (почасовая/фикс)?", 
        },
        
        [UserState.ExecutorExperience] = new() 
        { 
            Question = "Сколько у вас опыта (лет/уровень)?", 
        },
        
        [UserState.ExecutorPortfolio] = new() 
        { 
            Question = "Пришлите ссылки на портфолио (Behance, Dribbble, Drive).", 
            InputType = InputType.Links,   
        },
        
        [UserState.ExecutorAvailability] = new() 
        { 
            Question = "Ваша занятость: свободен, частично занят, не берете заказы?",
        },
        
        [UserState.ExecutorAvatar] = new() 
        { 
            Question = "Загрузите ваше фото/аватар.", 
            InputType = InputType.Photo, 
        },
        
        [UserState.ExecutorContacts] = new() 
        { 
            Question = "Оставьте контакты (телеграм, почта, телефон) — по желанию.", 
        },
        
        //Clients Steps
        [UserState.ClientName] = new() 
        { 
            Question = "Как вас зовут или как называется ваша компания/студия?", 
        },
        
        [UserState.ClientBio] = new() 
        { 
            Question = "Расскажите о себе или о вашем проекте (до 600 символов).", 
            MaxLength = 600     
        },
        [UserState.ClientType] = new() 
        { 
            Question = "Вы частное лицо, студия или компания?", 
        },
        [UserState.ClientBudgetary] = new() 
        { 
            Question = "Какой у вас бюджет?", 
        },
        [UserState.ClientBrandGuide] = new() 
        { 
            Question = "Есть ли у вас ссылки или бренд‑гайд?", 
            InputType = InputType.Links, 
        },
        [UserState.ClientLanguage] = new() 
        { 
            Question = "На каком языке(ах) ваш проект?", 
        },
        [UserState.ClientAvatar] = new() 
        { 
            Question = "Прикрепите ваш аватар или логотип.", 
            InputType = InputType.Photo, 
        },
        [UserState.ClientLocation] = new() 
        { 
            Question = "Где вы или ваша компания находитесь?", 
        },
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
            case InputType.Photo:
                if (message.Photo == null)
                {
                    error = "❗ Пожалуйста, отправьте фото.";
                    return false;
                }
                break;
            
            case InputType.Text:
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
            
            case InputType.MultiText:
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
            UserState.ExecutorRegistrationStart => UserState.ExecutorName,
            UserState.ExecutorName => UserState.ExecutorBio,
            UserState.ExecutorBio => UserState.ExecutorSpecializations,
            UserState.ExecutorSpecializations => UserState.ExecutorBid,
            UserState.ExecutorBid => UserState.ExecutorExperience,
            UserState.ExecutorExperience => UserState.ExecutorAvailability,
            UserState.ExecutorAvailability => UserState.ExecutorAvatar,
            UserState.ExecutorAvatar => UserState.ExecutorPortfolio,
            UserState.ExecutorPortfolio => UserState.ExecutorContacts,
            UserState.ExecutorContacts => UserState.ExecutorRegistrationCompleted,
            
            UserState.ClientRegistrationStart => UserState.ClientName,
            UserState.ClientName => UserState.ClientBio,
            UserState.ClientBio => UserState.ClientType,
            UserState.ClientType => UserState.ClientBudgetary,
            UserState.ClientBudgetary => UserState.ClientLanguage,
            UserState.ClientLanguage => UserState.ClientAvatar,
            UserState.ClientAvatar => UserState.ClientBrandGuide,
            UserState.ClientBrandGuide => UserState.ClientLocation,
            _ => UserState.ClientRegistrationCompleted,
        };
    }

    public static void SetValue(UserData session, string value)
    {
        switch (session.State)
        {
            case UserState.ExecutorName: (session as ExecutorData)!.Name = value; break;
            case UserState.ExecutorBio: (session as ExecutorData)!.Bio = value; break;
            case UserState.ExecutorSpecializations: (session as ExecutorData)!.Specializations = value; break;
            case UserState.ExecutorBid: (session as ExecutorData)!.Bid = value; break;
            case UserState.ExecutorExperience: (session as ExecutorData)!.Experience = value; break;
            case UserState.ExecutorPortfolio: (session as ExecutorData)!.Portfolio = value; break;
            case UserState.ExecutorAvailability: (session as ExecutorData)!.Availability = value; break;
            case UserState.ExecutorContacts: (session as ExecutorData)!.Contacts = value; break;
            case UserState.ExecutorAvatar: (session as ExecutorData)!.Avatar = value; break;

            case UserState.ClientName: (session as ClientData)!.Name = value; break;
            case UserState.ClientBio: (session as ClientData)!.Bio = value; break;
            case UserState.ClientType: (session as ClientData)!.Type = value; break;
            case UserState.ClientBudgetary: (session as ClientData)!.Budgetary = value; break;
            case UserState.ClientBrandGuide: (session as ClientData)!.BrandGuide = value; break;
            case UserState.ClientLanguage: (session as ClientData)!.Language = value; break;
            case UserState.ClientLocation: (session as ClientData)!.Location = value; break;
            case UserState.ClientAvatar: (session as ClientData)!.Avatar = value; break;
        }
    }
}

public class ResponseRegistrationProcessHandler(TelegramBotClient bot)
{
    public async Task ResponseRegistrationProcess(long userId, Message message)
    {
        var session = await SessionManager.GetSession(userId);
        UserRole role = session.Role;
        
        switch (role)
        {
            case UserRole.Executor:
                await RegistrationProcess(userId, message, UserState.ExecutorRegistrationCompleted);
                break;

            case UserRole.Client:
                await RegistrationProcess(userId, message, UserState.ClientRegistrationCompleted);
                break;

            default:
                await bot.SendMessage(userId, "Некорректный ответ");
                break;
        }
    }

    private async Task RegistrationProcess(long userId, Message message, UserState end) 
        {
            var session = await SessionManager.GetSession(userId);

            if (!RegQuestions.ValidateInput(session.State, message, out var error))
            {
                await bot.SendMessage(userId, error!);
                return;
            }

            if (message.Photo != null)
            {
                var photo = message.Photo!.Last();
                var file = await bot.GetFile(photo.FileId);

                Directory.CreateDirectory("Images");

                var fileName = $"{session.Role}_{session.UserId}_{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine("Images", fileName);

                await using var fs = new FileStream(filePath, FileMode.Create);
                await bot.DownloadFile(file.FilePath!, fs);
                
                RegQuestions.SetValue(session, filePath);
            }
            
            if (message.Text != null)
            {
                Console.WriteLine(session);
                RegQuestions.SetValue(session, message.Text);
            }
            
            session.State = RegQuestions.GetNext(session.State);

            if (session.State == end)
            {
                await using var db = new BotDbContext();
                
                // Save
                switch (end)
                {
                    case UserState.ExecutorRegistrationCompleted:
                        session.State = UserState.Menu;
                        if (session is ExecutorData executor)
                        {
                            await bot.SendMessage(userId, "✅ Спасибо! Ваш профиль создан.", replyMarkup: BotButtons.ExecutorUsageMenu());
                            await SqlDataBaseSave.SaveAsync(executor);
                        }
                        break;
                    case UserState.ClientRegistrationCompleted:
                        session.State = UserState.Menu;
                        if (session is ClientData client)
                        {
                            await bot.SendMessage(userId, "✅ Спасибо! Ваш профиль создан.", replyMarkup: BotButtons.ClientUsageMenu());
                            await SqlDataBaseSave.SaveAsync(client);
                        }
                        break;
                }
            }
            else
            {
                await bot.SendMessage(userId, RegQuestions.Questions[session.State].Question);
            }

            await SessionManager.SetSession(session);
        }
}