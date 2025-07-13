namespace E_commerce_Project.ViewModels.AdminViewModel;

public class AdminCustomerTrashViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public DateTime DeleteDate { get; set; }
    public string UserModifier { get; set; }
}