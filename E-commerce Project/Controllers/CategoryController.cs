using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.ViewModels.CategoryViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace E_commerce_Project.Controllers;

[Authorize(AuthenticationSchemes = "CookieAuthAdmin")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    public IActionResult Index()
    {
        return View(); 
    }
    
    public IActionResult Create()
    {
        ViewBag.countCategory = _categoryService.GetAllCategories().Count;
        var orders = _categoryService.GetAllCategories()
            .Select(item => new SelectListItem
            {
                Text = item.Order.ToString(),
                Value = item.Order.ToString()
            }).ToList();
        var model = new CategoryCreateViewModel
        {
           Orders  = orders
        };
        return View(model);
    }

    public IActionResult Update(int id)
    {
        var orders = _categoryService.GetAllCategories()
            .Select(item => new SelectListItem
            {
                Value = item.Order.ToString(),
                Text = item.Order.ToString()
            }).ToList();
        
        var category = _categoryService.GetCategoryById(id);
        var model = new CategoryUpdateViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsDisplayed = category.IsDisplayed,
            Order = category.Order,
            Orders = orders,
        };

        return View(model);
    }

    public IActionResult Trash(int? page)
    {
        var categories = 
            _categoryService.GetCategoriesWithPaginationAdminTrash(page);
        
        return View(categories);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        await _categoryService.CreateCategoryAsync(model);
        TempData["title"] = $"Thêm thành công";
        TempData["message"] = $"Đã thêm thành công danh mục {model.Name}.";
        TempData["icon"] = "fas fa-check";
        TempData["type"] = "success";
        return RedirectToAction("Category", "Admin");
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(int id, CategoryUpdateViewModel model)
    {
        await _categoryService.UpdateCategoryAsync(id , model);
        TempData["title"] = $"Cập nhập thành công";
        TempData["message"] = $"Thông tin  danh mục có ID = {model.Id} đã được cập nhật.";
        TempData["icon"] = "fas fa-edit";
        TempData["type"] = "info";
        return RedirectToAction("Category", "Admin");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        TempData["title"] = $"Đã chuyển vào thùng rác";
        TempData["message"] = $"Danh mục có ID = {id} đã được đưa vào thùng rác.";
        TempData["icon"] = "fas fa-trash";
        TempData["type"] = "warning";
        return RedirectToAction("Category", "Admin");
    }
    
    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _categoryService.RestoreCategoryAsync(id);
        TempData["title"] = $"Khôi phục thành công";
        TempData["message"] = $"Danh mục có ID = {id} đã được khôi phục.";
        TempData["icon"] = "fas fa-sync-alt";
        TempData["type"] = "success";
        return RedirectToAction("Trash", "Category");
    }
    
    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _categoryService.ForceDeleteCategoryAsync(id);
        TempData["title"] = "Đã xóa vĩnh viễn";
        TempData["message"] = $"Danh mục có ID = {id} đã bị xóa vĩnh viễn khỏi hệ thống.";
        TempData["icon"] = "fas fa-times";
        TempData["type"] = "danger";
        return RedirectToAction("Trash", "Category");
    }
}