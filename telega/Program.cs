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
            var connectionString = "Host=localhost;Port=5432;Database=pornStore;Username=postgres;Password=зpostgres;";

			//var pathDb = "/" + Path.Combine("home", "keykm","usersdata.db");
            //Console.WriteLine(pathDb);
            //Console.WriteLine(args[0]);
            ITelegramBotClient bot = new TelegramBotClient("7015770120:AAEmoCrQUK0p3JNcTYKZufVd6moFLbCYbzA");

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
InlineKeyboardButton.WithCallbackData("TOP F-CONTENT PRICE 50%", "Channel1Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("EasyCum PRICE 50%", "Channel2Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("F-Team PRICE 50%", "Channel3Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("Right Person PRICE 50%", "Channel4Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("PronHero", "Channel5Button"),
},
new InlineKeyboardButton[]
{
InlineKeyboardButton.WithCallbackData("PronMaster PRICE 50%", "Channel6Button"),
},
})),
"Купить"),
new TextToTextAnswer(
new TextAnswerMessage("Sergay"),
"Gay")
}),
            new SendHandler(),
            new EditMessageHandler(),
            new CryptoPayClient("168281:AALGMXN9Kn6oMQ8967JTQJFZXCX4PiAEVaQ"),
		    connectionString);
            TelegramBot botic = new TelegramBot(bot, _updateHandler);
            botic.Start();
        }
    }
}
