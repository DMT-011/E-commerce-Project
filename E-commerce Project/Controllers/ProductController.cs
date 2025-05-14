using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IProductImageService _productImageService;
    private readonly ApplicationDbContext _context;

    public ProductController(IProductService productService, ICategoryService categoryService
    , IProductImageService productImageService, ApplicationDbContext context)
    {
        _productService = productService;
        _categoryService = categoryService;
        _productImageService = productImageService;
        _context = context;
    }
    public IActionResult Index(int id)
    {
        return View();
    }

    public async Task<IActionResult> Create()
    {
        var model = new ProductCreateViewModel
        {
            Categories = await _categoryService.GetCategoriesWithSelectList()
        };
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        await _productService.CreateProductAsync(model);
        return RedirectToAction("Product", "Admin");
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        var model = new ProductUpdateViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Price = product.Price.ToString(),
            PromotionPrice = product.PromotionPrice.ToString(),
            Description = product.Description,
            Detail = product.Detail,
            HasDiscount = product.HasDiscount,
            IsDisplayed = product.IsDisplayed,
            CategoryId = product.CategoryId,
            Categories = await _categoryService.GetCategoriesWithSelectList(),
            ImageMainPath = _productImageService.GetImageMainProductById(product.Id),
            ImageSub1Path = _productImageService.GetImageSubProductByOrder(1, product.Id),
            ImageSub2Path = _productImageService.GetImageSubProductByOrder(2, product.Id),
            ImageSub3Path = _productImageService.GetImageSubProductByOrder(3, product.Id),
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateViewModel model)
    {
        await _productService.UpdateProductAsync(id, model);
        return RedirectToAction("Product", "Admin");
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction("Product", "Admin");
    }
    
    public IActionResult Trash()
    {
        var products = _context.Products
            .Where(item => item.IsDeleted == true)
            .Include(item => item.Category)
            .Select(item => new ProductTrashViewModel
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

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _productService.RestoreProductAsync(id);
        return RedirectToAction("Product", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _productService.ForceDeleteProductAsync(id);
        return RedirectToAction("Trash");
    }
}