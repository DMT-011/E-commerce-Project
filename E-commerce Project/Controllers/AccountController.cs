using E_commerce_Project.Models.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

[Authorize(AuthenticationSchemes = "CookieAuthAdmin")]
public class AccountController : Controller
{
    private readonly IUserService _userService; 
    
    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
   
    public IActionResult Trash(int? page)
    {
        var model = _userService.GetAllAccountTrashWithPagination(page);
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _userService.RestoreUserAsync(id);
        TempData["title"] = $"Khôi phục thành công";
        TempData["message"] = $"Tài khoản có ID = {id} đã được khôi phục.";
        TempData["icon"] = "fas fa-sync-alt";
        TempData["type"] = "success";
        return RedirectToAction("Trash", "Account");
    }
}