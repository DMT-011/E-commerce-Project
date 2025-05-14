using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.ProductViewModel;

namespace E_commerce_Project.Models.Services.ProductImageService;

public interface IProductImageService
{
    Task CreateImagesProductAsync(int productId, ProductCreateViewModel model);
    Task UpdateImagesProductAsync(int productId, ProductUpdateViewModel model);
    Task DeleteImagesProductAsync(int productId);
    
    
    // Query
    string GetImageSubProductByOrder(int order, int id);
    string GetImageMainProductById(int id);
    Task<ProductImage> GetProductImage(int id, int order, bool isPrimary);
}