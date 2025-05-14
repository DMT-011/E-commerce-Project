using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IProductImageService _productImageService;

    public AdminController(ApplicationDbContext context, IProductImageService productImageService)
    {
        _context = context;
        _productImageService = productImageService;
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
}