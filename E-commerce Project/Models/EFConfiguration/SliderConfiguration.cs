using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace E_commerce_Project.Models.EFConfiguration;

public class SliderConfiguration : IEntityTypeConfiguration<Slide>
{
    public void Configure(EntityTypeBuilder<Slide> builder)
    {
        builder.Property(item => item.ImagePath)
            .IsRequired(false);
        
        builder.Property(item => item.Priority)
            .IsRequired(false);
    }
}