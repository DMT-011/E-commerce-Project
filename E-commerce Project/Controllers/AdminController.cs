using System.Security.Claims;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.AuthService;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Models.Services.UserService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace E_commerce_Project.Controllers;

[Authorize(AuthenticationSchemes = "CookieAuthAdmin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISliderService _sliderService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AdminController(ApplicationDbContext context, IProductService productService
        , ISliderService sliderService, IOrderService orderService, ICategoryService categoryService,
        IUserService userService, IAuthService authService)
    {
        _context = context;
        _sliderService = sliderService;
        _productService = productService;
        _orderService = orderService;
        _categoryService = categoryService;
        _userService = userService;
        _authService = authService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.returnUrl = returnUrl;
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl, AdminLoginViewModel model)
    {
        var user = await _authService.AuthenticateAdmin(model);
        if (user == null) throw new Exception("Admin does not exists");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("userId", user.Id.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "CookieAuthAdmin");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("CookieAuthAdmin", principal);
        
        TempData["title"] = $"Đăng nhập thành công";
        TempData["message"] = $"Xin chào quản trị viên {user.FullName}.";
        TempData["icon"] = "fas fa-info";
        TempData["type"] = "info";  
        
        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        
        return RedirectToAction("Index", "Admin");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuthAdmin");
        TempData["title"] = $"Đã đăng xuất";
        TempData["message"] = $"Đã đăng xuất khỏi hệ thống";
        TempData["icon"] = "fas fa-info";
        TempData["type"] = "info";  
        return RedirectToAction("Login", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        await _userService.CreateUserAsync(model);
        TempData["title"] = $"Thêm thành công";
        TempData["message"] = $"Đã thêm thành công tài khoản có tên {model.FullName}.";
        TempData["icon"] = "fas fa-check";
        TempData["type"] = "success";
        return View();
    }

    public IActionResult Product(int? page)
    {
        var products = _productService.GetProductsWithPaginationAdmin(page);
        var count = _context.Products.Count(item => item.IsDeleted == true);
        ViewBag.countProductDel = count;
        return View(products);
    }

    public IActionResult Slider(int? page)
    {
        var sliders = _sliderService.GetSlidersWithPaginationAdmin(page);
        var count = _context.Slides.Count(item => item.IsDeleted == true);
        ViewBag.countSliderDel = count;
        return View(sliders);
    }

    public IActionResult Order(int? page)
    {
        var orders = _orderService.GetOrdersWithPaginationAdmin(page);
        var count = _context.Orders.Count(item => item.IsDeleted == true);
        ViewBag.countOrderDel = count;
        return View(orders);
    }

    public IActionResult Category(int? page)
    {
        ViewBag.countCategoryDel = _context.Categories.Count(item => item.IsDeleted == true);
        var categories = _categoryService.GetCategoriesWithPaginationAdmin(page);
        return View(categories);
    }
}