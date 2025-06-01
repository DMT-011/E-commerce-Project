namespace E_commerce_Project.Models.ViewModels.UserViewModel;

public class UserCreateViewModel
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int Gender { get; set; } // 1: male, 2: female, 3: other
    public string? Avatar { get; set; }
    public int AccountStatus { get; set; }
    public int RoleId { get; set; }
}