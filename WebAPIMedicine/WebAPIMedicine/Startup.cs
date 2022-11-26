using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebAPIMedicine.Models;

namespace WebAPIMedicine
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(); // используем контроллеры без представлений
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // подключаем маршрутизацию на контроллеры
            });
        }
    }
}
