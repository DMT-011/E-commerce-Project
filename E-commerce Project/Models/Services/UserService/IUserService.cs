using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.UserViewModel;

namespace E_commerce_Project.Models.Services.UserService;

public interface IUserService
{
    // Command
    Task CreateUserAsync(UserCreateViewModel model);
}