namespace E_commerce_Project.Models.ViewModels.ProductViewModel;

public class ProductItemViewModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public bool HasDiscount { get; set; }
    public string ImagePath { get; set; }
    public string Slug { get; set; }
}