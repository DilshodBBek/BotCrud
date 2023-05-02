using BotCrud;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class TelegramHandler : IUpdateHandler
{
    private ITelegramBotClient _client;
    AppDbContext _db = new AppDbContext();
    List<User> _users = new();
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("HandlePollingErrorAsync:" + exception.Message);
        await botClient.SendTextMessageAsync(591208356, "Date:" + DateTime.Now + " Exception: " + exception.Message);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _client, Update update, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync(update.Message.Text);
        if (update.Message == null) return;
        Parallel.Invoke(() =>
        {
            string cmd = update?.Message?.Text;
            if (!cmd.StartsWith("/")) { cmd = "/" + cmd.ToLower(); }
            var user = _users.FirstOrDefault(x => x.ChatId == update.Message.Chat.Id);

            if (user?.step == -1)
            {
                user.Command = cmd;
                if (cmd.Equals("/add")) user.step = 0;
            }
            else if (user != null) cmd = user.Command;
            switch (cmd)
            {
                case "/start": StartHandler(_client, update); break;
                case "/getall": GetAll(_client, update); break;
                case "/getbyid": GetById(_client, update); break;
                case "/add": Add(_client, update); break;
                case "/remove": Remove(_client, update); break;
                case "/update": UpdateProduct(_client, update); break;

                default: StartHandler(_client, update); break;
            }
        });
    }
    private void StartHandler(ITelegramBotClient _client, Update update)
    {
        KeyboardButton[][] keys = new KeyboardButton[][]
       {
        new []
        {
            new KeyboardButton("Start"),
            new KeyboardButton("GetById"),
            new KeyboardButton("GetAll")
        },
          new KeyboardButton[]
        {
            new KeyboardButton("Add"),
            new KeyboardButton("Remove"),
            new KeyboardButton("Update")
        }
       };
        ReplyKeyboardMarkup markup = new(keys)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true,
        };
        _client.SendTextMessageAsync(update.Message.Chat.Id, "Hi" + update.Message.Chat.FirstName, ParseMode.Markdown, replyMarkup: markup);
    }
    private void UpdateProduct(ITelegramBotClient client, Update? update)
    {
        throw new NotImplementedException();
    }

    private void Remove(ITelegramBotClient client, Update? update)
    {
        throw new NotImplementedException();
    }

    private void Add(ITelegramBotClient client, Update? update)
    {
        long id = update.Message.Chat.Id;
        var user = _users.FirstOrDefault(x => x.ChatId == id);
        if (user != null)
        {
            switch (user.step)
            {
                case 0:
                    {
                        client.SendTextMessageAsync(id, "Name:");
                        user.step++;
                        break;
                    }

                case 1:
                    {
                        if (!string.IsNullOrWhiteSpace(update.Message.Text))
                        {
                            user.Product.Name = update.Message.Text;
                            client.SendTextMessageAsync(id, "Price:");
                            user.step++;

                        }
                        else
                        {
                            user.step--;
                            client.SendTextMessageAsync(id, "Invalid Name! \nPlease reinput value");
                        }
                        break;
                    }
                case 2:
                    {
                        if (decimal.TryParse(update.Message.Text, out decimal price))
                        {
                            user.Product.Price = price;
                            client.SendTextMessageAsync(id, "Photo:");
                            user.step++;
                        }
                        else
                        {
                            client.SendTextMessageAsync(id, "Invalid Price! \nPlease reinput value");
                        }
                        break;
                    }
                case 3:
                    {
                        string path = @"C:\Users\User\Desktop\PDP\Photos\";
                        var file = client.GetFileAsync(update.Message.Photo.Last().FileId);
                        path += DateTime.Now.ToString() + "_" + Path.GetFileName(file.Result.FilePath);
                        using var stream = System.IO.File.OpenWrite(path);
                        var res = client.GetInfoAndDownloadFileAsync(file.Result.FileId, stream);



                        if (System.IO.File.Exists(path))
                        {
                            user.Product.Img = path;
                            client.SendTextMessageAsync(id, "ExpireDate:\nExample" + DateOnly.MinValue);
                            user.step++;
                        }
                        else
                        {
                            client.SendTextMessageAsync(id, "Invalid Photo! \nPlease reinput value");
                        }
                        break;
                    }
                case 4:
                    {
                        if (DateOnly.TryParse(update.Message.Text, out DateOnly date))
                        {
                            user.Product.ExpireDate = Convert.ToDateTime(date);
                            InlineKeyboardButton[][] keys = new InlineKeyboardButton[][]
                                                            {
                                                            new []
                                                            {
                                                                new InlineKeyboardButton("Vegetables"),
                                                                new InlineKeyboardButton("Fruits")
                                                            },
                                                            new[]
                                                            {
                                                                new InlineKeyboardButton("Drinks"),
                                                                new InlineKeyboardButton("MilkyProducts")
                                                            }
                                                            };
                            client.SendTextMessageAsync(id, "CategoryName:", replyMarkup: new InlineKeyboardMarkup(keys));
                            user.step++;
                        }
                        else
                        {
                            client.SendTextMessageAsync(id, "Invalid ExpireDate! \nPlease reinput value");
                        }
                        break;
                    }
            }
        }
    }

    private void GetById(ITelegramBotClient client, Update? update)
    {
        throw new NotImplementedException();
    }

    private void GetAll(ITelegramBotClient client, Update? update)
    {
        // IEnumerable<Product> 
    }


}

record User
{
    public long ChatId { get; set; } = 0;
    public sbyte step { get; set; } = -1;
    public Product Product { get; set; }
    public string Command { get; set; } = "";
}