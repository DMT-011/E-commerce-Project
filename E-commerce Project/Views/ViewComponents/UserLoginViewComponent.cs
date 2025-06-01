using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Views.ViewComponents;

public class UserLoginViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new UserLoginViewModel();
        return View(model);
    }
}