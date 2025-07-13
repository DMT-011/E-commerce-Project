using E_commerce_Project.Models.Enums;

namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminCustomerListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public AccountStatusType Status { get; set; }
}