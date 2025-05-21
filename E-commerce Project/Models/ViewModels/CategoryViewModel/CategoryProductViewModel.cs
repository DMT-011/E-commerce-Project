using E_commerce_Project.Models.ViewModels.ProductViewModel;

namespace E_commerce_Project.Models.ViewModels.CategoryViewModel;

public class CategoryProductViewModel
{ 
    public string CategoryName { get; set; }
    public List<ProductItemViewModel> Products { get; set; }
}