using Microsoft.AspNetCore.Mvc;
using RPA_Web_Portal_Prototype_v2.DTO;
using RPA_Web_Portal_Prototype_v2.Model;
using RPA_Web_Portal_Prototype_v2.Repositories;
using RPA_Web_Portal_Prototype_v2.Services;

namespace RPA_Web_Portal_Prototype_v2.Controllers;

public class LoginController : Controller
{
    private readonly TokenGenerator _generator;
    private readonly ThePasswordHasher _hasher;
    private readonly MySqlRepository _repo;

    public LoginController(TokenGenerator _generator, ThePasswordHasher _hasher, MySqlRepository _repo)
    {
        this._generator = _generator;
        this._hasher = _hasher;
        this._repo = _repo;
    }
    [HttpPost]
    public async Task<IActionResult> UserLogin([FromBody] UserDto userRequestDto)
    {
        var user = await _repo.GetUserByName(userRequestDto.Username);
        if (user is null) return Unauthorized("Invalid Username");
        var result = _hasher.VerifyPw(user, userRequestDto.Password);
        if(!result) return Unauthorized("Invalid Password");
        
        var accessToken = _generator.GenerateJwtToken(user);
        var refreshToken = _generator.GenerateRefreshToken();

        await _repo.UserTokenBinding(user, refreshToken);
        
        Response.Cookies.Append("jwt", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        Response.Cookies.Append("rt", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        
        
        return Ok(new{Token = accessToken, RefreshToken = refreshToken});
    }

    public async Task<IActionResult> Refresh()
    {
        var jwtAccessToken = Request.Cookies["jwt"];
        var rt = Request.Cookies["rt"];

        if (string.IsNullOrEmpty(rt)) return Unauthorized("No RefreshToken Found in Cookie");

        var user = await _repo.GetUserByRt(rt);
        if (user == null) return Unauthorized("Logged in another Session!");

        if (user.RefreshTokenExpiry <= DateTime.UtcNow) return Unauthorized("RefreshToken Expired!");
        
        var retrievedOldToken = await _repo.GetRefreshTokenByToken(rt!);
        var retrievedUser = await _repo.GetUserById(retrievedOldToken!.UserId);
        var newAccessToken = _generator.GenerateJwtToken(retrievedUser!);
        var newRefreshToken = _generator.GenerateRefreshToken();
        await _repo.UserTokenBinding(rt!, newRefreshToken);
        Response.Cookies.Append("jwt", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        Response.Cookies.Append("rt", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        return Ok();
    }
    
    
    
    
}