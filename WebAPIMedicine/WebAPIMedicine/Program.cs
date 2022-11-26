using Microsoft.EntityFrameworkCore;
using WebAPIMedicine.Models;

namespace WebAPIMedicine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // �������� ������ ����������� �� ����� ������������
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // ��������� �������� ItemsContext � �������� ������� � ����������
            builder.Services.AddDbContext<ItemsContext>(options => options.UseSqlServer(connection));
            
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.MapGet("/", (ItemsContext db) => db.Items.ToList());
            app.MapGet("/api/items", async (ItemsContext db) => await db.Items.ToListAsync());

            app.MapGet("/api/items/{id}", async (int id, ItemsContext db) =>
            {
                // �������� ������� �� id
                Item? item = await db.Items.FirstOrDefaultAsync(u => u.Id == id);

                // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
                if (item == null) return Results.NotFound(new { message = "������� �� ������" });

                // ���� ������� ������, ���������� ���
                return Results.Json(item);
            });

            app.MapDelete("/api/items/{id}", async (int id, ItemsContext db) =>
            {
                // �������� ������� �� id
                Item? item = await db.Items.FirstOrDefaultAsync(u => u.Id == id);

                // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
                if (item == null) return Results.NotFound(new { message = "������� �� ������" });

                // ���� ������������ ������, ������� ���
                db.Items.Remove(item);
                await db.SaveChangesAsync();
                return Results.Json(item);
            });


            app.MapPost("/api/items", async (Item item, ItemsContext db) =>
            {
                // ��������� ������� � ������
                await db.Items.AddAsync(item);
                await db.SaveChangesAsync();
                return item;
            });

            app.MapPut("/api/items", async (Item itemData, ItemsContext db) =>
            {
                // �������� ������� �� id
                var item = await db.Items.FirstOrDefaultAsync(u => u.Id == itemData.Id);

                // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
                if (item == null) return Results.NotFound(new { message = "������� �� ������" });

                // ���� �������� ������, �������� ��� ������ � ���������� ������� �������
                item.Count = itemData.Count;
                item.Name = itemData.Name;
                await db.SaveChangesAsync();
                return Results.Json(item);
            });

            app.Run();
        }
    }
}