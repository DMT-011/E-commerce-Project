using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce_Project.Models.EFConfiguration;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,0)");
        
        builder.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,0)");
        
        builder.Property(x => x.TotalOrder)
            .HasColumnType("decimal(18,0)");
    }
}