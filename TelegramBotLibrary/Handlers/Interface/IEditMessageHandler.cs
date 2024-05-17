using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotLibrary.Answers.Interface;

namespace TelegramBotLibrary.Handlers.Interface
{
    public interface IEditMessageHandler
	{
        Task HandleEditAsync(ITelegramBotClient botClient, Message message, IAnswer answer, CancellationToken cancellationToken);
    }
}