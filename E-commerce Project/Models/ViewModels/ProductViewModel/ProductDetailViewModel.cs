namespace E_commerce_Project.Models.ViewModels.ProductViewModel;

public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public bool HasDiscount { get; set; }
    public string Details { get; set; }
    public string Description { get; set; }
    public string ImageMainPath { get; set; }
    public string ImageSub1Path { get; set; }
    public string ImageSub2Path { get; set; }
    public string ImageSub3Path { get; set; }
}