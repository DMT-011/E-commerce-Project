using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_Project.Models.Services.UserService;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly ICartService _cartService;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger
    , ICartService cartService)
    {
        _context = context;
        _logger = logger;
        _cartService = cartService;
    }
    
    public async Task CreateUserAsync(UserCreateViewModel model)
    {
        var userName = model.Username.Trim();
        var email = model.Email.Trim();
        var phone = model.Phone.Trim();
        var password = model.Password.Trim();
        var passwordConfirm = model.PasswordConfirm.Trim();

        var checkExistUser = await _context.Users
            .Where(item => item.Username == userName)
            .SingleOrDefaultAsync();
        
        if (checkExistUser != null) throw new Exception($"User already exists");
        
        var checkExistEmail = await _context.Users
            .Where(item => item.Email == email)
            .SingleOrDefaultAsync();
        
        if (checkExistUser != null) throw new Exception($"Email already exists");
        
        var checkExistPhone = await _context.Users
            .Where(item => item.Phone == phone)
            .SingleOrDefaultAsync();
        
        if (checkExistPhone != null) throw new Exception($"Phone already exists");
        
        if(passwordConfirm != password) throw new Exception("Password confirm and password don't match");
        
        byte[] passwordHash, passwordSalt;
        PasswordHelper.GeneratePasswordHash(model.Password, out passwordHash, out passwordSalt);

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Phone = phone,
            Email = email,
            Username = userName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Address = model.Address,
            Gender = (int)GenderType.Other,
            AccountStatus = (int)AccountStatusType.Active,
            RoleId = 5,
            Avatar = model.Avatar ?? "",
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _cartService.CreateCartAsync(user.Id);
        _logger.LogInformation($"User {userName} with ID {user.Id} created successfully");
    }
}