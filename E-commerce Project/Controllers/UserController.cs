using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Cart()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }
}