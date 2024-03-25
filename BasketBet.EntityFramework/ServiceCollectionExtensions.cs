using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketBet.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDataServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("default");

            services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(30);
            });

            return services;
        }
    }

}
