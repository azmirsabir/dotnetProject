using learning.Helpers;
using learning.Services.Interfaces;
using learning.Data;
using learning.Data.Seeders;
using learning.Services;
using Microsoft.EntityFrameworkCore;

namespace learning.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //connect to DB
        var connectionString = configuration.GetConnectionString("DefaultConnection"); 
        services.AddDbContext<ApplicationDbContext>(options => {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        
        //Service registrations
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<DBSeeder>();
        services.AddScoped<JWT>();
        return services;
    }
}