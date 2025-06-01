namespace E_commerce_Project.Models.ViewModels.CartViewModel;

public class CartItemListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int QuantityOrder { get; set; }
    public bool HasDiscount { get; set; }
    public string Price { get; set; }
    public string? PromotionPrice { get; set; }
    public string ImagePath { get; set; }
}