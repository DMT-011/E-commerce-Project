using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Controllers;

[Authorize(AuthenticationSchemes = "CookieAuthAdmin")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
 
    public async Task<IActionResult> Detail(int id)
    {
        var model = await _orderService.GetOrderAdminDetailAsync(id);
        return View(model);
    }

    public IActionResult Trash(int? page)
    {
        var model = _orderService.GetOrdersTrashWithPaginationAdmin(page);
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(int id, AdminOrderListViewModel model)
    {
        await _orderService.UpdateOrderAsync(id, model);
        TempData["title"] = $"Cập nhập thành công";
        TempData["message"] = $"Thông tin của đơn hàng có ID = {id} đã được cập nhật.";
        TempData["icon"] = "fas fa-edit";
        TempData["type"] = "info";
        return RedirectToAction("Order", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Restore(int id)
    {
        await _orderService.RestoreOrderAsync(id);
        TempData["title"] = $"Khôi phục thành công";
        TempData["message"] = $"Đơn hàng có ID = {id} đã được khôi phục.";
        TempData["icon"] = "fas fa-sync-alt";
        TempData["type"] = "success";
        return RedirectToAction("Trash", "Order");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        TempData["title"] = $"Đã chuyển vào thùng rác";
        TempData["message"] = $"Đơn hàng có ID = {id} đã được đưa vào thùng rác.";
        TempData["icon"] = "fas fa-trash";
        TempData["type"] = "warning";
        return RedirectToAction("Order", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> ForceDelete(int id)
    {
        await _orderService.ForceDeleteOrderAsync(id);
        TempData["title"] = "Đã xóa vĩnh viễn";
        TempData["message"] = $"Đơn hàng có ID = {id} đã bị xóa vĩnh viễn khỏi hệ thống.";
        TempData["icon"] = "fas fa-times";
        TempData["type"] = "danger";
        return RedirectToAction("Trash", "Order");
    }
}   