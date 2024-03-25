using BasketBet.EntityFramework;
using BasketBet.Web.Interfaces;
using BasketBet.Web.Repositories;

namespace BasketBet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var basePath = System.IO.Directory.GetCurrentDirectory();
            var appSettingsPath = Path.Combine(basePath, "appsettings.json");
            var databaseSettingsPath = Path.Combine(Directory.GetParent(basePath).FullName, "BasketBet.EntityFramework", "databasesettings.json");

            builder.Configuration
                .SetBasePath(basePath)
                .AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true)
                .AddJsonFile(databaseSettingsPath, optional: false, reloadOnChange: true);
            // Add services to the container.
            builder.Services.AddScoped<IGamesRepository, GamesRepository>();
            builder.Services.AddScoped<IBetRepository, BetRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITeamsRepository, TeamsRepository>();
            builder.Services.AddControllersWithViews();

            builder.Services.RegisterDataServices(builder.Configuration);

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
