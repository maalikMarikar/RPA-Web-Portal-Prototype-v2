using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RPA_Web_Portal_Prototype_v2.Controllers;

public class MainPageController : Controller
{
    // GET
    [Authorize(Roles = "inputter")]
    public IActionResult InputterPage()
    {
        return View();
    }
    
    [Authorize(Roles = "inputter")]
    public IActionResult InputterDoSomething()
    {
        var message = "Hello Inputter";
        return Ok(message);
    }
    
    [Authorize(Roles = "auth")]
    public IActionResult AuthPage()
    {
        return View();
    }
    
    [Authorize(Roles = "auth")]
    public IActionResult AuthDoSomething()
    {
        var message = "Hello Authorizer";
        return Ok(message);
    }
}