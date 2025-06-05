using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.OrderViewModel;
using X.PagedList;

namespace E_commerce_Project.Models.Services.OrderServive;

public interface IOrderService 
{
    // Command
    Task<bool> CreateOrderAsync(int userId);
    Task UpdateOrderAsync(int id, AdminOrderListViewModel model);
    Task DeleteOrderAsync(int id);
    Task ForceDeleteOrderAsync(int id);
    Task RestoreOrderAsync(int id);
    
    // Query
    List<OrderDetailViewModel> GetOrderDetails(int userId);
    IPagedList<AdminOrderListViewModel> GetOrdersWithPaginationAdmin(int? page);
    IPagedList<AdminOrderTrashViewModel> GetOrdersTrashWithPaginationAdmin(int? page);
    
    Task<AdminOrderDetailViewModel> GetOrderAdminDetailAsync(int id);
}