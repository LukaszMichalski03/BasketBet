
using BasketBet.EntityFramework.Data;
using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Middleware;
using BasketBetWebAPI.Repositories;
using BasketBetWebAPI.Services;
using BasketBet.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;

namespace BasketBetWebAPI
{
    //https://stackoverflow.com/questions/60407040/how-to-add-migration-in-the-multi-project-solution kilka projektow migracje
    // https://medium.com/swlh/creating-a-multi-project-net-core-database-solution-a69decdf8d7e
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

            builder.Services.RegisterDataServices(builder.Configuration);

            builder.Services.AddScoped<IGamesRepository, GamesRepository>();
            builder.Services.AddScoped<ITeamsRepository, TeamsRepository>();

            builder.Services.AddScoped<Scrapper>();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Host.UseNLog();
            builder.Services.AddScoped<ErrorHandlingMiddleware>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasketBetWebAPI", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketBetWebAPI v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
