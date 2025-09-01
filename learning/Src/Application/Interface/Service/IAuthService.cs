using learning.Application.Model.DTOs;
using learning.Application.Model.Response;
using learning.Domain.Entities;

namespace learning.Application.Interface.Service;

public interface IAuthService
{
    Task<User> RegisterAsync(UserDto request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokensAsync(RefreshTokenRequest request);
}