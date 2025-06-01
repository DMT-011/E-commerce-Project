using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.ViewModels.CartViewModel;
using E_commerce_Project.Views.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductImageService _productImageService;
    private readonly IOrderService _orderService;

    public CartController(ICartService cartService, IProductImageService productImageService,
        IOrderService orderService)
    {
        _cartService = cartService;
        _productImageService = productImageService;
        _orderService = orderService;
    }

    public async Task<JsonResult> GetAll(int cartId)
    {
        var cartItems = await _cartService.GetAllCartItemsAsync(cartId);
        var productItems = cartItems.Select(item => new CartItemListViewModel
        {
            Id = item.Product.Id,
            Name = item.Product.Name,
            ImagePath = _productImageService.GetImageMainProductById(item.Product.Id),
            QuantityOrder = item.Quantity,
            Price = CurrencyFormatterHelper.Format(item.Price),
            PromotionPrice = CurrencyFormatterHelper.Format(item.Product.PromotionPrice ?? 0),
            Slug = item.Product.Slug,
            HasDiscount = item.Product.HasDiscount,
        });
        return Json(productItems);
    }
    
    public async Task<JsonResult> GetTotalPrice(int cartId)
    {
        var respone =  await _cartService.GetTotalPriceCartAsync(cartId);
        return Json(new
        {
            totalPrice = CurrencyFormatterHelper.Format(respone),
            status = "success"
        });
    }

    [HttpPost]
    public async Task<IActionResult> Payment()
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var isSuccess = await _orderService.CreateOrderAsync(userId);
        
        TempData["title"] = "Thanh toán thành công"; 
        TempData["message"] = " ";
        TempData["textBtn"] = "OK";
        TempData["type"] = "success";
        return RedirectToAction("Order", "User", new { userId = userId });
    }
}