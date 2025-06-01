namespace E_commerce_Project.Models.ViewModels.OrderViewModel;

public class OrderItemViewModel
{
    public string Name {get; set;}
    public int QuantityOrder {get; set;}
    public decimal Price {get; set;}
    public string ImagePath {get; set;}
    public string Slug {get; set;}
}