using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLibrary.Handlers.Interface
{
    public interface IUpdateHandler
    {
        IMessageHandler MessageHandler { get; set; }

        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}