using Telegram.Bot.Types;
using TelegramBotLibrary.Answers.Interface;
using TelegramBotLibrary.Handlers.Interface;

namespace TelegramBotLibrary.Handlers
{
    /// <summary>
    /// Хендлер который обрабатывает разные типы <see cref="Message.Type"/>
    /// </summary>
    public class MessageHandler : IMessageHandler
    {


		private IList<IAnswer> answers;

		IList<IAnswer> Answers { get => answers; set => answers = value; }

		public MessageHandler(IList<IAnswer> answers)
        {
            Answers = answers ?? throw new ArgumentNullException(nameof(answers));
        }

        public virtual async Task<IAnswer> HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            return answers.First(x => x.Type == message.Type && x.IsNeededThisAnwer(message));
        }

		public virtual async Task<IAnswer> HandleMessageAsync(string data, CancellationToken cancellationToken)
		{
			return answers.First(x => x.IsNeededThisAnwer(data));
		}
	}
}
