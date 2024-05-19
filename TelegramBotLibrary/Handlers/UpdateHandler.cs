using CryptoPay;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotLibrary.AnswerMessages;
using TelegramBotLibrary.AnswerMessages.Interface;
using TelegramBotLibrary.Answers;
using TelegramBotLibrary.Answers.Interface;
using TelegramBotLibrary.Constants;
using TelegramBotLibrary.Handlers.Interface;

namespace TelegramBotLibrary.Handlers
{
    /// <summary>
    /// Хендлер который обрабатывает разние типы <see cref="Update.Type"/>
    /// </summary>
    public class UpdateHandler : IUpdateHandler
    {
        IMessageHandler _messageHandler;
        ISendHandler _sendHandler;
		IEditMessageHandler _editHandler;
        ILogger _logger;

		private CryptoPayClient _cryptoPayClient;
		private string _connectionString;

		public UpdateHandler(IMessageHandler messageHandler, ISendHandler sendHandler, IEditMessageHandler editMessageHandler, ILogger logger = null)
        {
            MessageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            SendHandler = sendHandler;
			EditHandler = editMessageHandler;
            Logger = logger/* ?? throw new ArgumentNullException(nameof(logger))*/;
        }

		public UpdateHandler(IMessageHandler messageHandler, ISendHandler sendHandler, IEditMessageHandler editMessageHandler, CryptoPayClient cryptoPayClient, string connectionString, ILogger logger = null)
			: this(messageHandler, sendHandler, editMessageHandler)
		{
			_cryptoPayClient = cryptoPayClient ?? throw new ArgumentNullException(nameof(cryptoPayClient));
			_connectionString = connectionString;
		}

		public IMessageHandler MessageHandler { get => _messageHandler; set => _messageHandler = value; }
        public ISendHandler SendHandler { get => _sendHandler; set => _sendHandler = value; }
		public IEditMessageHandler EditHandler { get => _editHandler; set => _editHandler = value; }
		public ILogger Logger { get => _logger; set => _logger = value; }
        
        public virtual async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
				await OnUserAction(botClient, update, cancellationToken);
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.Message);
				if (update.Message != null)
				{
					await botClient.SendTextMessageAsync(update.Message.Chat, "/start");
					
				}

				if (update.CallbackQuery != null)
				{
					await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat, "/start");
				}
				//throw;
			}
            
        }

        private async Task OnUserAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
			switch (update.Type)
			{
				case UpdateType.Message:
					await OnMessageSend(botClient, update, cancellationToken);

					break;

				case UpdateType.CallbackQuery:
					await OnButtonClick(botClient, update, cancellationToken);

					break;

				default:
					Logger.LogInformation("Вызван неподерживаемый тип update", update.Type);

					break;
			}
		}

		private async Task OnMessageSend(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			IAnswer answer = await MessageHandler.HandleMessageAsync(update.Message, cancellationToken);
			await _sendHandler.HandleSendAsync(botClient, update.Message, answer, cancellationToken);
		}

		private async Task OnButtonClick(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var callbackQuery = update.CallbackQuery;
			var wayToPayButtons = new TextToTextAnswer(
				new TextAnswerMessage("Выберите способ оплаты",
					new InlineKeyboardMarkup(
						new IEnumerable<InlineKeyboardButton>[]
						{
							new InlineKeyboardButton[]
							{
								InlineKeyboardButton.WithCallbackData("CryptoBot", "CryptoBotButton")
							}
						})), callbackQuery.Data);

			switch (callbackQuery.Data)
			{
				case "Channel1Button":
				case "Channel2Button":
				case "Channel3Button":
				case "Channel4Button":
				case "Channel5Button":
				case "Channel6Button":
					OnChannelClick(update);
					await _editHandler.HandleEditAsync(botClient, callbackQuery.Message, wayToPayButtons, cancellationToken);

					break;

				case "CryptoBotButton":
					using (var connection = new SqliteConnection(_connectionString))
					{
						try
						{
							connection.Open();

							var checkPayButton = await CryptoBotClick(connection, update);

							await _editHandler.HandleEditAsync(botClient, callbackQuery.Message, checkPayButton, cancellationToken);
						}
						catch (Exception ex)
						{ 
							connection.Close();
						}
					}

					break;

				case "PaidButton":
					using (var connection = new SqliteConnection(_connectionString))
					{
						try
						{
							connection.Open();

							var invoice = GetInvoice(connection, update);

							if (invoice.Status == CryptoPay.Types.Statuses.paid)
							{
								var linkButtons = new TextToTextAnswer(
									new TextAnswerMessage("Спасибо за покупку!",
										new InlineKeyboardMarkup(
											new IEnumerable<InlineKeyboardButton>[]
											{
												new InlineKeyboardButton[]
												{
													InlineKeyboardButton.WithUrl("Ссылка на канал", GetChannelUrl(connection, update)),
												}
											})), callbackQuery.Data);

								await _editHandler.HandleEditAsync(botClient, callbackQuery.Message, linkButtons, cancellationToken);
							}
							else
							{
								var payErrorButtons = new TextToTextAnswer(
									new TextAnswerMessage("Оплата еще не прошла, пожалуйста подождите"), callbackQuery.Data);

								await _sendHandler.HandleSendAsync(botClient, callbackQuery.Message, payErrorButtons, cancellationToken);
							}
						}
						catch
						{
							connection.Close();
						}
					}

					break;
			}
		}

		private void OnChannelClick(Update update)
		{
			using (var connection = new SqliteConnection(_connectionString))
			{
				try { 
					connection.Open();

					var query = IsUserExist(update.CallbackQuery.From.Username)
						? SqlQueries.UpdateBuyer
						: SqlQueries.InsertBuyer;
					var command = new SqliteCommand(query, connection);

					command.Parameters.AddWithValue("@Username", update.CallbackQuery.From.Username);
					command.Parameters.AddWithValue("@ChannelName", update.CallbackQuery.Data);
					command.ExecuteNonQuery();
				}
				catch
				{
					connection.Close();
				}
			}
		}

		private bool IsUserExist(string username)
		{
			using (var connection = new SqliteConnection(_connectionString))
			{
				connection.Open();

				var command = new SqliteCommand(SqlQueries.SelectBuyer, connection);
				command.Parameters.AddWithValue("@Username", username);

				return command.ExecuteReader().HasRows;
			}
		}

		private async Task<IAnswer> CryptoBotClick(SqliteConnection connection, Update update)
		{
			var cost = GetChannelCost(connection, update);
			var invoice = await _cryptoPayClient.CreateInvoiceAsync(cost, CryptoPay.Types.CurrencyTypes.crypto, "USDT");
			var checkPayButton = new TextToTextAnswer(
				new TextAnswerMessage($"Счёт на оплату в {cost}$ создан",
					new InlineKeyboardMarkup(
						new IEnumerable<InlineKeyboardButton>[]
						{
							new InlineKeyboardButton[]
							{
								InlineKeyboardButton.WithUrl("Оплатить", invoice.PayUrl)
							},
							new InlineKeyboardButton[]
							{
								InlineKeyboardButton.WithCallbackData("Я оплатил", "PaidButton"),
							}
						})), update.CallbackQuery.Data);

			UpdateInvoicedId(connection, invoice, update.CallbackQuery.From.Username);

			return checkPayButton;
		}

		private double GetChannelCost(SqliteConnection connection, Update update)
		{
			var command = new SqliteCommand(SqlQueries.SelectCostChannelByUsername, connection);
			command.Parameters.AddWithValue("@Username", update.CallbackQuery.From.Username);

			return Convert.ToDouble(command.ExecuteScalar());
		}

		private void UpdateInvoicedId(SqliteConnection connection, CryptoPay.Types.Invoice invoice, string username)
		{
			var command = new SqliteCommand(SqlQueries.UpdateBuyerInvoicedIdByUsername, connection);
			
			command.Parameters.AddWithValue("@Username", username);
			command.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
			command.ExecuteNonQuery();
		}

		private CryptoPay.Types.Invoice GetInvoice(SqliteConnection connection, Update update)
		{
			var command = new SqliteCommand(SqlQueries.SelectInvoiceIdByUsername, connection);
			command.Parameters.AddWithValue("@Username", update.CallbackQuery.From.Username);

			var invoiceId = command.ExecuteScalar() ?? throw new ArgumentNullException();

			return _cryptoPayClient
				.GetInvoicesAsync()
				.Result
				.Items
				.FirstOrDefault(p => p.InvoiceId == (long)invoiceId)
				?? throw new ArgumentNullException();
		}

		private string GetChannelUrl(SqliteConnection connection, Update update)
		{
			var command = new SqliteCommand(SqlQueries.SelectChannelUrl, connection);
			command.Parameters.AddWithValue("@Username", update.CallbackQuery.From.Username);

			return command.ExecuteScalar().ToString() ?? throw new ArgumentNullException();
		}
	}
}
