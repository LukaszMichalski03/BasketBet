
using BasketBetWebAPI.Entities;
using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Middleware;
using BasketBetWebAPI.Repositories;
using BasketBetWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;

namespace BasketBetWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("default");
            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(
                options => options.UseSqlServer(connectionString)
            );
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
