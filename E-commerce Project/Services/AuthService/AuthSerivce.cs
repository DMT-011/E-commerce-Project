using System.Security.Claims;
using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.AuthService;

public class AuthSerivce : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthSerivce(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> AuthenticateCustomer(UserLoginViewModel model)
    {
        var userName = model.UserName.Trim();
        var password = model.Password.Trim();
        var user = await _context.Users
            .Include(item => item.Cart)
            .Where(item => item.Username == userName)
            .SingleOrDefaultAsync();
        
        if (user == null) throw new ApplicationException("User does not exist");
        
        var passwordValid = PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        if (!passwordValid) throw new Exception("Passwords do not valid");
        
        return user;
    }

    public async Task<User> AuthenticateAdmin(AdminLoginViewModel model)
    {
        var userName = model.UserName.Trim();
        var password = model.Password.Trim();
        var user = await _context.Users
            .Where(item => item.Username == userName)
            .SingleOrDefaultAsync();
        
        if (user == null) throw new ApplicationException("User does not exist");

        var isAdmin = user.Role == (int) UserRoleType.Admin;
        if (!isAdmin) throw new Exception("Access is blocked the role of the invalid account!");
        
        var passwordValid = PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        if (!passwordValid) throw new Exception("Passwords do not valid");
        
        return user;
    }
}