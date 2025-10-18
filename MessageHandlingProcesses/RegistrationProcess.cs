using Crewly.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Crewly.MessageHandlingProcesses;

public static class RegQuestions
{
    public static readonly Dictionary<UserState, string> Questions = new()
    {
        { UserState.ExecutorName, "Как вас зовут?" },
        { UserState.ExecutorBio, "Коротко расскажите о себе (до 400 символов)." },
        { UserState.ExecutorSpecializations, "Укажите ваши специализации (до 5)." },
        { UserState.ExecutorBid, "Какая у вас ставка (почасовая/фикс)?" },
        { UserState.ExecutorExperience, "Сколько у вас опыта (лет/уровень)?" },
        { UserState.ExecutorPortfolio, "Пришлите ссылки на портфолио (Behance, Dribbble, Drive)." },
        { UserState.ExecutorAvailability, "Ваша занятость: свободен, частично занят, не берете заказы?" },
        { UserState.ExecutorAvatar, "Загрузите ваше фото/аватар." },
        { UserState.ExecutorContacts, "Оставьте контакты (телеграм, почта, телефон) — по желанию." },
        
        { UserState.ClientName, "Как вас зовут или как называется ваша компания/студия?" },
        { UserState.ClientBio, "Расскажите о себе или о вашем проекте (до 600 символов)." },
        { UserState.ClientType, "Вы частное лицо, студия или компания?" },
        { UserState.ClientBudgetary, "Какой у вас бюджет?" },
        { UserState.ClientBrandGuide, "Есть ли у вас ссылки или бренд‑гайд (опционально)?" },
        { UserState.ClientLanguage, "На каком языке(ах) будет проект?" },
        { UserState.ClientAvatar, "Прикрепите ваш аватар или логотип."},
        { UserState.ClientLocation, "Где вы или ваша компания находитесь?" },
    };

    public static UserState GetNext(UserState current)
    {
        return current switch
        {
            UserState.ExecutorRegistrationStart => UserState.ExecutorName,
            UserState.ExecutorName => UserState.ExecutorBio,
            UserState.ExecutorBio => UserState.ExecutorSpecializations,
            UserState.ExecutorSpecializations => UserState.ExecutorBid,
            UserState.ExecutorBid => UserState.ExecutorExperience,
            UserState.ExecutorExperience => UserState.ExecutorPortfolio,
            UserState.ExecutorPortfolio => UserState.ExecutorAvailability,
            UserState.ExecutorAvailability => UserState.ExecutorAvatar,
            UserState.ExecutorAvatar => UserState.ExecutorContacts,
            UserState.ExecutorContacts => UserState.ExecutorRegistrationCompleted,
            
            UserState.ClientRegistrationStart => UserState.ClientName,
            UserState.ClientName => UserState.ClientBio,
            UserState.ClientBio => UserState.ClientType,
            UserState.ClientType => UserState.ClientBudgetary,
            UserState.ClientBudgetary => UserState.ClientBrandGuide,
            UserState.ClientBrandGuide => UserState.ClientLanguage,
            UserState.ClientLanguage => UserState.ClientLocation,
            UserState.ClientLocation => UserState.ClientAvatar,
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
                await RegistrationProcess(userId, message,
                    UserState.ExecutorRegistrationStart, UserState.ExecutorRegistrationCompleted);
                break;

            case UserRole.Client:
                await RegistrationProcess(userId, message,
                    UserState.ClientRegistrationStart, UserState.ClientRegistrationCompleted);
                break;

            default:
                await bot.SendMessage(userId, "Некорректный ответ");
                break;
        }
    }

    private async Task RegistrationProcess(long userId, Message message, UserState start,
        UserState end) 
        {
            var session = await SessionManager.GetSession(userId);
            
            if (session.State == start)
            {
                session.State = RegQuestions.GetNext(session.State);
                await bot.SendMessage(userId, RegQuestions.Questions[session.State]);
                return;
            }
            
            if (session.State == UserState.ExecutorAvatar ||
                session.State == UserState.ClientAvatar)
            {
                if(message.Photo == null)
                {
                    await bot.SendMessage(userId, "❗ Пожалуйста, отправьте текст или фото в зависимости от шага.");
                    return;
                }
                
                var photo = message.Photo!.Last();
                var file = await bot.GetFile(photo.FileId);

                Directory.CreateDirectory("Images");

                var fileName = $"{session.Role}_{session.UserId}_{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine("Images", fileName);

                await using var fs = new FileStream(filePath, FileMode.Create);
                await bot.DownloadFile(file.FilePath!, fs);
                
                if (session is ExecutorData executor)
                    executor.Avatar = filePath;
                else if (session is ClientData client)
                    client.Avatar = filePath;
                
                session.State = RegQuestions.GetNext(session.State);
            }   
            
            else if(message.Text != null)
            {
                RegQuestions.SetValue(session, message.Text!);
                session.State = RegQuestions.GetNext(session.State);
            }

            if (session.State == end)
            {
                await using var db = new BotDbContext();
                
                // Save
                switch (end)
                {
                    case UserState.ExecutorRegistrationCompleted:
                        session.State = UserState.Main;
                        if (session is ExecutorData executor)
                        {
                            await bot.SendMessage(userId, "✅ Спасибо! Ваш профиль создан.", replyMarkup: BotButtons.ExecutorClientUsageMenu());
                            await SqlDataBaseSave.SaveAsync(executor);
                        }
                        break;
                    case UserState.ClientRegistrationCompleted:
                        session.State = UserState.Main;
                        if (session is ClientData client)
                        {
                            await bot.SendMessage(userId, "✅ Спасибо! Ваш профиль создан.", replyMarkup: BotButtons.CreateClientUsageMenu());
                            await SqlDataBaseSave.SaveAsync(client);
                        }
                        break;
                }
            }
            else
            {
                await bot.SendMessage(userId, RegQuestions.Questions[session.State]);
            }
        }
    
}