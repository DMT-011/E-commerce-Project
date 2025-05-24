using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Views.ViewComponents;

public class ProductNewListViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly IProductImageService _productImageService;

    public ProductNewListViewComponent(ApplicationDbContext context, IProductImageService productImageService)
    {
        _context = context;
        _productImageService = productImageService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var productNews = await _context.Products
            .Where(item => item.IsDisplayed == true && item.IsDeleted == false)
            .OrderByDescending(item => item.CreatedAt)
            .Take(8)
            .Select(item => new ProductNewListViewModel
            {
                Name = item.Name,
                Slug = item.Slug,
                ImagePath = _productImageService.GetImageMainProductById(item.Id),
                Price = item.Price.ToString(),
            }).ToListAsync();

        return View(productNews);
    }
}