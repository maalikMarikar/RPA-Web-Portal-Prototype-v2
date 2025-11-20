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

    public IActionResult UserLogin()
    {
        // Check TempData first (normal server redirects)
        if (TempData["Msg"] != null)
        {
            ViewBag.Message = TempData["Msg"].ToString();
        }
        // Check query string (for JS redirects)
        else if (Request.Query.ContainsKey("msg"))
        {
            ViewBag.Message = Request.Query["msg"].ToString();
        }

        return View();
    }

    
    [HttpPost]
    public async Task<IActionResult> UserLogin(string username, string password)
    {
        var user = await _repo.GetUserByName(username);
        if (user is null)
        {
            return View();
        }
        var result = _hasher.VerifyPw(user, password);
        if (!result)
        {
            ViewBag.Error = "Invalid Password";
            return View();
            
        }
        
        var accessToken = _generator.GenerateJwtToken(user);
        var refreshToken = _generator.GenerateRefreshToken();
    
        await _repo.UserTokenBinding(user, refreshToken);
        
        Response.Cookies.Append("jwt", accessToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        Response.Cookies.Append("rt", refreshToken.Token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        Response.Cookies.Append("uzn",user.Username, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });
        
        Response.Cookies.Append("brnch", user.Branch, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });
        
        Response.Cookies.Append("dtnow",System.DateTime.Now.ToString("dd-MM-yyyy"), new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });
    
        if (user.Role == "inputter")
        {
            return RedirectToAction("InputterPage", "MainPage");
        }
        
        if (user.Role == "auth")
        {
            
            return RedirectToAction("authPage", "MainPage");
        }
        
        
        return Ok(new{Token = accessToken, RefreshToken = refreshToken});
    }
    
    
    
    
    // [HttpPost]
    // public async Task<IActionResult> UserLogin([FromBody] UserDto un)
    // {
    //     var user = await _repo.GetUserByName(un.Username);
    //     if (user is null)
    //     {
    //         ViewBag.Error = "Invalid Username";
    //         return View();
    //     }
    //     var result = _hasher.VerifyPw(user, un.Password);
    //     if (!result)
    //     {
    //         ViewBag.Error = "Invalid Password";
    //         return View();
    //         
    //     }
    //     
    //     var accessToken = _generator.GenerateJwtToken(user);
    //     var refreshToken = _generator.GenerateRefreshToken();
    //
    //     await _repo.UserTokenBinding(user, refreshToken);
    //     
    //     Response.Cookies.Append("jwt", accessToken, new CookieOptions
    //     {
    //         HttpOnly = false,
    //         Secure = false,
    //         SameSite = SameSiteMode.Lax,
    //         Expires = DateTime.UtcNow.AddDays(7)
    //     });
    //     
    //     Response.Cookies.Append("rt", refreshToken.Token, new CookieOptions
    //     {
    //         HttpOnly = false,
    //         Secure = false,
    //         SameSite = SameSiteMode.Lax,
    //         Expires = DateTime.UtcNow.AddDays(7)
    //     });
    //
    //     // if (user.Role == "inputter")
    //     // {
    //     //     return RedirectToAction("InputterPage", "MainPage");
    //     // }
    //     //
    //     // if (user.Role == "auth")
    //     // {
    //     //     return RedirectToAction("authPage", "MainPage");
    //     // }
    //     
    //     
    //     return Ok(new{Token = accessToken, RefreshToken = refreshToken});
    // }

    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        
        Console.WriteLine("Cookies received in backend:");
    
        if (Request.Cookies.Count == 0)
        {
            Console.WriteLine("No cookies received at all!");
        }

        foreach (var cookie in Request.Cookies)
        {
            Console.WriteLine($"{cookie.Key} = {cookie.Value}");
        }
        
        
        
        Console.WriteLine("bruhhhhhhhhhhhhhhh");
        
        
        var jwtAccessToken = Request.Cookies["jwt"];
        var rt = Request.Cookies["rt"];
        
        
        
        

        if (string.IsNullOrEmpty(rt))
        {
            Console.WriteLine("No RefreshToken Found in Cookie");
            return Unauthorized("No RefreshToken Found in Cookie");
        }

        var user = await _repo.GetUserByRt(rt);
        if (user == null)
        {
            
            

            // DELETE the cookies since the session is invalid
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("rt");
            

            // Redirect them to login page
            return Unauthorized("User logged in on another device.");


        }
        if (user.RefreshTokenExpiry <= DateTime.UtcNow) return Unauthorized("RefreshToken Expired!");
        
        var retrievedOldToken = await _repo.GetRefreshTokenByToken(rt!);
        var retrievedUser = await _repo.GetUserById(retrievedOldToken!.UserId);
        var newAccessToken = _generator.GenerateJwtToken(retrievedUser!);
        var newRefreshToken = _generator.GenerateRefreshToken();
        await _repo.UserTokenBinding(rt!, newRefreshToken);
        Response.Cookies.Append("jwt", newAccessToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        Response.Cookies.Append("rt", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        return Ok();
    }
    
    
    
    
}