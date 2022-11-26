using Microsoft.EntityFrameworkCore;
using WebAPIMedicine.Models;

namespace WebAPIMedicine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // получаем строку подключения из файла конфигурации
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // добавляем контекст ItemsContext в качестве сервиса в приложение
            builder.Services.AddDbContext<ItemsContext>(options => options.UseSqlServer(connection));
            
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.MapGet("/", (ItemsContext db) => db.Items.ToList());
            app.MapGet("/api/items", async (ItemsContext db) => await db.Items.ToListAsync());

            app.MapGet("/api/items/{id}", async (int id, ItemsContext db) =>
            {
                // получаем элемент по id
                Item? item = await db.Items.FirstOrDefaultAsync(u => u.Id == id);

                // если не найден, отправляем статусный код и сообщение об ошибке
                if (item == null) return Results.NotFound(new { message = "Предмет не найден" });

                // если предмет найден, отправляем его
                return Results.Json(item);
            });

            app.MapDelete("/api/items/{id}", async (int id, ItemsContext db) =>
            {
                // получаем элемент по id
                Item? item = await db.Items.FirstOrDefaultAsync(u => u.Id == id);

                // если не найден, отправляем статусный код и сообщение об ошибке
                if (item == null) return Results.NotFound(new { message = "Элемент не найден" });

                // если пользователь найден, удаляем его
                db.Items.Remove(item);
                await db.SaveChangesAsync();
                return Results.Json(item);
            });


            app.MapPost("/api/items", async (Item item, ItemsContext db) =>
            {
                // добавляем элемент в массив
                await db.Items.AddAsync(item);
                await db.SaveChangesAsync();
                return item;
            });

            app.MapPut("/api/items", async (Item itemData, ItemsContext db) =>
            {
                // получаем элемент по id
                var item = await db.Items.FirstOrDefaultAsync(u => u.Id == itemData.Id);

                // если не найден, отправляем статусный код и сообщение об ошибке
                if (item == null) return Results.NotFound(new { message = "Элемент не найден" });

                // если эелемент найден, изменяем его данные и отправляем обратно клиенту
                item.Count = itemData.Count;
                item.Name = itemData.Name;
                await db.SaveChangesAsync();
                return Results.Json(item);
            });

            app.Run();
        }
    }
}