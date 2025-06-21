using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.ViewModels.CartViewModel;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IProductImageService _productImageService;
    private readonly ICartService _cartService;

    public ProductController(IProductService productService, ICategoryService categoryService
    , IProductImageService productImageService, ICartService cartService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _productImageService = productImageService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(string slug)
    {
        var product = await _productService.GetProductBySlugAsync(slug);
        var model = new ProductDetailViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.StockQuantity,
            Price = product.Price,
            PromotionPrice = product.PromotionPrice,
            Description = product.Description,
            Details = product.Detail,
            HasDiscount = product.HasDiscount,
            ImageMainPath = _productImageService.GetImageMainProductById(product.Id),
            ImageSub1Path = _productImageService.GetImageSubProductByOrder(1, product.Id),
            ImageSub2Path = _productImageService.GetImageSubProductByOrder(2, product.Id),
            ImageSub3Path = _productImageService.GetImageSubProductByOrder(3, product.Id),
        };
        return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> AddProductToCart([FromBody] CartItemViewModel model)
    {
        var cartId = int.Parse(User.FindFirst("cartId")?.Value);
        var respone=  await _cartService.AddProductToCartAsync(model, cartId);
        return Json(respone);
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
        TempData["title"] = $"Thêm thành công";
        TempData["message"] = $"Đã thêm thành công sản phẩm {model.Name}.";
        TempData["icon"] = "fas fa-check";
        TempData["type"] = "success";
        return RedirectToAction("Product", "Admin");
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        var model = new ProductUpdateViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.StockQuantity,
            Price = CurrencyFormatterHelper.Format(product.Price),
            PromotionPrice = CurrencyFormatterHelper.Format(product.PromotionPrice ?? 0),
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
        TempData["title"] = $"Cập nhập thành công";
        TempData["message"] = $"Thông tin sản phẩm có ID = {id} đã được cập nhật.";
        TempData["icon"] = "fas fa-edit";
        TempData["type"] = "info";
        return RedirectToAction("Product", "Admin");
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        TempData["title"] = $"Đã chuyển vào thùng rác";
        TempData["message"] = $"Sản phẩm có ID = {id} đã được đưa vào thùng rác.";
        TempData["icon"] = "fas fa-trash";
        TempData["type"] = "warning";
        return RedirectToAction("Product", "Admin");
    }
    
    public IActionResult Trash(int? page)
    {
        var products = _productService.GetProductsWithPaginationAdminTrash(page);
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _productService.RestoreProductAsync(id);
        TempData["title"] = $"Khôi phục thành công";
        TempData["message"] = $"Sản phẩm có ID = {id} đã được khôi phục.";
        TempData["icon"] = "fas fa-sync-alt";
        TempData["type"] = "success";
        return RedirectToAction("Trash", "Product");
    }

    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _productService.ForceDeleteProductAsync(id);
        TempData["title"] = "Đã xóa vĩnh viễn";
        TempData["message"] = $"Sản phẩm có ID = {id} đã bị xóa vĩnh viễn khỏi hệ thống.";
        TempData["icon"] = "fas fa-times";
        TempData["type"] = "danger";
        return RedirectToAction("Trash", "Product");
    }
}