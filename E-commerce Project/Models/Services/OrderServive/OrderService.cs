using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.ViewModels.OrderViewModel;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.OrderServive;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly ICartService _cartService;
    private readonly IProductImageService _productImageService;
    private readonly IProductService _productService;

    public OrderService(ApplicationDbContext context, ILogger<OrderService> logger
        , ICartService cartService, IProductImageService productImageService
        , IProductService productService)
    {
        _context = context;
        _logger = logger;
        _cartService = cartService;
        _productImageService = productImageService;
        _productService = productService;
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
}