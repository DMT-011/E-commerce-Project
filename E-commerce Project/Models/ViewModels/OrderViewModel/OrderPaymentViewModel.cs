namespace E_commerce_Project.Models.ViewModels.OrderViewModel;

public class OrderPaymentViewModel
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    
    public decimal TotalOrder { get; set; }
}