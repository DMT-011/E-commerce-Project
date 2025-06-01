using E_commerce_Project.Models.Context;
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

    public AdminController(ApplicationDbContext context, IProductService productService
    , ISliderService sliderService)
    {
        _context = context;
        _sliderService = sliderService;
        _productService = productService;
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
    
    
}