using E_commerce_Project.Models.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Views.ViewComponents;

public class UserRegisterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model =  new UserCreateViewModel();
        return View(model);
    }
}