using E_commerce_Project.Helpers;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Enums;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.UserViewModel;
using E_commerce_Project.ViewModels.AdminViewModel;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_commerce_Project.Services.UserService;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IFileService _fileService;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger, IFileService fileService)
    {
        _context = context;
        _logger = logger;
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

        if (checkExistEmail != null) throw new Exception($"Email already exists");

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
            Role = (int)UserRoleType.Customer
        };

        if (model.Gender != null &&
            model.Role != null &&
            model.Gender != null
           )
        {
            user.Gender = (int)model.Gender;
            user.AccountStatus = (int)model.AccountStatus;
            user.Role = (int)model.Role;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (user.Role == (int)UserRoleType.Admin) await CreateAvatarUser(model, user);
        _logger.LogInformation($"User {userName} with ID {user.Id} created successfully");
    }

    public async Task DeleteUserAsync(int id, int userIdModifier)
    {
        var user = await _context.Users
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (user == null) throw new Exception("User not found!");

        if (user.Id == userIdModifier) throw new Exception("Dont delete account yourself!");

        user.IsDeleted = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("User has been deleted");
    }

    public async Task RestoreUserAsync(int id)
    {
        var user = await _context.Users
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();

        if (user == null) throw new Exception("User not found!");

        user.IsDeleted = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"User has {id} restore account successfully");
    }

    public async Task<UserAccountStatusResultViewModel> UpdateStatusAccountUserAsync(UserStatusAccountViewModel model,
        int userIdModifier)
    {
        var user = await _context.Users
            .Where(item => item.Id == model.UserId && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return new UserAccountStatusResultViewModel
            {
                StatusCode = 404,
                Message = "User update status not found!"
            };
        }

        if (user.Id == userIdModifier)
        {
            return new UserAccountStatusResultViewModel
            {
                StatusCode = 304,
                Message = "Account status update not change."
            };
        }

        var isActive = user.AccountStatus == (int)AccountStatusType.Active;
        // Update status (active) account has been banned 
        if (!isActive && model.Status == (int)AccountStatusType.Active)
        {
            user.AccountStatus = model.Status;
            _context.Users.Update(user);
        }
        else
        {
            user.AccountStatus = model.Status;
            _context.Users.Update(user);
        }

        await _context.SaveChangesAsync();
        return new UserAccountStatusResultViewModel
        {
            StatusCode = 200,
            Message = "Account status updated"
        };
    }

    public User GetUserById(int id)
    {
        var user = _context.Users
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefault();
        if (user == null) throw new Exception($"User with ID {id} not found");
        return user;
    }

    public IPagedList<AdminAccountListViewModel> GetAllAccountListWithPagination(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;
        var roleAdmin = (int)UserRoleType.Admin;

        var accounts = _context.Users
            .AsNoTracking()
            .Where(item => item.Role == roleAdmin && item.IsDeleted == false)
            .OrderByDescending(item => item.UpdatedAt)
            .Select(item => new AdminAccountListViewModel
            {
                Id = item.Id,
                FullName = item.FullName,
                UserName = item.Username,
                Status = (AccountStatusType)item.AccountStatus,
            })
            .ToPagedList(pageNumber, pageSize);
        return accounts;
    }

    public IPagedList<AdminAccountTrashViewModel> GetAllAccountTrashWithPagination(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;
        var roleAdmin = (int)UserRoleType.Admin;

        var users = _context.Users
            .AsNoTracking()
            .Where(item => item.Role == roleAdmin && item.IsDeleted == true)
            .OrderByDescending(item => item.UpdatedAt)
            .ToList();

        var accounts = users.Select(item => new AdminAccountTrashViewModel
            {
                Id = item.Id,
                FullName = item.FullName,
                UserModifier = GetUserById(item.UpdatedBy ?? 1).FullName,
                UserName = item.Username,
                DeleteDate = item.UpdatedAt
            })
            .ToPagedList(pageNumber, pageSize);

        return accounts;
    }

    public IPagedList<AdminCustomerListViewModel> GetAllCustomerWithPagination(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;
        var roleCustomer = (int)UserRoleType.Customer;

        var users = _context.Users
            .AsNoTracking()
            .Where(item => item.Role == roleCustomer && item.IsDeleted == false)
            .OrderByDescending(item => item.UpdatedAt)
            .Select(item => new AdminCustomerListViewModel
            {
                Id = item.Id,
                FullName = item.FullName,
                Phone = item.Phone,
                Email = item.Email,
                Status = (AccountStatusType)item.AccountStatus
            }).ToPagedList(pageNumber, pageSize);
        return users;
    }

    public IPagedList<AdminCustomerTrashViewModel> GetAllCustomerTrashWithPagination(int? page)
    {
        int pageSize = 5;
        int pageNumber = page ?? 1;
        var roleCustomer = (int)UserRoleType.Customer;

        var users = _context.Users
            .AsNoTracking()
            .Where(item => item.Role == roleCustomer && item.IsDeleted == true)
            .OrderByDescending(item => item.UpdatedAt)
            .ToList();

        var customers = users
            .Select(item => new AdminCustomerTrashViewModel
            {
                Id = item.Id,
                FullName = item.FullName,
                Phone = item.Phone,
                DeleteDate = item.UpdatedAt,
                UserModifier = GetUserById(item.UpdatedBy ?? 0).FullName,
            }).ToPagedList(pageNumber, pageSize);
        return customers;
    }

    public int GetTotalAccountAdminDelete()
    {
        var roleAdmin = (int)UserRoleType.Admin;
        var accountDeletes = _context.Users
            .Count(item => item.Role == roleAdmin && item.IsDeleted == true);
        return accountDeletes;
    }

    public int GetTotalCustomerDelete()
    {
        var roleCustomer = (int)UserRoleType.Customer;
        var customerDeletes = _context.Users
            .Count(item => item.Role == roleCustomer && item.IsDeleted == true);
        return customerDeletes;
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