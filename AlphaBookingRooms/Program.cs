using AlphaBookingRooms;
using AlphaBookingRooms.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AlphaBookingTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
			/* 
			 * Чтобы запустить приложение ASP.NET Core, необходим объект IWebHost,
             * в рамках которого развертывается веб-приложение.
             * Для создания IWebHost применяется объект IWebHostBuilder.
            */
			var host = CreateWebHostBuilder(args).Build();

            /*
             * Заполнить БД первоначальными данными
             */
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Заполнить таблицу Пользователей и Комнат начальными данными
                    var UsersContext = services.GetRequiredService<ApplicationContext>();
					StartInitialization.UsersInitialize(UsersContext);
                }
                catch (Exception ex) { }
            }

            host.Run();                                               // запускаем приложение
        }

        /*
         * Метод UseStartup<Startup>() устанавливает класс Startup в качестве стартового.
         * При запуске приложения среда ASP.NET будет искать
         * в сборке приложения класс с именем Startup и загружать его.
        */
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
