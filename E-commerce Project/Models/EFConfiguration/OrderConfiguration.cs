using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce_Project.Models.EFConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasMany<OrderDetail>(x => x.OrderDetails)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,0)");
    }
}