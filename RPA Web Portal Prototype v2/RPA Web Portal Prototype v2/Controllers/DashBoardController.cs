using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RPA_Web_Portal_Prototype_v2.Controllers;

public class DashBoardController : Controller
{
    [HttpGet]
    [Authorize
    ]
    public IActionResult AdminPanel()
    {
        return Ok("Hello Admin");
    }
}