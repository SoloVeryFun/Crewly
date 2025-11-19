using Telegram.Bot;
using Telegram.Bot.Types;

using Crewly.Data;
using Crewly.Manager;

namespace Crewly.MessageHandlingProcesses;

public class SendUserProfileProcessHandler
{
    public async Task SendUserProfileProcess(long userId)
    {
        var session = await SessionManager.GetSession(userId);
        await using var db = new BotDbContext();

        string caption;
        
        switch (session)
        {
            case ClientData client:
                caption = BuildClientCaption(client);
                await SendUser(userId, client.Avatar!, caption);
                break;
            case ExecutorData executor:
                caption = BuildExecutorCaption(executor);
                await SendUser(userId, executor.Avatar!, caption);
                break;
        }
    }

    private string BuildExecutorCaption(ExecutorData executor)
    {
        return $"👤 Роль: Исполнитель\n" +
               $"Имя: {executor.Name}\n" +
               $"Био: {executor.Bio}\n" +
               $"Специализации: {executor.Specializations}\n" +
               $"Ставка: {executor.Bid}\n";
    }
    
    private string BuildClientCaption(ClientData client)
    {
        return $"👤 Роль: Клиент\n" +
               $"Имя: {client.Name}\n" +
               $"Био: {client.Bio}\n" +
               $"Специализации: {client.Location}\n" +
               $"Ставка: {client.Language}\n";
    }

    private async Task SendUser(long chatId, string avatarPath, string caption)
    {
        var bot = BotHolder.Bot!;
        
        if (!string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath))
        {
            await bot.SendPhoto(chatId, new InputFileStream(File.OpenRead(avatarPath)), caption);
        }
        else
        {
            await bot.SendMessage(chatId, caption);
        }
    }
}