using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Models.Services.UserService;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Mvc;


namespace E_commerce_Project.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISliderService _sliderService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;

    public AdminController(ApplicationDbContext context, IProductService productService
    , ISliderService sliderService, IOrderService orderService, ICategoryService categoryService,
    IUserService userService)
    {
        _context = context;
        _sliderService = sliderService;
        _productService = productService;
        _orderService = orderService;
        _categoryService = categoryService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
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