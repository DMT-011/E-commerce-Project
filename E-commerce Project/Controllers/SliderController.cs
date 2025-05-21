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
        return RedirectToAction("Slider", "Admin");
    }

    public IActionResult Trash()
    {
        var sliders = _sliderService.GetAllSlidesDeleted()
            .Select(item => new SliderTrashViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ImagePath = item.ImagePath,
            });
        return View(sliders);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _sliderService.DeleteSliderAsync(id);
        return RedirectToAction("Slider", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _sliderService.RestoreSliderAsync(id);
        return RedirectToAction("Slider", "Admin");
    }
    
    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _sliderService.ForceDeleteSliderAsync(id);
        return RedirectToAction("Trash");
    }
    
}