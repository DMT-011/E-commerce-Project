using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Models.Services.CategoryService;

public interface ICategoryService
{
    // Command
    Task CreateCategoryAsync(CategoryCreateViewModel model);
    Task UpdateCategoryAsync(int id, CategoryUpdateViewModel model);
    Task DeleteCategoryAsync(int id);
    Task ForceDeleteCategoryAsync(int id);
    
    // Query
    Task<List<SelectListItem>> GetCategoriesWithSelectList();
    List<CategoryProductViewModel> GetCategoriesWithProducts();
    IQueryable<Category> GetAllCategories();
}