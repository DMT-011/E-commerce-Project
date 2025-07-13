using E_commerce_Project.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

public class CustomerController : Controller
{
    private readonly IUserService _userService;

    public CustomerController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize(AuthenticationSchemes = "CookieAuthAdmin")]
    public IActionResult Trash(int? page)
    {
        var model = _userService.GetAllCustomerTrashWithPagination(page);
        return View(model);       
    }
}