using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.ViewModels.ProductViewModel;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.ProductImageService;

public class ProductImageService : IProductImageService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly ILogger<ProductImageService> _logger;
    
    public ProductImageService(IFileService fileService, ApplicationDbContext context
        , ILogger<ProductImageService> logger)
    {
        _fileService = fileService;
        _context = context;
        _logger = logger;
    }
    
    public async Task CreateImagesProductAsync(int productId, ProductCreateViewModel model)
    {
        var productFolder =_fileService.GetUploadsFolderByIdItem("products", $"{productId}");
        var productImages = new List<ProductImage>();
        var order = 0;

        var imageMain = model.ImageMain;
        var imageSub1 = model.ImageSub1;
        var imageSub2 = model.ImageSub2;
        var imageSub3 = model.ImageSub3;
        var imagesSub = new List<IFormFile>();

        if (imageSub1 != null && imageSub2 != null && imageSub3 != null)
        {
            imagesSub.Add(imageSub1);      
            imagesSub.Add(imageSub2);      
            imagesSub.Add(imageSub3);      
        }
        
        // Save image main product in stored server
        if (imageMain != null && imageMain.Length > 0)
        {
            var imagePath = await _fileService.SaveFileAsync(imageMain, productFolder);
            productImages.Add(new ProductImage
            {
                ProductId = productId,
                FilePath = _fileService.GetRelativePath(imagePath),
                IsPrimary = true,
                Order = order,
            });
        }
        
        // Save images sub product in stored server
        if (imagesSub != null)
        {
            foreach (var imageSub in imagesSub)
            {
                var imagePath = await _fileService.SaveFileAsync(imageSub, productFolder);
                productImages.Add(new ProductImage
                {
                    ProductId = productId,
                    FilePath = _fileService.GetRelativePath(imagePath),
                    IsPrimary = false,
                    Order = ++order,
                });
            }
        }
        
        _context.ProductImages.AddRange(productImages);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Added 4 image product {productId} successfully");
    }

    public async Task UpdateImagesProductAsync(int productId, ProductUpdateViewModel model)
    {
        var imageMain = model.ImageMain;
        var imageSub1 = model.ImageSub1;
        var imageSub2 = model.ImageSub2;
        var imageSub3 = model.ImageSub3;

        var productFolder = _fileService.GetUploadsFolderByIdItem("products", $"{productId}");
        var productImages = new List<ProductImage>();
        
        if (imageMain != null && imageMain.Length > 0)
        {
            var imageMainOldFile = Path.GetFileName(GetImageMainProductById(productId));
            var imageMainOldPath = Path.Combine(productFolder, imageMainOldFile);
            
            // Delete image old and save image in stored server
            _fileService.DeleteFile(imageMainOldPath);
            var imageNewPath = await _fileService.SaveFileAsync(imageMain, productFolder);
            
            // Get image product update path in db
            var imageProduct = await GetProductImage(productId, 0, true);
            imageProduct.FilePath = _fileService.GetRelativePath(imageNewPath);
            productImages.Add(imageProduct);
        }

        if (imageSub1 != null)
        {
            var imageSub1OldFile = Path.GetFileName(GetImageSubProductByOrder(1, productId));
            var imageSub1OldPath = Path.Combine(productFolder, imageSub1OldFile);
            
            // Delete image old and save image in stored server
            _fileService.DeleteFile(imageSub1OldPath);
            var imageNewPath = await _fileService.SaveFileAsync(imageSub1, productFolder);
            var imageProduct = await GetProductImage(productId, 1, false);
            
            // Get image product update path in db
            imageProduct.FilePath = _fileService.GetRelativePath(imageNewPath);
            productImages.Add(imageProduct);
        }
        
        if (imageSub2 != null)
        {
            var imageSub2OldFile = Path.GetFileName(GetImageSubProductByOrder(2, productId));
            var imageSub2OldPath = Path.Combine(productFolder, imageSub2OldFile);
            
            // Delete image old and save image in stored server
            _fileService.DeleteFile(imageSub2OldPath);
            var imageNewPath = await _fileService.SaveFileAsync(imageSub2, productFolder);
                    
            // Get image product update path in db
            var imageProduct = await GetProductImage(productId, 2, false);
            imageProduct.FilePath = _fileService.GetRelativePath(imageNewPath);
            productImages.Add(imageProduct);
        }
        
        if (imageSub3 != null)
        {
            var imageSub3OldFile = Path.GetFileName(GetImageSubProductByOrder(3, productId));
            var imageSub3OldPath = Path.Combine(productFolder, imageSub3OldFile);
            
            // Delete image old and save image in stored server
            _fileService.DeleteFile(imageSub3OldPath);
            var imageNewPath = await _fileService.SaveFileAsync(imageSub3, productFolder);
            
            // Get image product update path in db
            var imageProduct = await GetProductImage(productId, 3, false);
            imageProduct.FilePath = _fileService.GetRelativePath(imageNewPath);
            productImages.Add(imageProduct);
        }

        if (productImages.Count > 0)
        {
            _context.ProductImages.UpdateRange(productImages);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated image product {productId} successfully");
        }
        
    }

    public async Task DeleteImagesProductAsync(int productId)
    {
        var productImages = _context.ProductImages
            .Where(item => item.ProductId == productId && item.IsDeleted == false)
            .ToList();

        if (productImages == null) throw new Exception($"Images product {productId} not found");
        
        // Delete folder contain image this product force delete
        var productFolder = _fileService.GetUploadsFolderByIdItem("products", $"{productId}");
        _fileService.DeleteFolder(productFolder);
        
        _context.ProductImages.RemoveRange(productImages);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deleted 4 images for product {productId} successfully.");
    }

    public string GetImageSubProductByOrder(int order, int id)
    {
        var image = _context.ProductImages
            .Where(item =>
                item.ProductId == id &&
                item.IsPrimary == false &&
                item.Order == order &&
                item.IsDeleted == false)
            .FirstOrDefault();
        
        if (image == null) throw new Exception($"Image sub product {id} not found.");
        return image.FilePath;
    }

    public string GetImageMainProductById(int id)
    {
        var image = _context.ProductImages
            .Where(item =>
                item.ProductId == id &&
                item.IsPrimary == true &&
                item.IsDeleted == false)
            .FirstOrDefault();
        
        if (image == null) throw new Exception($"Image main product {id} not found.");
        return image.FilePath;
    }

    public async Task<ProductImage> GetProductImage(int id, int order, bool isPrimary)
    {
        var imageProduct = _context.ProductImages
            .Where(item =>
                item.ProductId == id &&
                item.IsPrimary == isPrimary &&
                item.Order == order &&
                item.IsDeleted == false)
            .FirstOrDefault();
        
        if (imageProduct == null) throw new Exception($"Image product {id} not found.");
        return imageProduct;
    }
}