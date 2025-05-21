using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.ViewModels.SliderViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Views.ViewComponents;

public class SliderListViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public SliderListViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var sliders = await _context.Slides
            .Where(item => item.IsDisplayed == true && item.IsDeleted == false)
            .Select(item => new SliderListViewModel
            {
                ImagePath = item.ImagePath
            }).ToListAsync();

        return View(sliders);
    }
}