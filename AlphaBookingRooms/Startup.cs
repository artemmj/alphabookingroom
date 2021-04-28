using AlphaBookingRooms.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaBookingTest
{
    /*
     * Этот класс производит конфигурацию приложения, настраивает сервисы, которые приложение будет
     * использовать, устанавливает компоненты для обработки запроса или middleware.
    */
    public class Startup
    {
        // Конфигурационные свойства
        public IConfiguration Configuration { get; }

        // 1. Конструктор - производится начальная конфигурация приложения.
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // 2. Необязательный метод регистрирует сервисы, которые используются приложением.
        public void ConfigureServices(IServiceCollection services)
        {
            // Подключение к локальной БД MSSQL
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            // Контекст
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

			// Установка конфигурации подключения
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				// В этом методе производится конфигурация объекта CookieAuthenticationOptions,
				// который описывает параметры аутентификации с помощью кук.
				.AddCookie(options =>
				{
					// Это свойство устанавливает относительный путь, по которому будет перенаправляться 
					// анонимный пользователь при доступе к ресурсам, для которых нужна аутентификация.
					options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
				});

			// Метод добавляет в коллекцию сервисов сервисы MVC ASP Net Core 2.1.
			// После добавления в коллекцию сервисов добавленные сервисы становятся доступными для приложения.
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // 3. Метод устанавливает, как приложение будет обрабатывать запрос.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Если приложение в процессе разработки
            if (env.IsDevelopment())
            {
                // То выводим информацию об ошибке, при наличии ошибки
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Установка обработчика ошибок
                app.UseExceptionHandler("/Home/Error");
            }

			// Чтобы аутентификация была встроена в конвейер обработки запроса
			app.UseAuthentication();
            // Установка использования HTTPS
            app.UseHttpsRedirection();
            // Подключаем файлы по умолчанию
            app.UseDefaultFiles();
            // Подключаем статические файлы
            app.UseStaticFiles();

			// Подключаем единственный маршрут по-умолчанию
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
