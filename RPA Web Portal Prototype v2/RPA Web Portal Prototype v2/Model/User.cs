using System.ComponentModel.DataAnnotations.Schema;

namespace RPA_Web_Portal_Prototype_v2.Model;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "inputter";
    public string Branch { get; set; } = "dehiwala";

    public string? RefreshToken { get; set; } = String.Empty;
    
    public DateTime? RefreshTokenExpiry { get; set; }
}