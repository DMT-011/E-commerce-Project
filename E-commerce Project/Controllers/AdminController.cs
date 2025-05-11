using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View(); 
    }
    
    public IActionResult Product()
    {
        return View();
    }
}