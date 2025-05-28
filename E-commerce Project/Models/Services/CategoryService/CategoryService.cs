using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IProductImageService _productImageService;

    public CategoryService(ILogger<CategoryService> logger, ApplicationDbContext context
    , IProductImageService productImageService)
    {
        _logger = logger;
        _context = context;
        _productImageService = productImageService;
    }

    public async Task CreateCategoryAsync(CategoryCreateViewModel model)
    {
        var categoryName = model.Name.Trim();
        var order = model.Order;

        var category = await _context.Categories
            .Where(item => item.Name == categoryName && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (category != null) throw new Exception("Category already exists");

        category = await _context.Categories
            .Where(item => item.Order == model.Order && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (category != null) throw new Exception("Category order already exists");

        category = new Category
        {
            Name = categoryName,
            Slug = SlugHelper.GenerateSlug(categoryName),
            Order = model.Order,
            IsDisplayed = model.IsDisplayed
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {categoryName} created successfully");
    }

    public async Task UpdateCategoryAsync(int id, CategoryUpdateViewModel model)
    {
        var categoryName = model.Name.Trim();
        var order = model.Order;

        var checkExist = await _context.Categories
            .Where(item =>
                item.Id != id &&
                item.Name == categoryName &&
                item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (checkExist != null) throw new Exception("Category name already exists");

        checkExist = await _context.Categories
            .Where(item =>
                item.Id != id &&
                item.IsDeleted == false &&
                item.Order == order)
            .FirstOrDefaultAsync();
        
        if (checkExist != null) throw new Exception("Order category already exists");

        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (category == null) throw new Exception("Category not found");

        category.Name = categoryName;
        category.Order = order;
        category.IsDisplayed = model.IsDisplayed;
        category.Slug = SlugHelper.GenerateSlug(categoryName);
        
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {categoryName} has been updated successfully");
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (category == null) throw new Exception("Category not found");
        
        category.IsDisplayed = false;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {category.Name} has been deleted successfully");
    }

    public async Task ForceDeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (category == null) throw new Exception("Category not found");
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {category.Name} has been deleted from database successfully");
    }

    public async Task<List<SelectListItem>> GetCategoriesWithSelectList()
    {
        var categories = await _context.Categories
            .Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString(),
            }).ToListAsync();

        return categories;
    }

    public List<CategoryProductViewModel> GetCategoriesWithProducts()
    {
       var categories = GetAllCategories()
           .Select(item => new CategoryProductViewModel
           {
               CategoryName = item.Name,
               Products = item.Products
                   .Where(product => product.IsDisplayed == true && product.IsDeleted == false)
                   .Select(product => new ProductItemViewModel
                   {
                       Name = product.Name,
                       Price = product.Price,
                       PromotionPrice = product.PromotionPrice,
                       Slug = product.Slug,
                       HasDiscount = product.HasDiscount,
                       ImagePath = _productImageService.GetImageMainProductById(product.Id)
                   }).ToList(),
           }).ToList();
       
           return categories;
    }   

    public IQueryable<Category> GetAllCategories()
    {
        var categories = _context.Categories
            .Include(item => item.Products)
            .Where(item => item.IsDeleted == false && item.IsDisplayed == true);

        return categories;
    }

}