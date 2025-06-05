using E_commerce_Project.Models.Common;

namespace E_commerce_Project.Models.Entities;

public class Cart : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}
