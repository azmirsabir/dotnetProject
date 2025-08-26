using learning.Entities;
using Microsoft.AspNetCore.Identity;

namespace learning.Data.Seeders;

public class DBSeeder(ApplicationDbContext context)
{
    public async Task SeedDataAsync()
    {
        if (!context.Users.Any())
        {
            var user1 = new User { Name = "Azmir Sabir", Username = "azmir", Role = "admin",RefreshToken = "PMpwdejoKcxwFNL/Xy/OF8r8aEetixUrCfWjbuFbpMk=",RefreshTokenExpiryTime = DateTime.UtcNow};
            user1.PasswordHash = new PasswordHasher<User>().HashPassword(user1, "123456");

            var user2 = new User { Name = "Aryo Hadi", Username = "aryo", Role = "user",RefreshToken = "fMpwdejoKcxwFdL/Xy/OF8g8aEetixUrCfWjbuFbpMk=",RefreshTokenExpiryTime = DateTime.UtcNow };
            user2.PasswordHash = new PasswordHasher<User>().HashPassword(user2, "123456");

            var user3 = new User { Name = "Sabir Sleman", Username = "sabir", Role = "admin",RefreshToken = "qMpwdejogcxwFNL/Xy/OF8r8aEetixUrCfWjbgFbpMk=",RefreshTokenExpiryTime = DateTime.UtcNow };
            user3.PasswordHash = new PasswordHasher<User>().HashPassword(user3, "123456");

            context.Users.AddRange(user1, user2, user3);
        }
    }
}