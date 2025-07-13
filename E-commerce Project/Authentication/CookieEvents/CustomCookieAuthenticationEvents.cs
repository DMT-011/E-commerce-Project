using E_commerce_Project.Models.Enums;
using E_commerce_Project.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace E_commerce_Project.Authentication.CookieEvents;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IUserService _userService;
    
    public CustomCookieAuthenticationEvents(IUserService userService)
    {
        _userService = userService;
    }
    
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;
        var userId = userPrincipal.FindFirst("userId")?.Value;
        var user = _userService.GetUserById(int.Parse(userId));
        
        var isAdmin = user.Role == (int)UserRoleType.Admin;
        var isBanned = user.AccountStatus == (int)AccountStatusType.Banned;
        
        // Reject request account user role admin has been banned
        if (isAdmin && isBanned)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync("CookieAuthAdmin");
        }
        // Reject request account user role customer has been banned
        if (!isAdmin && isBanned)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync("CookieAuthCustomer");
        }
    }
}