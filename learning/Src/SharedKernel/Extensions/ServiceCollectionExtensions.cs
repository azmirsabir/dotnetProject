using learning.Application.Interface.Service;
using learning.Infrastructure.Persistence.Data;
using learning.Infrastructure.Persistence.Data.Seeders;
using learning.Infrastructure.Service;
using learning.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore;

namespace learning.SharedKernel.Extensions;

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