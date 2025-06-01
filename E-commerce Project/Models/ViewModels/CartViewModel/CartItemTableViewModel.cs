namespace E_commerce_Project.Models.ViewModels.CartViewModel;

public class CartItemTableViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public int QuantityOrder { get; set; }
    public decimal TotalPrice { get; set; }
    public string ImagePath { get; set; }
}