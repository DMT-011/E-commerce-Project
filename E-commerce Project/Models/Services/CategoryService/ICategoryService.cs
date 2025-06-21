using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace E_commerce_Project.Models.Services.CategoryService;

public interface ICategoryService
{
    // Command
    Task CreateCategoryAsync(CategoryCreateViewModel model);
    Task UpdateCategoryAsync(int id, CategoryUpdateViewModel model);
    Task DeleteCategoryAsync(int id);
    Task ForceDeleteCategoryAsync(int id);
    Task RestoreCategoryAsync(int id);
    
    // Query
    Category GetCategoryById(int id);
    Task<List<SelectListItem>> GetCategoriesWithSelectList();
    List<CategoryProductViewModel> GetCategoriesWithProducts();
    List<Category> GetAllCategories();
    IPagedList<AdminCategoryListViewModel> GetCategoriesWithPaginationAdmin(int? page);
    IPagedList<AdminCategoryTrashViewModel> GetCategoriesWithPaginationAdminTrash(int? page);
}