using E_commerce_Project.Models.Common;

namespace E_commerce_Project.Models.Entities;

public class ProductImage : AuditableEntity
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public bool IsPrimary { get; set; }
    
    public int Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}