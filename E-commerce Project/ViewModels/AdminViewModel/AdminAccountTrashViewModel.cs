namespace E_commerce_Project.Models.ViewModels.AdminViewModel;

public class AdminAccountTrashViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public DateTime DeleteDate { get; set; }
    public string UserModifier { get; set; }
}
