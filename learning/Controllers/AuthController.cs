using learning.Entities;
using learning.Model.Dtos;
using learning.Model.DTOs;
using learning.Model.Response;
using learning.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learning.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService _authService): ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<Response<LoginResponse>>> Login(LoginRequest request)
    {
        var auth = await _authService.LoginAsync(request);
        return Ok(new Response<LoginResponse>("Login successful ",auth));
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<Response<User>>> Register(UserDto request)
    {
        var user=await _authService.RegisterAsync(request);
        return Ok(new Response<User>("Register successful", user));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken(RefreshTokenRequest request)
    {
        var auth=await _authService.RefreshTokensAsync(request);
        return Ok(new Response<LoginResponse>("Token refreshed successfully",auth));
    }
}
