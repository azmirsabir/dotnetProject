using learning.Entities;
using learning.Model.Dtos;
using learning.Model.DTOs;
using learning.Model.Response;

namespace learning.Services.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(UserDto request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokensAsync(RefreshTokenRequest request);
}