using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using E_commerce_Project.ViewModels.AdminViewModel;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace E_commerce_Project.Services.UserService;

public interface IUserService
{
    // Command
    Task CreateUserAsync(UserCreateViewModel model);
    Task DeleteUserAsync(int id, int userIdModifier);
    Task RestoreUserAsync(int id);

    Task<UserAccountStatusResultViewModel> UpdateStatusAccountUserAsync([FromBody] UserStatusAccountViewModel model, int id);
    // Query
    User GetUserById(int id);
    IPagedList<AdminAccountListViewModel> GetAllAccountListWithPagination(int? page);
    IPagedList<AdminAccountTrashViewModel> GetAllAccountTrashWithPagination(int? page);
    IPagedList<AdminCustomerListViewModel> GetAllCustomerWithPagination(int? page);
    IPagedList<AdminCustomerTrashViewModel> GetAllCustomerTrashWithPagination(int? page);
    
    int GetTotalAccountAdminDelete();
    int GetTotalCustomerDelete();
}