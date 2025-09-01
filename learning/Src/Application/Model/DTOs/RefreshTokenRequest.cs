using System.ComponentModel.DataAnnotations;

namespace learning.Application.Model.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public int userId { get; set; }
    
    [Required]
    public required string refreshToken { get; set; }
}