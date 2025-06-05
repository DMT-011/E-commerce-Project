using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.UserService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.OrderViewModel;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_commerce_Project.Models.Services.OrderServive;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly ICartService _cartService;
    private readonly IProductImageService _productImageService;
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    public OrderService(ApplicationDbContext context, ILogger<OrderService> logger
        , ICartService cartService, IProductImageService productImageService
        , IProductService productService, IUserService userService)
    {
        _context = context;
        _logger = logger;
        _cartService = cartService;
        _productImageService = productImageService;
        _productService = productService;
        _userService = userService;
    }

    public async Task<bool> CreateOrderAsync(int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var cart = await _context.Carts
                .Include(item => item.CartItems)
                .Where(item => item.UserId == userId && item.IsDeleted == false)
                .FirstOrDefaultAsync();

            var order = new Order
            {
                UserId = userId,
                TotalAmount = await _cartService.GetTotalPriceCartAsync(cart.Id),
                OrderStatus = (int)OrderStatusType.Pending,
                IsLocked = false,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order {order.Id} has been created");

            var orderDetails = cart.CartItems
                .Where(item => item.IsDeleted == false)
                .Select(item => new OrderDetail
                {
                    OderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    UnitPrice = item.Price,
                }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // Clear cart when success payment 
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task UpdateOrderAsync(int id, AdminOrderListViewModel model)
    {
        var order = await _context.Orders
            .Include(item => item.OrderDetails)
            .ThenInclude(item => item.Product)
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (order == null) throw new Exception("Order not found");
        if (order.IsLocked) throw new Exception("Order is locked");

        var isProcessed = (int) model.OrderStatus == 1;
        if (isProcessed)
        {
            var orderDetails = order.OrderDetails.ToList();

            // Update sold quantity and stock of product
            foreach (var orderDetail in orderDetails)
            {
                var productOrder = orderDetail.Product;
                productOrder.SoldQuantity += orderDetail.Quantity;
                productOrder.StockQuantity -= orderDetail.Quantity;
            }
            
            order.ShippingDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        var isPending = (int) model.OrderStatus == 0;
        
        if (!isPending) order.IsLocked = true;
        order.OrderStatus = (int) model.OrderStatus;
        order.OrderNote = model.OrderNote;
        
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Order has been updated");
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (order == null) throw new Exception("Order not found");
        
        order.IsDeleted = true;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Order has been deleted");
    }

    public async Task ForceDeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (order == null) throw new Exception("Order not found!");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Order {id} has been deleted from database");
    }

    public async Task RestoreOrderAsync(int id)
    {
        var order = await _context.Orders
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();
        
        if(order == null) throw new Exception("Order not found!");
        
        order.IsDeleted = false;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Order {id} has been restored");
    }

    public List<OrderDetailViewModel> GetOrderDetails(int userId)
    {
        var order = _context.Orders
            .Include(item => item.OrderDetails)
            .Include(item => item.User)
            .Where(item => item.UserId == userId && item.IsDeleted == false)
            .OrderByDescending(item => item.UpdatedAt)
            .Select(item => new OrderDetailViewModel
            {
                OrderStatus = (OrderStatusType)item.OrderStatus,
                TotalOrderPrice = item.TotalAmount,
                OrderDate = item.CreatedAt,
                OrderUserItem = new OrderUserItemViewModel
                {
                    Name = item.User.FullName,
                    Phone = item.User.Phone,
                    Email = item.User.Email,
                    Address = item.User.Address,
                },
                OrderItems = item.OrderDetails
                    .Where(item => item.IsDeleted == false)
                    .OrderByDescending(item => item.UpdatedAt)
                    .Select(orderItem => new OrderItemViewModel
                    {
                        Name = _productService.GetProductById(orderItem.ProductId).Name,
                        QuantityOrder = orderItem.Quantity,
                        ImagePath = _productImageService.GetImageMainProductById(orderItem.ProductId),
                        Price = orderItem.UnitPrice,
                        Slug = _productService.GetProductById(orderItem.ProductId).Slug
                    }).ToList(),
            }).ToList();

        return order;
    }

    public IPagedList<AdminOrderListViewModel> GetOrdersWithPaginationAdmin(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var orders = _context.Orders
            .Include(item => item.User)
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == false)
            .Select(item => new AdminOrderListViewModel
            {
               Id = item.Id,
               FullName = item.User.FullName,
               AdminName = _userService.GetUserByIdAsync(item.UserId).FullName,
               TotalOrder = item.TotalAmount,
               OrderNote = item.OrderNote ?? "",
               OrderStatus = (OrderStatusType) item.OrderStatus,
               OrderDateCreate = item.CreatedAt,
               OrderDateUpdate = item.ShippingDate,
            })
            .ToPagedList(pageNumber, pageSize);

        return orders;
    }

    public IPagedList<AdminOrderTrashViewModel> GetOrdersTrashWithPaginationAdmin(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var orders = _context.Orders
            .Include(item => item.User)
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == true)
            .Select(item => new AdminOrderTrashViewModel
            {
                Id = item.Id,
                FullName = item.User.FullName,
                TotalPriceOrder = item.TotalAmount,
                OrderNote = item.OrderNote ?? "",
                OrderCreateDate = item.CreatedAt,
            })
            .ToPagedList(pageNumber, pageSize);

        return orders;
    }

    public async Task<AdminOrderDetailViewModel> GetOrderAdminDetailAsync(int id)
    {
        var orders = await _context.Orders
            .Include(item => item.OrderDetails)
            .Include(item => item.User)
            .Where(item => item.Id == id && item.IsDeleted == false)
            .Select(item => new AdminOrderDetailViewModel
            {
                TotalOrderPrice = item.TotalAmount,
                OrderUserItem = new OrderUserItemViewModel
                {
                    Name = item.User.FullName,
                    Phone = item.User.Phone,
                    Email = item.User.Email,
                    Address = item.User.Address,
                },
                OrderItems = item.OrderDetails
                    .Where(orderItem => orderItem.IsDeleted == false)
                    .Select(orderItem => new OrderItemViewModel
                    {
                        Name = _productService.GetProductById(orderItem.ProductId).Name,
                        QuantityOrder = orderItem.Quantity,
                        TotalOrderItemPrice = orderItem.UnitPrice * orderItem.Quantity,
                        ImagePath = _productImageService.GetImageMainProductById(orderItem.ProductId),
                        Price = orderItem.UnitPrice,
                        Slug = _productService.GetProductById(orderItem.ProductId).Slug,
                    }).ToList(),
            }).FirstOrDefaultAsync();

        return orders;
    }
}