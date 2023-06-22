using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BotAit;
using System.Net;
using Telegram.Bot.Types.InputFiles;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


#region bot
var botClient = new TelegramBotClient(System.Configuration.ConfigurationSettings.AppSettings["token"]);

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Start listening for @{me.Username}");


#endregion

Console.ReadLine();
cts.Cancel();


async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{

    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await handleMessage(botClient, update.Message, cancellationToken);
        return;
    }

    if (update.Type == UpdateType.CallbackQuery)
    {
        await handleCallBackQuery(botClient, update.CallbackQuery, cancellationToken);
        return;
    }
}

async Task handleMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
{
    try
    {
        await SQL.AddUser('@' + message.Chat.Id);

        if (message.Text == "/start")
        {
            if (SQL.userCount % 100 == 0 && SQL.hundredthUser)
            {
                SQL.hundredthUser = false;
                await botClient.SendTextMessageAsync(message.Chat.Id, $"🎉поздравляю вы {SQL.userCount} пользователь АИТ бота🎉");
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, "<b>👋🏻 Вас приветствует телеграмм бот Анапского Индустриального Техникума 2.0🥳 🏫</b>" + Environment.NewLine + Environment.NewLine +
                "- Добавлены баги* 😁" + Environment.NewLine + "- Устраненны фичи* 🫣" + Environment.NewLine + Environment.NewLine +
                "Eсли у вас возникли проблемы при использовании бота" + Environment.NewLine +
                "Пожалуйста, напишите нам на почту" + Environment.NewLine +
                "Ait.vamr.technology@gmail.com", ParseMode.Html);

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                     {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("✍🏻 Расписание", "para"),
                        InlineKeyboardButton.WithCallbackData("📰 Новости ", "somenews"),
                    },
                });

            await botClient.SendTextMessageAsync(message.Chat.Id, "🕵️‍♂️ Что показать ?", replyMarkup: inlineKeyboard);

        }
        if (message.Text == "/myschedule")
        {
            if (SQL.userCount % 100 == 0 && SQL.hundredthUser)
            {
                SQL.hundredthUser = false;
                await botClient.SendTextMessageAsync(message.Chat.Id, $"🎉поздравляю вы {SQL.userCount} пользователь бота🎉");
            }
            string grupp = await SQL.GetUserLang('@' + message.Chat.Id);

            string url = "https://aitanapa.ru/расписание-занятий/";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string html = reader.ReadToEnd();

            string pattern = @"<span\sclass=""linkText"">Расписание\sс\s(\d{2}\.\d{2}\.\d{4})\sпо\s(\d{2}\.\d{2}\.\d{4})<\/span>";
            MatchCollection matches = Regex.Matches(html, pattern);
            string date;
            DateTime dt;
            Match match = Regex.Match(html, pattern);
            date = match.Groups[1].Value;
            dt = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            //if (matches.Count > 0)
            //{
            //    // Получаем последнее совпадение шаблона
            //    Match lastMatch = matches[matches.Count - 1];
            //    date = lastMatch.Groups[1].Value;
            //    dt = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            //}
            //else
            //{
                
            //}





            // Использование объекта dt для дальнейших операций с датой

            response.Close();

            if (grupp != " ")
            {
                InlineKeyboardMarkup weekDay = new InlineKeyboardMarkup(new[]
                {
            new []
                {
                InlineKeyboardButton.WithCallbackData("Понедельник  " + dt.ToString("d"), "Понедельник_Week"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Вторник  " + dt.AddDays(1).ToString("d"), "Вторник_Week"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Среда  " + dt.AddDays(2).ToString("d"), "Среда_Week"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Четверг  " + dt.AddDays(3).ToString("d"), "Четверг_Week"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Пятница  " + dt.AddDays(4).ToString("d"), "Пятница_Week"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Суббота  " + dt.AddDays(5).ToString("d"), "Суббота_Week"),
                }
            });


                await SQL.ChangeUserLang('@' + message.Chat.Id, grupp);
                await botClient.SendTextMessageAsync(message.Chat.Id, text: "<b>🧑‍🏫 Группа:  </b>" + grupp + Environment.NewLine + $"📅 Выберите день недели 📅", ParseMode.Html, replyMarkup: weekDay);

            }
            else
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Расписание", "para"),
                    },
                });

                await botClient.SendTextMessageAsync(message.Chat.Id, "👨‍🏫 Для начала выберите свою группу", replyMarkup: inlineKeyboard);

            }

        }
        if (message.Text == "/news")
        {
            if (SQL.userCount % 100 == 0 && SQL.hundredthUser)
            {
                SQL.hundredthUser = false;
                await botClient.SendTextMessageAsync(message.Chat.Id, $"🎉поздравляю вы {SQL.userCount} пользователь бота🎉");
            }

            string newsArr = await NewsFromSite.GetNews();
            Console.WriteLine("_________NEWS_________");

            string[] firstSplit = newsArr.Split(new char[] { '#' });
            string[] firstElements = new string[firstSplit.Length];

            for (int i = 0; i < firstSplit.Length; i++)
            {
                string[] splitElement = firstSplit[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                firstElements[i] = splitElement.Length > 0 ? splitElement[2] : "";
            }

            InlineKeyboardMarkup newsButton = new InlineKeyboardMarkup(new[]
                 {
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[0], "picknews_0"),
                },
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[1], "picknews_1"),
                },
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[2], "picknews_2"),
                },
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[3], "picknews_3"),
                },
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[4], "picknews_4"),
                },
                new []
                {
                InlineKeyboardButton.WithCallbackData(firstElements[5], "picknews_5"),
                }
            });

            await botClient.SendTextMessageAsync(message.Chat.Id, $"📰  Последние новости АИТa  📰", replyMarkup: newsButton);

        }
        if (message.Text == "/game")
        {
            var gameKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallBackGame("Играть")
                }
            });

            await botClient.SendGameAsync(
                chatId: message.Chat.Id,
                gameShortName: "TestGame",
                replyMarkup: gameKeyboard
            );

        }
    }

    catch (Exception ex)
    {
        Console.WriteLine("_____________Возникло исключение!______________");
        Console.WriteLine(ex.Message);
    }
}


async Task handleCallBackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
{
    try
    {

        if (callbackQuery != null)
        {
            await SQL.AddUser('@' + callbackQuery.Message.Chat.Id);

            string gameName = "TestGame";

            if (callbackQuery.GameShortName == gameName)
            {
                Console.WriteLine(callbackQuery.Data);
                //?userid=" + callbackQuery.Message.Chat.Id
                var gameUrl = "https://weitix.github.io/webAppTG/";
                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, url: gameUrl);
            }
          

            else
            {
                //if (SQL.userCount % 100 == 0 && SQL.hundredthUser)
                //{
                //    SQL.hundredthUser = false;
                //    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"🎉поздравляю вы {SQL.userCount} пользователь АИТ бота🎉");
                //}

                if (callbackQuery.Data.Contains("para"))
                {
                    await specificGrupp(botClient, callbackQuery);
                }

                if (callbackQuery.Data.Contains("gruppa"))
                {
                    await selectGrupp(botClient, callbackQuery);

                }

                if (callbackQuery.Data.Contains("-"))
                {
                    await selectDayOfWeek(botClient, callbackQuery);

                }

                if (callbackQuery.Data.Contains("Week"))
                {
                    string grupp = await SQL.GetUserLang('@' + callbackQuery.Message.Chat.Id);
                    string Week = callbackQuery.Data.Split("_")[0];
                    string substr = Week.Substring(0, 3) + "_";

                    Message waitMessage = await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "🔎 Ищем ваше расписание на " + Week + " 🤔");
                    Thread.Sleep(1500);
                    await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, messageId: waitMessage.MessageId, cancellationToken: cancellationToken);

                    try
                    {
                        List<List<string>> paraInfo = await getExcel(callbackQuery, grupp, substr);
                        List<List<string>> msgLine = new List<List<string>>();
                        string[] lessonTime = { "8.00-09.20", "09.30-10.50", "11.10-12.20", "12.40-14.00", "14.10-15.30", "15.40-17.00" };

                        if (paraInfo != null)
                        {
                            //убрать остутствующие пары при выводе, логика в сообщения #INWORK 
                            Console.WriteLine("______________paraInfo_______________________");

                            for (int i = 0; i < paraInfo.Count; i++)
                            {
                                msgLine.Add(new List<string>());

                                if (paraInfo[i][0].ToString() != " ")
                                {

                                    msgLine[i].Add($"<b>⏰   {lessonTime[i]}   ⏰</b>{Environment.NewLine}"); //0
                                    msgLine[i].Add("📒 " + paraInfo[i][0].ToString() + Environment.NewLine); //1
                                    msgLine[i].Add($"<b>🏫 Кабинет: </b>"); //2
                                    msgLine[i].Add(paraInfo[i][1].ToString() + Environment.NewLine + Environment.NewLine); //3 
                                }
                                else
                                {
                                    msgLine[i].Add("");
                                    msgLine[i].Add("");
                                    msgLine[i].Add("");
                                    msgLine[i].Add("");
                                }
                            }

                            bool allEmpty = msgLine.All(list => list.All(string.IsNullOrEmpty));


                            if (allEmpty)
                            {
                                await botClient.SendTextMessageAsync(
                                           callbackQuery.Message.Chat.Id,
                                           text:
                                           "<b>🧑‍🏫 Группа:  </b>" + grupp + Environment.NewLine +
                                           "<b>📅 День недели:  </b>" + Week + Environment.NewLine +
                                           "<b>😜 У вас нет пар 😜</b>", ParseMode.Html
                                           );
                            }

                            else
                            {
                                await botClient.SendTextMessageAsync(
                                           callbackQuery.Message.Chat.Id,
                                           text:
                                           "<b>🧑‍🏫 Группа:  </b>" + grupp + Environment.NewLine +
                                           "<b>📅 День недели:  </b>" + Week + Environment.NewLine + Environment.NewLine +
                                           msgLine[0][0] + msgLine[0][1] + msgLine[0][2] + msgLine[0][3] +
                                           msgLine[1][0] + msgLine[1][1] + msgLine[1][2] + msgLine[1][3] +
                                           msgLine[2][0] + msgLine[2][1] + msgLine[2][2] + msgLine[2][3] +
                                           msgLine[3][0] + msgLine[3][1] + msgLine[3][2] + msgLine[3][3] +
                                           msgLine[4][0] + msgLine[4][1] + msgLine[4][2] + msgLine[4][3] +
                                           msgLine[5][0] + msgLine[5][1] + msgLine[5][2] + msgLine[5][3]
                                           , ParseMode.Html
                                           );
                            }
                        }






                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("_____________Возникло исключение!______________");
                        Console.WriteLine(ex.Message);
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"😔 при попытке вывести ваше расписание возникла неизвестная ошибка ⛔");
                    }


                }

                if (callbackQuery.Data.Contains("somenews"))
                {
                    await pickNews(botClient, callbackQuery);
                }

                if (callbackQuery.Data.Contains("picknews"))
                {
                    await pickCorrentNews(botClient, callbackQuery, cancellationToken);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("_____________Возникло исключение!______________");
        Console.WriteLine(ex.Message);
    }
}

async Task<List<List<string>>> getExcel(CallbackQuery callbackQuery, string grupp, string substr)
{
    await DownloadExcel.Download();

    Console.WriteLine(substr);

    string paraInfo = await ExcelParse.readExcel(grupp);
    Console.WriteLine(paraInfo);

    string input = paraInfo;

    if (input.Contains("Нет такой группы"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "Нет такой группы");

        return null;
    }
    else
    {
        char delimiter = '|';
        string[] lines = input.Split('#');
        List<string> sos = new List<string>() { " ", " ", " " };
        List<List<string>> elementsList = new List<List<string>>();

        foreach (string line in lines)
        {
            List<string> elements = new List<string>();
            string[] parts = line.Split(delimiter);

            foreach (string part in parts)
            {
                if (part.Contains(substr))
                {
                    int index = part.IndexOf(substr);
                    string modifiedPart = part.Remove(index, substr.Length);
                    elements.Add(modifiedPart);
                }
            }

            if (!line.Contains(substr))
            {
                elementsList.Add(sos);
            }

            else
            {
                elementsList.Add(elements);
            }
        }
        return elementsList;
    }

}

#region { mainFunc }

static async Task specificGrupp(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    #region klava
    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
            new []
                {
                InlineKeyboardButton.WithCallbackData("Акушерское дело", "АКД_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Информационные системы и программирование", "ИСП_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Монтаж, наладка и эксплуатация электрооборудования", "МНЭЭ_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Правоохранительная деятельность", "ПД_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Прикладная информатика", "ПИ_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Право и организация социального обеспечения", "ПСО_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Реклама", "РЕК_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Сестринское дело", "СД_gruppa"),
                },
            new []
                {
                InlineKeyboardButton.WithCallbackData("Стоматология ортопедическая", "СО_gruppa"),
                },
             new []
                {
                InlineKeyboardButton.WithCallbackData("Техническое обслуживание и ремонт двигателей", "ТО_gruppa"),
                },
             new []
                {
                InlineKeyboardButton.WithCallbackData("Фармация", "ФАРМ_gruppa"),
                },
              new []
                {
                InlineKeyboardButton.WithCallbackData("Экономика и бухгалтерский учет", "ЭКБ_gruppa"),
                }
            });
    #endregion
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"🧑‍🏫 Выберите свою специальность", replyMarkup: inlineKeyboard);
}

static async Task selectGrupp(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    string gruppName = callbackQuery.Data.Split("_")[0];

    Console.WriteLine(gruppName);

    string json = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"output.json"));
    dynamic jsonObj = JsonConvert.DeserializeObject(json);
    List<string> gruppList = new List<string>();


    foreach (string item in jsonObj["grupp"])
    {
        if (item.Contains(gruppName) && gruppName.Length == item.Split("-")[0].Length)
        {
            gruppList.Add(item);
        }
    }

    int maxButtonsPerRow = 3; // максимальное количество кнопок в одной строке
    int rowCount = (int)Math.Ceiling((double)gruppList.Count / maxButtonsPerRow); // количество строк кнопок

    List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

    int counter = 0;
    int buttonIndex = 0;

    for (int row = 0; row < rowCount; row++)
    {
        int buttonsLeft = gruppList.Count - buttonIndex; // сколько кнопок осталось
        int buttonsInRow = Math.Min(maxButtonsPerRow, buttonsLeft); // сколько кнопок в текущей строке

        buttons.Add(new InlineKeyboardButton[buttonsInRow]);

        for (int i = 0; i < buttonsInRow; i++)
        {
            buttons[row][i] = InlineKeyboardButton.WithCallbackData(gruppList[buttonIndex], gruppList[buttonIndex]);
            buttonIndex++;
        }
    }

    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"🧑‍🏫 Выберите свою группу", replyMarkup: inlineKeyboard);
}

static async Task selectDayOfWeek(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    string Grupp = callbackQuery.Data;

    string url = "https://aitanapa.ru/расписание-занятий/";
    WebRequest request = WebRequest.Create(url);
    WebResponse response = request.GetResponse();

    Stream stream = response.GetResponseStream();
    StreamReader reader = new StreamReader(stream);
    string html = reader.ReadToEnd();

    string pattern = @"<span\sclass=""linkText"">Расписание\sс\s(\d{2}\.\d{2}\.\d{4})\sпо\s(\d{2}\.\d{2}\.\d{4})<\/span>";
    MatchCollection matches = Regex.Matches(html, pattern);
    string date;
    DateTime dt;
    Match match = Regex.Match(html, pattern);
    date = match.Groups[1].Value;
    dt = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

    //if (matches.Count > 0)
    //{
    //    // Получаем последнее совпадение шаблона
    //    Match lastMatch = matches[matches.Count - 1];
    //    date = lastMatch.Groups[1].Value;
    //    dt = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    //}
    //else
    //{
        
    //}

    response.Close();


    InlineKeyboardMarkup weekDay = new InlineKeyboardMarkup(new[]
    {
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Понедельник  " + dt.ToString("d"), "Понедельник_Week"),
                                },
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Вторник  " + dt.AddDays(1).ToString("d"), "Вторник_Week"),
                                },
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Среда  " + dt.AddDays(2).ToString("d"), "Среда_Week"),
                                },
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Четверг  " + dt.AddDays(3).ToString("d"), "Четверг_Week"),
                                },
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Пятница  " + dt.AddDays(4).ToString("d"), "Пятница_Week"),
                                },
                            new []
                                {
                                InlineKeyboardButton.WithCallbackData("Суббота  " + dt.AddDays(5).ToString("d"), "Суббота_Week"),
                                }
                            });

    await SQL.ChangeUserLang('@' + callbackQuery.Message.Chat.Id, Grupp);
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"📆 Выберите день недели 📆", replyMarkup: weekDay);
}

static async Task pickNews(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    string newsArr = await NewsFromSite.GetNews();
    Console.WriteLine("_________NEWS_________");

    string[] firstSplit = newsArr.Split(new char[] { '#' });
    string[] firstElements = new string[firstSplit.Length];

    for (int i = 0; i < firstSplit.Length; i++)
    {
        string[] splitElement = firstSplit[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        firstElements[i] = splitElement.Length > 0 ? splitElement[2] : "";
    }

    InlineKeyboardMarkup newsButton = new InlineKeyboardMarkup(new[]
         {
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[0], "picknews_0"),
                                },
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[1], "picknews_1"),
                                },
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[2], "picknews_2"),
                                },
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[3], "picknews_3"),
                                },
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[4], "picknews_4"),
                                },
                                new []
                                {
                                InlineKeyboardButton.WithCallbackData(firstElements[5], "picknews_5"),
                                }
                            });

    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"<b>📰  Последние новости АИТa  ✍🏻</b>", ParseMode.Html, replyMarkup: newsButton);
}

static async Task pickCorrentNews(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
{
    int newsCallBack = Int32.Parse(callbackQuery.Data.Split("_")[1]);

    Message waitMessage = await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "📰 Формируем новостное сообщение 🧑🏻‍💻");
    Thread.Sleep(1500);
    await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, messageId: waitMessage.MessageId, cancellationToken: cancellationToken);

    string newsArr = await NewsFromSite.GetNews();

    string[] firstSplit = newsArr.Split(new char[] { '#' });
    string[] firstElements = new string[firstSplit.Length];

    string[] firstSplitLink = newsArr.Split(new char[] { '#' });
    string[] firstElementsLink = new string[firstSplit.Length];

    string[] firstSplitPhoto = newsArr.Split(new char[] { '#' });
    string[] firstElementsPhoto = new string[firstSplit.Length];

    for (int i = 0; i < firstSplit.Length; i++)
    {
        string[] splitElement = firstSplit[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        firstElements[i] = splitElement.Length > 0 ? splitElement[3] : "";

        string[] splitElementLink = firstSplitLink[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        firstElementsLink[i] = splitElementLink.Length > 0 ? splitElementLink[0] : "";

        string[] splitElementPhoto = firstSplitPhoto[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        firstElementsPhoto[i] = splitElementPhoto.Length > 0 ? splitElementPhoto[1] : "";
    }

    InlineKeyboardMarkup newsButton = new InlineKeyboardMarkup(new[]
        {
                                new []
                                {
                                InlineKeyboardButton.WithUrl("📰 Перейти к новости на сайте 🌐", firstElementsLink[newsCallBack]),
                                }
                            });


    var imageUrl = firstElementsPhoto[newsCallBack];
    var imageData = new WebClient().DownloadData(imageUrl);

    using (var imageStream = new System.IO.MemoryStream(imageData))
    {
        // отправляем фото
        var photo = new InputOnlineFile(imageStream, "photo.jpg");
        await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, new InputMedia(imageStream, "image.jpg"), firstElements[newsCallBack], replyMarkup: newsButton);
    }
}

#endregion

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

