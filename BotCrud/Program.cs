using Telegram.Bot;

namespace BotCrud
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new BotCrud.TelegramService.TelegramService.TelegramService("6048729939:AAFxLRGj8ZoPXguIF8w6Dine3BPXK1oFEC0").Start();

            //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //AppDbContext db = new();

            //db.Products.Entry(new()
            //{
            //    Name = "Nestle",
            //    CategoryName = Category.MilkyProducts,
            //    ExpireDate = DateTime.Now.AddDays(7),
            //    Img = @"C:\Users\User\Desktop\PDP\Nestle.jpg",
            //    Price = 25.000M
            //}).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            //db.SaveChanges();
           
        }
    }
}