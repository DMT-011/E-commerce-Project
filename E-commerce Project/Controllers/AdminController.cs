using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using Microsoft.AspNetCore.Mvc;


namespace E_commerce_Project.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISliderService _sliderService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;

    public AdminController(ApplicationDbContext context, IProductService productService
    , ISliderService sliderService, IOrderService orderService)
    {
        _context = context;
        _sliderService = sliderService;
        _productService = productService;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Product(int? page)
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
}