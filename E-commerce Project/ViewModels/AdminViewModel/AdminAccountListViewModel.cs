using E_commerce_Project.Models.Enums;

namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminAccountListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    
    public AccountStatusType Status { get; set; }
}