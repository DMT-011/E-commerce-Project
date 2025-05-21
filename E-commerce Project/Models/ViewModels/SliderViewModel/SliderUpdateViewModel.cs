using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Models.ViewModels.SliderViewModel;

public class SliderUpdateViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IFormFile? ImageSlide { get; set; }
    public string ImagePath { get; set; }
    public bool IsDisplayed { get; set; }
    
    public int Priority { get; set; }
    public List<SelectListItem> Priorities { get; set; }
}