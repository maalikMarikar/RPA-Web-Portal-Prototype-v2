using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RPA_Web_Portal_Prototype_v2.Model;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RPA_Web_Portal_Prototype_v2.Services;

public class TokenGenerator
{
    private readonly IConfiguration _config;

    public TokenGenerator(IConfiguration _config) => this._config = _config;

    public string GenerateJwtToken(User requestedUser)
    {
        var jwtKey = _config.GetValue<string>("Jwt:Key")??"";
        var jwtIssuer = _config.GetValue<string>("Jwt:Issuer");
        var jwtAudience = _config.GetValue<string>("Jwt:Audience");
        var jwtKeyEncrypted = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var creds = new SigningCredentials(jwtKeyEncrypted, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, requestedUser.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, requestedUser.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds
        );

        var toReturn = new JwtSecurityTokenHandler().WriteToken(token);
        return toReturn;
    }

    public RefreshToken GenerateRefreshToken()
    {
        var byteArray = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(byteArray);
        var token = Convert.ToBase64String(byteArray);
        RefreshToken toReturn = new RefreshToken
        {
            Token = token,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        return toReturn;
    }
}