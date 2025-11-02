namespace RPA_Web_Portal_Prototype_v2.Model;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked {get; set; }
    public int? ReplacedByToken { get; set; }

    private bool isExpired => DateTime.UtcNow >= Expires;
    public bool isActive => !isExpired && ReplacedByToken == null;
}