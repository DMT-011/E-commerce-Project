namespace E_commerce_Project.Models.ViewModels.ProductViewModel;

public class ProductNewListViewModel
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    
    public bool HasDiscount { get; set; }
    public string ImagePath { get; set; }
}