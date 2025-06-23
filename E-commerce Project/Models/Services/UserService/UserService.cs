using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;

namespace E_commerce_Project.Models.Services.UserService;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly ICartService _cartService;
    private readonly IFileService _fileService;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger
        , ICartService cartService, IFileService fileService)
    {
        _context = context;
        _logger = logger;
        _cartService = cartService;
        _fileService = fileService;
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

        if (passwordConfirm != password) throw new Exception("Password confirm and password don't match");

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
            Role = (int) UserRoleType.Customer
        };

        if (model.Gender != null &&
            model.Role != null &&
            model.Gender != null
           )
        {
            user.Gender = (int)model.Gender;
            user.AccountStatus = (int)model.AccountStatus;
            user.Role = (int) model.Role;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        if (user.Role == (int)UserRoleType.Admin) await CreateAvatarUser(model, user);
        
        _logger.LogInformation($"User {userName} with ID {user.Id} created successfully");
    }

    public User GetUserByIdAsync(int id)
    {
        var user = _context.Users
            .Where(item => item.Id == id && item.IsDeleted == false)
            .SingleOrDefault();

        if (user == null) throw new Exception($"User with ID {id} not found");
        return user;
    }

    private async Task CreateAvatarUser(UserCreateViewModel model, User user)
    {
        var avatarFolder = _fileService.GetUploadsFolderByIdItem("avatars", $"{user.Id}");
        var imageAvatar = model.ImageAvatar;

        if (imageAvatar == null || imageAvatar.Length == 0)
        {
            throw new Exception("Image avatar upload is empty");
        }

        var imagePath = await _fileService.SaveFileAsync(imageAvatar, avatarFolder);

        user.Avatar = _fileService.GetRelativePath(imagePath);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Create avatar user successful");
    }
}