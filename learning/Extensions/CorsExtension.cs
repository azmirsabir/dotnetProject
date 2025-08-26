namespace learning.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var trustedOrigins = configuration["TrustedOrigins"]?
                                 .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) 
                             ?? Array.Empty<string>();
        
        services.AddCors(options => {
            
            options.AddPolicy("AllowTrusted", policy =>
            {
                policy.WithOrigins(trustedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
            
            options.AddPolicy("AllowAll",
                policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        return services;
    }
}