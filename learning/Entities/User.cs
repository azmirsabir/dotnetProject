using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace learning.Entities;

public class User: BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50, ErrorMessage = "Name can not be more than 50 characters long.")]
    public string Name { get; set; }= string.Empty;
    
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string Username { get; set; }= string.Empty;
    
    public string Role { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string? RefreshToken { get; set; } = string.Empty;
    
    [JsonIgnore]
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; }
}