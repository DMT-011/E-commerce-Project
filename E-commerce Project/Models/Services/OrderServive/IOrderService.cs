using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.OrderViewModel;

namespace E_commerce_Project.Models.Services.OrderServive;

public interface IOrderService 
{
    // Command
    Task<bool> CreateOrderAsync(int userId);
    
    // Query
    List<OrderDetailViewModel> GetOrderDetails(int userId);
}