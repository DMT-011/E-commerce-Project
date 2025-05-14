using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce_Project.Models.ViewModels.ProductViewModel;

public class ProductCreateViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Detail { get; set; }
    public string Price { get; set; }
    public string? PromotionPrice { get; set; }
    public int Quantity { get; set; }
    public bool HasDiscount { get; set; }  
    public bool IsDisplayed { get; set; }
    
    public IFormFile ImageMain { get; set; }
    public IFormFile ImageSub1 { get; set; }
    public IFormFile ImageSub2 { get; set; }
    public IFormFile ImageSub3 { get; set; }
    
    public int CategoryId { get; set; }
    public List<SelectListItem> Categories { get; set; }
}