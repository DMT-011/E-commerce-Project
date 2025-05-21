using Microsoft.AspNetCore.Mvc;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;


namespace E_commerce_Project.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly ICategoryService _categoryService;
    public HomeController(IProductService productService, IProductImageService productImageService
    , ICategoryService categoryService)
    {
       _productService = productService;
       _productImageService = productImageService;
       _categoryService = categoryService;
    }

    public IActionResult Index()
    {
        var categories = _categoryService.GetCategoriesWithProducts();
        return View(categories);
    }
}