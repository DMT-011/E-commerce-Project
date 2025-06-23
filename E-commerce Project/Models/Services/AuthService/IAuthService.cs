using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;

namespace E_commerce_Project.Models.Services.AuthService;

public interface IAuthService
{
    Task<User> AuthenticateCustomer(UserLoginViewModel model);
    Task<User> AuthenticateAdmin(AdminLoginViewModel model);
}