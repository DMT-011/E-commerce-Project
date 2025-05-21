using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IProductImageService _productImageService;
    private readonly ISliderService _sliderService;

    public AdminController(ApplicationDbContext context, IProductImageService productImageService
    , ISliderService sliderService)
    {
        _context = context;
        _productImageService = productImageService;
        _sliderService = sliderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Product()
    {
        var count = _context.Products.Count(item => item.IsDeleted == true);
        ViewBag.countProductDel = count;
        
        var products = _context.Products
            .Where(item => item.IsDeleted == false)
            .Include(item => item.Category)
            .Select(item => new AdminProductListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                PromotionPrice = item.PromotionPrice,
                ImagePath = _productImageService.GetImageMainProductById(item.Id),
                Quantity = item.Quantity,
                CategoryName = item.Category.Name,
                CreatedDate = item.CreatedAt,
            });
        return View(products);
    }

    public IActionResult Slider()
    {
        ViewBag.countProductDel = _context.Slides.Count(item => item.IsDeleted == true);
        
        var sliders = _sliderService.GetAllSlides()
            .Select(item => new AdminSliderListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ImagePath = item.ImagePath,
            });
        return View(sliders);
    }
}