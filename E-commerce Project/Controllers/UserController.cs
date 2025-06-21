using System.Security.Claims;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.AuthService;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.UserService;
using E_commerce_Project.Models.ViewModels.CartViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly IProductImageService _productImageService;
    private readonly IOrderService _orderService;
    public UserController(IUserService userService, IAuthService authService
    , ICartService cartService, IProductImageService productImageService , IOrderService orderService)
    {
        _userService = userService;
        _authService = authService;
        _cartService = cartService;
        _productImageService = productImageService;
        _orderService = orderService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Order()
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var model = _orderService.GetOrderDetails(userId);
        return View(model);
    }

    [Authorize(AuthenticationSchemes = "CookieAuthCustomer")]
    public async Task<IActionResult> Cart()
    {
        var cartId = int.Parse(User.FindFirst("cartId")?.Value);
        var cartItems = await _cartService.GetAllCartItemsAsync(cartId);
        var model = cartItems.Select(item => new CartItemTableViewModel
        {
            Id = item.Id,
            Name = item.Product.Name,
            Price = item.Price,
            Slug = item.Product.Slug,
            ImagePath = _productImageService.GetImageMainProductById(item.Product.Id),
            QuantityOrder = item.Quantity,
            TotalPrice = item.TotalPrice
        });
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserCreateViewModel model)
    {
        await _userService.CreateUserAsync(model);
        TempData["title"] = "Đăng ký thành công"; 
        TempData["message"] = "Nhấn OK để tiếp tục đi người anh em";
        TempData["textBtn"] = "OK";
        TempData["type"] = "success";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string? returnUrl, UserLoginViewModel model)
    {
        var user = await _authService.AuthenticateCustomer(model);
        
        if(user == null) throw new Exception("User does not exists");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("userId", user.Id.ToString()),
            new Claim("cartId", user.Cart.Id.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, "CookieAuthCustomer");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);
        
        TempData["title"] = "Đăng nhập thành công"; 
        TempData["message"] = "Nhấn OK để tiếp tục";
        TempData["textBtn"] = "OK";
        TempData["type"] = "success";
        
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuthCustomer");
        TempData["title"] = "Đã đăng xuất"; 
        TempData["message"] = " ";
        TempData["textBtn"] = "OK";
        TempData["type"] = "info";
        return RedirectToAction("Index", "Home");
    }
}