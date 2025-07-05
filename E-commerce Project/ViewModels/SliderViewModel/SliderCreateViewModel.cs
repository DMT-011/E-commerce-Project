using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Models.ViewModels.SliderViewModel;

public class SliderCreateViewModel
{
    public string Name { get; set; }
    public IFormFile ImageSlide { get; set; }
    public int? Priority { get; set; }
    public bool IsDisplayed { get; set; }
    
    public List<SelectListItem> Priorities { get; set; }
}