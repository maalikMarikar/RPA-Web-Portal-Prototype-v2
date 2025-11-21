using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA_Web_Portal_Prototype_v2.Model;
using RPA_Web_Portal_Prototype_v2.Repositories;

namespace RPA_Web_Portal_Prototype_v2.Controllers;

public class MainPageController : Controller
{
    private readonly MySqlRepository _repo;
    public MainPageController(MySqlRepository _repo)
    {
        this._repo = _repo;
    }
    
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

    public IActionResult RegularSettlement()
    {
        return PartialView();
    }

    [HttpPost]
    [Authorize(Roles = "inputter")]
    public async Task<IActionResult> BrUserSaveTrx(string userr, string theName, int theNumber, string theCif, string theDeal, string theProduct, string theFin, string f1, string f2, string f3, string f4, string f5)
    {
        TransactionBase trx = new RegularSettlement
        {
            BranchId = theNumber,
            BranchName = theName,
            SubmittedUser = userr,
            DateTimeSubmitted = System.DateTime.UtcNow,
            Cif = theCif,
            DealNo = theDeal,
            ProductType = theProduct,
            FinalAmount = theFin,

            Rcf1 = f1,
            Rcf2 = f2,
            Rcf3 = f3,
            Rcf4 = f4,
            Rcf5 = f5
        };

        await _repo.SaveTransactionToDb(trx);
        
        
        return Ok("Added to Db");
    }
}