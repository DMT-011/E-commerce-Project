namespace E_commerce_Project.Models.ViewModels.ProductViewModel;

public class ProductTrashViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public int Quantity { get; set; }
    public string CategoryName { get; set; }
    public string ImagePath { get; set; }
    public DateTime CreatedDate { get; set; }
}