using Microsoft.AspNetCore.Mvc;

namespace E_commerce_Project.Views.ViewComponents;

public class HeaderCartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}