using learning.Infrastructure.Persistence.Data.Seeders;

namespace learning.SharedKernel.Extensions;

public static class MigrationManagerExtension
{
    public static async Task<IApplicationBuilder> UseDatabaseSeeder(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DBSeeder>();
        await seeder.SeedDataAsync();
        return app;
    }
}