namespace RPA_Web_Portal_Prototype_v2.DTO;

public class UserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }

    public string Role { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
}