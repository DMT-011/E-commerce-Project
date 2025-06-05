using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce_Project.Models.EFConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,0)");
        
        builder.Property(x => x.PromotionPrice)
            .HasColumnType("decimal(18,0)");
        
        builder.Property(x => x.Slug)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany<ProductImage>(x => x.ProductImages)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<CartItem>(x => x.CartItems)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<OrderDetail>(x => x.OrderDetails)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.Restrict);
    }
}