using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.ViewModels.SliderViewModel;

namespace E_commerce_Project.Models.Services.SliderService;

public interface ISliderService
{
    // Command
    Task CreateSliderAsync(SliderCreateViewModel model);
    Task UpdateSliderAsync(int id, SliderUpdateViewModel model);
    Task DeleteSliderAsync(int id);
    Task ForceDeleteSliderAsync(int id);
    Task RestoreSliderAsync(int id);
    
    Task CreateImageSliderAsync(Slide entity, SliderCreateViewModel model);
    Task UpdateImageSliderAsync(Slide entity, SliderUpdateViewModel model);
    
    // Query
    List<Slide> GetAllSlides();
    List<Slide> GetAllSlidesDeleted();
    Task<Slide> GetSliderByIdAsync(int id);
    
}