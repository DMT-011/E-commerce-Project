using E_commerce_Project.Models.Enums;

namespace E_commerce_Project.Models.ViewModels.OrderViewModel;

public class OrderDetailViewModel
{
    public OrderStatusType OrderStatus { get; set; }
    public decimal TotalOrderPrice { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; }
    
    public OrderUserItemViewModel OrderUserItem { get; set; }
    
    public DateTime OrderDate { get; set; }
}