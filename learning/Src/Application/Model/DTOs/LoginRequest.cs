using System.ComponentModel.DataAnnotations;

namespace learning.Application.Model.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}