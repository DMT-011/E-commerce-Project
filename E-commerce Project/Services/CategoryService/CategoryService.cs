using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

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

        var category = await _context.Categories
            .Where(item => item.Name == categoryName && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (category != null) throw new Exception("Category already exists");

        var orderMax = _context.Categories
            .Where(item => item.IsDeleted == false)
            .Max(item => item.Order)
            .GetValueOrDefault();

        category = new Category
        {
            Name = categoryName,
            Slug = SlugHelper.GenerateSlug(categoryName),
            IsDisplayed = model.IsDisplayed
        };

        // Set priority = 1 when first time init slider
        if (orderMax == 0 && model.Order == 0)
        {
            orderMax = 1;
            category.Order = orderMax;
        }

        var categoryOrderOld = await _context.Categories
            .Where(item => item.Order == model.Order && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        // Change position category old and category new create when select priority client
        if (categoryOrderOld != null)
        {
            categoryOrderOld.Order = ++orderMax;
            category.Order = model.Order;
            _context.Categories.Update(categoryOrderOld);
            await _context.SaveChangesAsync();
        }
        else
        {
            category.Order = ++orderMax;
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {categoryName} created successfully");
    }

    public async Task UpdateCategoryAsync(int id, CategoryUpdateViewModel model)
    {
        var categoryName = model.Name.Trim();
        var checkExist = await _context.Categories
            .Where(item =>
                item.Id != id &&
                item.Name == categoryName &&
                item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (checkExist != null) throw new Exception("Category name already exists");

        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (category == null) throw new Exception("Category not found");

        var orderOld = category.Order;

        category.Name = categoryName;
        category.Order = model.Order;
        category.IsDisplayed = model.IsDisplayed;
        category.Slug = SlugHelper.GenerateSlug(categoryName);

        var categoryOrderOld = await _context.Categories
            .Where(item => item.Order == model.Order && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (categoryOrderOld != null)
        {
            categoryOrderOld.Order = orderOld;
            _context.Categories.Update(categoryOrderOld);
            await _context.SaveChangesAsync();
        }

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

        category.IsDeleted = true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {category.Name} has been deleted successfully");
    }

    public async Task ForceDeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (category == null) throw new Exception("Category not found");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {category.Name} has been deleted from database successfully");
    }

    public async Task RestoreCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (category == null) throw new Exception("Category not found!");
        
        var checkExistOrder = await _context.Categories
            .Where(item => item.Order == category.Order && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (checkExistOrder != null)
        {
            var orderMax = await _context.Categories
                .Where(item => item.IsDeleted == false)
                .MaxAsync(item => item.Order);

            category.Order = ++orderMax;
        }

        category.IsDeleted = false;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Category {category.Name} has been restore successfully");
    }

    public Category GetCategoryById(int id)
    {
        var category = _context.Categories
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefault();
        if (category == null) throw new Exception("Category not found");
        return category;
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
            .OrderBy(item => item.Order)
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

    public List<Category> GetAllCategories()
    {
        var categories = _context.Categories
            .Include(item => item.Products)
            .Where(item => item.IsDeleted == false)
            .ToList();
        return categories;
    }

    public IPagedList<AdminCategoryListViewModel> GetCategoriesWithPaginationAdmin(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var categories = _context.Categories
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == false)
            .Select(item => new AdminCategoryListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug,
                Order = item.Order ?? 0,
                CreatedDate = item.CreatedAt,
            })
            .ToPagedList(pageNumber, pageSize);

        return categories;
    }

    public IPagedList<AdminCategoryTrashViewModel> GetCategoriesWithPaginationAdminTrash(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var categories = _context.Categories
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == true)
            .Select(item => new AdminCategoryTrashViewModel
            {
                Id = item.Id,
                Name = item.Name,
                DateCreated = item.CreatedAt
            })
            .ToPagedList(pageNumber, pageSize);

        return categories;
    }
}