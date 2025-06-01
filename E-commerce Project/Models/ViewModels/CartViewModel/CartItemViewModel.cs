namespace E_commerce_Project.Models.ViewModels.CartViewModel;

public class CartItemViewModel
{
    public int Quantity { get; set; }
    public decimal Price { get; set; } 
    public decimal TotalPrice { get; set; }
    
    public int CartId { get; set; }
    public int ProductId { get; set; }
}