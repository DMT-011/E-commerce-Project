using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.ViewModels.OrderViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Views.ViewComponents;

public class PaymentOrderViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;
    
    public PaymentOrderViewComponent(ApplicationDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(int userId)
    {
        
        var user = await _context.Users
            .Include(item => item.Cart)
            .Where(item => item.Id == userId && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        var totalOrder = await _cartService.GetTotalPriceCartAsync(user.Cart.Id);
        
        var model = new OrderPaymentViewModel
        {
            Name = user.FullName,
            Email = user.Email,
            Address = user.Address,
            Phone = user.Phone,
            TotalOrder = totalOrder
        };
        
        return View(model);
    }
}