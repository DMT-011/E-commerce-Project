using E_commerce_Project.Models.ViewModels.OrderViewModel;

namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminOrderDetailViewModel
{
    
    public decimal TotalOrderPrice { get; set; }
    public DateTime ShippingDate { get; set; }
    public string OrderNote { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; }
    public OrderUserItemViewModel OrderUserItem { get; set; }
}