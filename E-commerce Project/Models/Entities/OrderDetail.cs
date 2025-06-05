using E_commerce_Project.Models.Common;

namespace E_commerce_Project.Models.Entities;

public class OrderDetail : AuditableEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; } // not has key
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalOrder { get; set; }
    
    public int OderId { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
}