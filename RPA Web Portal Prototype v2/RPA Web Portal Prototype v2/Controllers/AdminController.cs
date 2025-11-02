using Microsoft.AspNetCore.Mvc;
using RPA_Web_Portal_Prototype_v2.DTO;
using RPA_Web_Portal_Prototype_v2.Model;
using RPA_Web_Portal_Prototype_v2.Repositories;
using RPA_Web_Portal_Prototype_v2.Services;

namespace RPA_Web_Portal_Prototype_v2.Controllers;

public class AdminController : Controller
{
    private readonly ThePasswordHasher _hasher;
    private readonly MySqlRepository _repo;

    public AdminController(ThePasswordHasher _hasher, MySqlRepository _repo)
    {
        this._hasher = _hasher;
        this._repo = _repo;
    }
    
    // // GET
    // public IActionResult Index()
    // {
    //     return View();
    // }
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto newUser)
    {
        var newUserToAdd = new User { Username = newUser.Username, PasswordHash = newUser.Password, Role = newUser.Role, Branch = newUser.Branch};
        newUserToAdd.PasswordHash = _hasher.HashThePassword(newUserToAdd, newUser.Password);
        await _repo.AddUser(newUserToAdd);
        return Created();
    }
}