using System.Reflection;
using E_commerce_Project.Models.Common;
using E_commerce_Project.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;

namespace E_commerce_Project.Models.Context;

public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Slide> Slides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now.ToUniversalTime();
                entry.Entity.UpdatedAt = DateTime.Now.ToUniversalTime();
                entry.Entity.CreatedBy = userId;
                entry.Entity.IsDeleted = false;

            } else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now.ToUniversalTime();
                entry.Entity.UpdatedBy = userId;
            } 
        }
        
        return await base.SaveChangesAsync(cancellationToken); 
    }

    private int? GetCurrentUserId()
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return int.TryParse(userIdStr, out int userId) ? userId : null;
    }
}