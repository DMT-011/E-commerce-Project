using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using X.PagedList;

namespace E_commerce_Project.Models.Services.ProductService;

public interface IProductService
{
    // Command
    Task CreateProductAsync(ProductCreateViewModel model);
    Task UpdateProductAsync(int id, ProductUpdateViewModel model);
    Task DeleteProductAsync(int id);
    Task ForceDeleteProductAsync(int id);
    Task RestoreProductAsync(int id);
    
    // Query
    Task<Product> GetProductByIdAsync(int id);
    Product GetProductById(int id);
    Task<Product> GetProductBySlugAsync(string slug);
    IQueryable<Product> GetAllProducts();
    IPagedList<AdminProductListViewModel> GetProductsWithPaginationAdmin(int? page);
    IPagedList<AdminProductTrashViewModel> GetProductsWithPaginationAdminTrash(int? page);
}