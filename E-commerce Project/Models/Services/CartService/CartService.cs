using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.ViewModels.CartViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.CartService;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CartService> _logger;
    private readonly IProductService _productService;

    public CartService(ApplicationDbContext context, ILogger<CartService> logger
    , IProductService productService)
    {
        _context = context;
        _logger = logger;
        _productService = productService;
    }
    
    public async Task CreateCartAsync(int userId)
    {
        var checkExistingCart = await _context.Carts
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();
        
        if(checkExistingCart != null) throw new ApplicationException("Already existing cart");

        var cart = new Cart { UserId = userId };
        
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
       _logger.LogInformation($"Cart {cart.Id}  created successfully");
    }

    public async Task<CartItemResultViewModel> AddProductToCartAsync([FromBody] CartItemViewModel model, int cartId)
    {
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        
        var CartItemExist = await _context.CartItems
            .Where(item =>
                item.CartId == cartId &&
                item.ProductId == model.ProductId &&
                item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        var price = product.HasDiscount ? product.PromotionPrice ?? 0 : product.Price;

        if (CartItemExist != null)
        {
            var quantityNew = CartItemExist.Quantity + model.Quantity;
            CartItemExist.Quantity = quantityNew;
            CartItemExist.TotalPrice = quantityNew * price;

            _context.CartItems.Update(CartItemExist);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Cart item has been update successfully");

            return new CartItemResultViewModel { Status = "updated" };
        }
        
        var cartItem = new CartItem
        {
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            Price = price,
            TotalPrice = price * model.Quantity,
            CartId = cartId,
        };
    
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Cart item added to cart");
        return new CartItemResultViewModel { Status = "added" };
    }

    public async Task<List<CartItem>> GetAllCartItemsAsync(int cartId)
    {
        var cartItems = await _context.CartItems
            .Include(item => item.Product)
            .Where(item => item.CartId == cartId && item.IsDeleted == false)
           .ToListAsync();
        
        _logger.LogInformation("Get all cart item succesfully");
        return cartItems;
    }

    public async Task<decimal> GetTotalPriceCartAsync(int cartId)
    {
        var totalPriceCart = await _context.CartItems
            .Where(item => item.CartId == cartId && item.IsDeleted == false)
            .SumAsync(item => item.TotalPrice);
        
        _logger.LogInformation("Get total price succesfully");
        return totalPriceCart;
    }
}