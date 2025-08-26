using System.Net;
using learning.Data;
using learning.Entities;
using learning.Exceptions;
using learning.Helpers;
using learning.Model.Dtos;
using learning.Model.DTOs;
using learning.Model.Response;
using learning.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace learning.Services;

public class AuthService(ApplicationDbContext context,JWT _jwt, IUserService _userService) : IAuthService
{
    public async Task<User> RegisterAsync(UserDto request)
    {
        var user= await _userService.SaveUserAsync(request);
        return user;
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user =await context.Users.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (user is null) 
            throw new CustomException(HttpStatusCode.Unauthorized,"Invalid username or password");

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
            PasswordVerificationResult.Failed)
        {
            throw new CustomException(HttpStatusCode.Unauthorized,"Invalid username or password");
        }

        return new LoginResponse
        {
            Token = _jwt.CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
    }
    public async Task<LoginResponse> RefreshTokensAsync(RefreshTokenRequest request)
    {
        var user = await context.Users.FindAsync(request.userId);
        if (user is null || user.RefreshToken != request.refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new CustomException(HttpStatusCode.Unauthorized,"Invalid refresh token");
        }
        
        return new LoginResponse
        {
            Token = _jwt.CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        };
    }
    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = _jwt.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await context.SaveChangesAsync();
        return refreshToken;
    }
}