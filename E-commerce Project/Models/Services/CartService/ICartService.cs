
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.CartViewModel;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Models.Services.CartService;

public interface ICartService
{
    // Command
    Task CreateCartAsync(int userId);
    Task<CartItemResultViewModel> AddProductToCartAsync([FromBody] CartItemViewModel model, int cartId);
    
    // Query
    Task<List<CartItem>> GetAllCartItemsAsync(int cartId);
    Task<decimal> GetTotalPriceCartAsync(int cartId);
}