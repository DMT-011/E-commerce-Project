
using E_commerce_Project.Models.Common;
using Microsoft.EntityFrameworkCore.Internal;

namespace E_commerce_Project.Models.Entities;

public class Product : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Detail { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? SoldQuantity { get; set; }
    public bool HasDiscount { get; set; } 
    public bool IsDisplayed { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; }
    
    public ICollection<OrderDetail> OrderDetails { get; set; }
}