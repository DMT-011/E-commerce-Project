using E_commerce_Project.Models.Enums;

namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminOrderListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    
    public string AdminName { get; set; }
    public OrderStatusType OrderStatus { get; set; }
    public decimal TotalOrder { get; set; }
    public string OrderNote { get; set; }
    
    public DateTime OrderDateCreate { get; set; }
    public DateTime? OrderDateUpdate { get; set; }
}