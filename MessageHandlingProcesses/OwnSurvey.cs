using System.Text.Json;
using Newtonsoft.Json;

using Telegram.Bot;
using Telegram.Bot.Types;

using Crewly.Data;
using Crewly.Manager;

using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Crewly.MessageHandlingProcesses;

public class SendUserProfileProcessHandler
{
    public async Task SendUserProfileProcess(long userId, TelegramBotClient bot)
    {
        await using var db = new BotDbContext();
        
        var executor = await db.Executors.FirstOrDefaultAsync(x => x.UserId == userId);
        if (executor != null)
        {
            string caption = BuildExecutorCaption(executor);
            await SendUser(bot, userId, executor.Avatar!, caption);
            return;
        }
        
        var client = await db.Clients.FirstOrDefaultAsync(x => x.UserId == userId);
        if (client != null)
        {
            string caption = BuildClientCaption(client);
            await SendUser(bot, userId, client.Avatar!, caption);
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

    private async Task SendUser(TelegramBotClient bot, long chatId, string avatarPath, string caption)
    {
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