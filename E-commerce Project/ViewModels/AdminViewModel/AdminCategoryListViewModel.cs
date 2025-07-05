namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminCategoryListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    
    public int Order { get; set; }
    public DateTime CreatedDate { get; set; }
}