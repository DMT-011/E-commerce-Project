using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Views.ViewComponents;

public class ProductSalesViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly IProductImageService _productImageService;
    
    public ProductSalesViewComponent(ApplicationDbContext context , IProductImageService productImageService)
    {
        _context = context;
        _productImageService = productImageService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var productSales = await _context.Products
            .Where(item =>
                item.HasDiscount == true &&
                item.IsDisplayed == true &&
                item.IsDeleted == false)
            .OrderByDescending(item => item.UpdatedAt)
            .Take(3).Select(item => new ProductSalesViewModel
            {
                Name = item.Name,
                Price = item.Price.ToString(),
                PromotionPrice = item.PromotionPrice.ToString(),
                Slug = item.Slug,
                ImagePath = _productImageService.GetImageMainProductById(item.Id)  ,
            }).ToListAsync();
        
       return View(productSales);
    } 
}