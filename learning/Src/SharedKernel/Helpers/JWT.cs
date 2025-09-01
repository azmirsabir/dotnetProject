using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using learning.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace learning.SharedKernel.Helpers;

public class JWT(IConfiguration _configuration)
{
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")));
        var creds=new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static User GetCurrentUser(ClaimsPrincipal user)
    {
        User u = new User();
        u.Name=user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userId= int.TryParse(userIdValue, out var id)
            ? id
            : throw new UnauthorizedAccessException("Invalid or missing user ID in claims.");
        
        var role=userRole is not null ? userRole : throw new UnauthorizedAccessException("Invalid or missing role in claims.");
        
        u.Id=userId;
        u.Role=role;

        return u;

    }
}