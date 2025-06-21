using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Models.ViewModels.CategoryViewModel;

public class CategoryCreateViewModel
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public int? Order { get; set; }
    public bool IsDisplayed { get; set; }
    
    public List<SelectListItem> Orders { get; set; }
}