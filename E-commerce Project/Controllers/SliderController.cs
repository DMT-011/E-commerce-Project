using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Models.ViewModels.SliderViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Controllers;

public class SliderController : Controller
{
    private readonly ISliderService _sliderService;

    public SliderController(ISliderService sliderService)
    {
        _sliderService = sliderService;
    }
        
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        ViewBag.countSlider = _sliderService.GetAllSlides().Count;
        var listPriority = _sliderService.GetAllSlides()
            .Select(item => new SelectListItem
            {
                Text = item.Priority.ToString(),
                Value = item.Priority.ToString(),
            }).ToList();

        var model = new SliderCreateViewModel
        {
            Priorities = listPriority
        };
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateViewModel model)
    {
        await _sliderService.CreateSliderAsync(model);
        TempData["title"] = $"Thêm thành công";
        TempData["message"] = $"Đã thêm thành công slider {model.Name}.";
        TempData["icon"] = "fas fa-check";
        TempData["type"] = "success";
        return RedirectToAction("Slider", "Admin");
    }

    public async Task<IActionResult> Update(int id)
    {
        ViewBag.countSlider = _sliderService.GetAllSlides().Count;
        var listPriority = _sliderService.GetAllSlides()
            .Select(item => new SelectListItem
            {
                Text = item.Priority.ToString(),
                Value = item.Priority.ToString(),
            }).ToList();
        
        var slider = await _sliderService.GetSliderByIdAsync(id);
        var model = new SliderUpdateViewModel
        {
            Id = slider.Id,
            Name = slider.Name,
            IsDisplayed = slider.IsDisplayed,
            ImagePath = slider.ImagePath,
            Priority = slider.Priority ?? 0,
            Priorities = listPriority,
        };
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, SliderUpdateViewModel model)
    {
        await _sliderService.UpdateSliderAsync(id, model);
        TempData["title"] = $"Cập nhập thành công";
        TempData["message"] = $"Thông tin slider có ID = {id} đã được cập nhật.";
        TempData["icon"] = "fas fa-edit";
        TempData["type"] = "info";
        return RedirectToAction("Slider", "Admin");
    }

    public IActionResult Trash(int? page)
    {
        var sliders = _sliderService.GetSlidersWithPaginationAdminTrash(page);
        return View(sliders);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _sliderService.DeleteSliderAsync(id);
        TempData["title"] = $"Đã chuyển vào thùng rác";
        TempData["message"] = $"Slider có ID = {id} đã được đưa vào thùng rác.";
        TempData["icon"] = "fas fa-trash";
        TempData["type"] = "warning";
        return RedirectToAction("Slider", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _sliderService.RestoreSliderAsync(id);
        TempData["title"] = $"Khôi phục thành công";
        TempData["message"] = $"Slider có ID = {id} đã được khôi phục.";
        TempData["icon"] = "fas fa-sync-alt";
        TempData["type"] = "success";
        return RedirectToAction("Trash", "Slider");
    }
    
    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _sliderService.ForceDeleteSliderAsync(id);
        TempData["title"] = "Đã xóa vĩnh viễn";
        TempData["message"] = $"Slider có ID = {id} đã bị xóa vĩnh viễn khỏi hệ thống.";
        TempData["icon"] = "fas fa-times";
        TempData["type"] = "danger";
        return RedirectToAction("Trash", "Slider");
    }
    
}