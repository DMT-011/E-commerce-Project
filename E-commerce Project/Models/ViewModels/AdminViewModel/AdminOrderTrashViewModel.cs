namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminOrderTrashViewModel
{
    public int Id { get; set; } 
    public string FullName { get; set; }
    public decimal TotalPriceOrder { get; set; }
    
    public string OrderNote { get; set; }
    public DateTime OrderCreateDate { get; set; }
}