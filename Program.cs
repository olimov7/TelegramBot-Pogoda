using HtmlAgilityPack;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        private static TelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("7149722787:AAFRmfKgPGgg4-K3g-o3EHccSSYSrap4kgI");

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallbackQuery;
            botClient.StartReceiving();

            Console.ReadKey();

            botClient.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var Id = e.Message.Chat.Id;
            var Text = e.Message.Text;

            if (Text == "/start")
            {
                await botClient.SendTextMessageAsync(Id, "Выберите город для прогноза погоды", replyMarkup: GetInlineMenu());
            }
            else if (Text == "кто ты" || Text == "Кто ты" || Text == "кто тебя создал")
            {
                await botClient.SendTextMessageAsync(Id, "Я телеграм бот, созданный разработчиком по имени Эмомали.");
            }
            else if (Text == "привет" || Text == "Привет")
            {
                await botClient.SendTextMessageAsync(Id, "Привет как поживаешь 😁 ");
            }
            else
            {
                await botClient.SendTextMessageAsync(Id, "Простите, я вас не понимаю 🥶 ");
            }
        }
        

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Message;
            var chatId = message.Chat.Id;

            string url = "";
            switch (e.CallbackQuery.Data)
            {
                case "item1":
                    url = "https://pogoda7.ru/prognoz/gorod207314-Tajikistan-Dushanbe-Dushanbe/3days";
                    break;
                case "item2":
                    url = "https://pogoda7.ru/prognoz/gorod207301-Tajikistan-Viloyati_Khatlon-Kulob/3days";
                    break;
                case "item3":
                    url = "https://pogoda7.ru/prognoz/gorod207330-Tajikistan-Viloyati_Sughd-Khujand/3days";
                    break;
                case "item4":
                    url = "https://pogoda7.ru/prognoz/gorod207310-Tajikistan-Gharm/3days";
                    break;
                case "item5":
                    url = "https://pogoda7.ru/prognoz/gorod207305-Tajikistan-Gorno_Badakhshan-Khorugh/3days";
                    break;

                case "item6":
                    url = "https://pogoda7.ru/prognoz/gorod207276-Tajikistan-Varzob/3days";
                    break;

            }

            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    string weatherInfo = await GetWeatherAsync(url);
                    await botClient.SendTextMessageAsync(chatId, weatherInfo);
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(chatId, "Произошла ошибка , попробуйте еще раз позже.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Город не найден.");
            }

            await botClient.SendTextMessageAsync(chatId, "Выберите город для прогноза погоды", replyMarkup: GetInlineMenu());
        }

        private static async Task<string> GetWeatherAsync(string url)
        {
            string weatherInfo = "";

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var placeNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"subheader\"]/h1"); 

                if (placeNode != null)
                {
                    string place = placeNode.InnerText.Trim();
                    weatherInfo += $"{place}  😶‍🌫️ \n\n";

                    for (int i = 1; i <= 3; i++)
                    {
                        var dataNode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"body\"]/div[8]/div[2]/div[{i}]/div[1]/div[1]");
                        var temperatureNode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"body\"]/div[8]/div[2]/div[{i}]/div[2]/div[2]/div[2]");
                        var prognozNode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"body\"]/div[8]/div[2]/div[{i}]/div[2]/div[2]/div[4]/div");
                        var sunNode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"body\"]/div[8]/div[2]/div[{i}]/div[3]/div/div[2]");

                        if (dataNode != null && temperatureNode != null && prognozNode != null && sunNode != null)
                        {
                            string data = dataNode.InnerText.Trim();
                            string temperature = temperatureNode.InnerText.Trim();
                            string prognoz = prognozNode.InnerText.Trim();
                            string sun = sunNode.InnerText.Trim();

                            weatherInfo += $"{data}  📆 \nТемпература воздуха {temperature}   🌡 \n{prognoz} 🌥 \n            {sun}   \n\n ";
                        }
                    }               
                }
                else
                {
                    weatherInfo = "Информация о погоде не найдена 😔 ";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return weatherInfo;
        }


        private static InlineKeyboardMarkup GetInlineMenu()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Душанбе 🌘 ", "item1"),
                    InlineKeyboardButton.WithCallbackData("Куляб 🌘 ", "item2"),
                    InlineKeyboardButton.WithCallbackData("Худжанд 🌘 ", "item3")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Гарм 🌘 ", "item4"),
                    InlineKeyboardButton.WithCallbackData("Хорог 🌘 ", "item5"),
                    InlineKeyboardButton.WithCallbackData("Варзоб 🌘 ", "item6")


                },
            });

            return inlineKeyboard;
        }
    }
}
