using CryptoPay;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotLibrary;
using TelegramBotLibrary.AnswerMessages;
using TelegramBotLibrary.Answers;
using TelegramBotLibrary.Answers.Interface;
using TelegramBotLibrary.Handlers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            ITelegramBotClient bot = new TelegramBotClient(token: args[0]);

            var buyButton = new ReplyKeyboardMarkup(new KeyboardButton("Купить"));
            buyButton.ResizeKeyboard = true;

            var _updateHandler = new UpdateHandler(
            new MessageHandler(
            new List<IAnswer>
            {
new TextToTextAnswer(
new TextAnswerMessage("Нажмите кнопку 'Купить', чтобы приобрести доступ", buyButton),
"/start"),
new TextToTextAnswer(
new TextAnswerMessage("Выберите канал", new InlineKeyboardMarkup(
new IEnumerable<InlineKeyboardButton>[]
{
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 1", "Channel1Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 2", "Channel2Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 3", "Channel3Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 4", "Channel4Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 5", "Channel5Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Канал 6", "Channel6Button"),
},
})),
"Купить"),
new TextToTextAnswer(
new TextAnswerMessage("Sergay"),
"Gay")
            }),
            new SendHandler(),
            new EditMessageHandler(),
            new CryptoPayClient(args[1]),
            "Data Source = usersdata.db");
            TelegramBot botic = new TelegramBot(bot, _updateHandler);
            botic.Start();
        }
    }
}
