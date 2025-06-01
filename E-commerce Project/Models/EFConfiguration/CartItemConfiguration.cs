using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce_Project.Models.EFConfiguration;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,0)");
        
        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,0)");
    }
}