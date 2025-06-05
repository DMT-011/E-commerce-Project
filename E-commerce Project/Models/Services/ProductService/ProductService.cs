using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_commerce_Project.Models.Services.ProductService;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;
    private readonly IProductImageService _productImageService;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger
        , IProductImageService productImageService)
    {
        _context = context;
        _logger = logger;
        _productImageService = productImageService;
    }

    public async Task CreateProductAsync(ProductCreateViewModel model)
    {
        var productName = model.Name.Trim();
        var product = await _context.Products
            .Where(item => item.Name == productName)
            .FirstOrDefaultAsync();

        if (product != null) throw new Exception("Product name already exists");

        product = new Product
        {
            Name = productName,
            Description = model.Description,
            Detail = model.Detail,
            Slug = SlugHelper.GenerateSlug(productName),
            Price = CurrencyFormatterHelper.RawValue(model.Price),
            PromotionPrice = CurrencyFormatterHelper.RawValue(model.PromotionPrice),
            StockQuantity = model.Quantity,
            CategoryId = model.CategoryId,
            IsDisplayed = model.IsDisplayed,
            HasDiscount = model.HasDiscount,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await _productImageService.CreateImagesProductAsync(product.Id, model);
        _logger.LogInformation($"Product: {productName} has created successfully");
    }

    public async Task UpdateProductAsync(int id, ProductUpdateViewModel model)
    {
        var productName = model.Name.Trim();
        var productExistName = await _context.Products
            .Where(item =>
                item.Id != id &&
                item.Name == productName)
            .FirstOrDefaultAsync();

        if (productExistName != null) throw new Exception("Product name exists");

        var product = await _context.Products
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (product == null)  throw new Exception("Product not found");

        product.Name = productName;
        product.Description = model.Description;
        product.Detail = model.Detail;
        product.Slug = SlugHelper.GenerateSlug(productName);
        product.Price = CurrencyFormatterHelper.RawValue(model.Price);
        product.PromotionPrice = CurrencyFormatterHelper.RawValue(model.PromotionPrice);
        product.StockQuantity = model.Quantity;
        product.IsDisplayed = model.IsDisplayed;
        product.HasDiscount = model.HasDiscount;
        product.CategoryId = model.CategoryId;

        await _productImageService.UpdateImagesProductAsync(product.Id, model);
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Product: {productName} has update successfully");
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new Exception("Product not found");
        }

        product.IsDeleted = true;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Product: {product} has sort deleted successfully");
    }

    public async Task ForceDeleteProductAsync(int id)
    {
        var product = await _context.Products
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new Exception("Product not found");
        }

        await _productImageService.DeleteImagesProductAsync(product.Id);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Product: {product} has deleted from database.");
    }

    public async Task RestoreProductAsync(int id)
    {
        var product = await _context.Products
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (product == null) throw new Exception("Product restore not found!");

        product.IsDeleted = false;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (product == null) throw new Exception("Product not found");
        return product;
    }

    public Product GetProductById(int id)
    {
        var product = _context.Products
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefault();
        if (product == null) throw new Exception("Product not found");
        return product;
    }

    public async Task<Product> GetProductBySlugAsync(string slug)
    {
        var product = await _context.Products
            .Where(item =>
                item.Slug == slug &&
                item.IsDisplayed == true &&
                item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (product == null) throw new Exception("Product detail not found!");
        return product;
    }

    public IQueryable<Product> GetAllProducts()
    {
        var products = _context.Products
            .Where(item => item.IsDisplayed == false && item.IsDeleted == false);
        return products;
    }

    public IPagedList<AdminProductListViewModel> GetProductsWithPaginationAdmin(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var products = _context.Products
            .Include(item => item.Category)
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == false)
            .Select(item => new AdminProductListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                PromotionPrice = item.PromotionPrice,
                Quantity = item.StockQuantity,
                CategoryName = item.Category.Name,
                ImagePath = _productImageService.GetImageMainProductById(item.Id),
                CreatedDate = item.CreatedAt,
            })
            .ToPagedList(pageNumber, pageSize);

        return products;
    }

    public IPagedList<AdminProductTrashViewModel> GetProductsWithPaginationAdminTrash(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;

        var products = _context.Products
            .Include(item => item.Category)
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == true)
            .Select(item => new AdminProductTrashViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                PromotionPrice = item.PromotionPrice,
                ImagePath = _productImageService.GetImageMainProductById(item.Id),
                Quantity = item.StockQuantity,
                CategoryName = item.Category.Name,
                CreatedDate = item.CreatedAt,
            })
            .ToPagedList(pageNumber, pageSize);

        return products;
    }
}