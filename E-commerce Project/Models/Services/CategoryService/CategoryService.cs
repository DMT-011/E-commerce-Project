using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ApplicationDbContext _context;

    public CategoryService(ILogger<CategoryService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
}