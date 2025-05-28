using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.ViewModels.AdminViewModel;
using E_commerce_Project.Models.ViewModels.SliderViewModel;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_commerce_Project.Models.Services.SliderService;

public class SliderService : ISliderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SliderService> _logger;
    private readonly IFileService _fileService;

    public SliderService(ApplicationDbContext context, ILogger<SliderService> logger
    , IFileService fileService)
    {
        _context = context;
        _logger = logger;
        _fileService = fileService;
    }
    
    public async Task CreateSliderAsync(SliderCreateViewModel model)
    {
        var sliderName = model.Name.Trim();
        var slider = await _context.Slides
            .Where(item => item.Name == sliderName)
            .FirstOrDefaultAsync();
        
        if (slider != null) throw new Exception($"Name slider {sliderName} is already exist"); 

        var priorityMax = _context.Slides
            .Where(item => item.IsDeleted == false)
            .Max(item => item.Priority)
            .GetValueOrDefault();

        slider = new Slide
        {
            Name = sliderName,
            IsDisplayed = model.IsDisplayed,
        };
        
        // Set priority = 1 when first time init slider
        if (priorityMax == 0 && model.Priority == 0)
        {
            priorityMax = 1;
            slider.Priority = priorityMax;
        }

        var sliderPriorityOld = await _context.Slides
            .Where(item => item.Priority == model.Priority && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        // Change position slider old and slider new create when select priority client
        if (sliderPriorityOld != null)
        {
            sliderPriorityOld.Priority = ++priorityMax;
            slider.Priority = model.Priority;
            _context.Slides.Update(sliderPriorityOld);
            await _context.SaveChangesAsync();
        }
        else
        {
            slider.Priority = ++priorityMax;
        }
        
        _context.Slides.Add(slider);
        await _context.SaveChangesAsync();
        await CreateImageSliderAsync(slider, model);
        _logger.LogInformation($"Product: {sliderName} has created successfully");
    }

    public async Task UpdateSliderAsync(int id, SliderUpdateViewModel model)
    {
        var name = model.Name.Trim();
        var sliderExist = await _context.Slides
            .Where(item => item.Id != id && item.Name == name)
            .FirstOrDefaultAsync();
        
        if (sliderExist != null) throw new Exception("Name slider is already exist");

        var slider = await _context.Slides
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (slider == null) throw new Exception("Slider not found.");
        var priorityOld = slider.Priority;
            
        slider.Name = name;
        slider.IsDisplayed = model.IsDisplayed;
        slider.Priority = model.Priority;

        var sliderPriorityOld = await _context.Slides
            .Where(item => item.Priority == model.Priority && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (sliderPriorityOld != null)
        {
            sliderPriorityOld.Priority = priorityOld;
            _context.Slides.Update(sliderPriorityOld);
            await _context.SaveChangesAsync();
        }

        if (model.ImageSlide != null && model.ImageSlide.Length > 0)
        {
            await UpdateImageSliderAsync(slider, model);
        }
        
        _context.Slides.Update(slider);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSliderAsync(int id)
    {
        var slider = await _context.Slides
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        if (slider == null) throw new Exception("Slider not found.");

        slider.IsDeleted = true;
        
        _context.Slides.Update(slider);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Slider ${slider.Id} deleted successfully.");
    }

    public async Task ForceDeleteSliderAsync(int id)
    {
        var slider = await _context.Slides
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();
        
        if (slider == null) throw new Exception("Slider not found.");

        var sliderFolder = _fileService.GetUploadsFolderByIdItem("sliders", $"{slider.Id}");
        _fileService.DeleteFolder(sliderFolder);

        _context.Slides.Remove(slider);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deleted slider {slider.Id} in database successfully.");
    }

    public async Task RestoreSliderAsync(int id)
    {
        var slider = await _context.Slides
            .Where(item => item.Id == id && item.IsDeleted == true)
            .FirstOrDefaultAsync();
        
        if (slider == null) throw new Exception("Slider not found.");

        var checkExistPriority = await _context.Slides
            .Where(item => item.Priority == slider.Priority && item.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (checkExistPriority != null)
        {
            var priorityMax = await _context.Slides
                .Where(item => item.IsDeleted == false)
                .MaxAsync(item => item.Priority);

            slider.Priority = ++priorityMax;
        }
        
        slider.IsDeleted = false;
        _context.Slides.Update(slider);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Restored slide {slider.Id} successfully.");
    }

    public async Task CreateImageSliderAsync(Slide slider, SliderCreateViewModel model)
    {
        var sliderFolder = _fileService.GetUploadsFolderByIdItem("sliders", $"{slider.Id}");
        var imageSlider = model.ImageSlide;

        if (imageSlider == null && imageSlider.Length < 0)
        {
            throw new Exception("Image slider is empty");
        } 
        
        var imagePath = await _fileService.SaveFileAsync(imageSlider, sliderFolder);
        slider.ImagePath = _fileService.GetRelativePath(imagePath);
        
        _context.Slides.Update(slider);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Slider image {slider.Name} created successfully");
    }

    public async Task UpdateImageSliderAsync(Slide slider, SliderUpdateViewModel model)
    {
        var sliderFolder = _fileService.GetUploadsFolderByIdItem("sliders", $"{slider.Id}");
        var imageOldPath = Path.Combine(sliderFolder, Path.GetFileName(slider.ImagePath));
        var imageNew = model.ImageSlide;
        
        // Delete file image old and save image new update
        _fileService.DeleteFile(imageOldPath);
        var imagePath = await _fileService.SaveFileAsync(imageNew, sliderFolder);
        slider.ImagePath = _fileService.GetRelativePath(imagePath);
        
        _logger.LogInformation($"Updated image slider {slider.Id} successfully");
    }

    public List<Slide> GetAllSlides()
    {
        var slides = _context.Slides
            .Where(item => item.IsDeleted == false)
            .ToList();
        
        _logger.LogInformation("Get all slide image successfully");
        return slides;
    }

    public List<Slide> GetAllSlidesDeleted()
    {
        var sliders = _context.Slides
            .Where(item => item.IsDeleted == true)
            .ToList();

        _logger.LogInformation("Get all slide image deleted successfully");
        return sliders;
    }

    public async Task<Slide> GetSliderByIdAsync(int id)
    {
        var slider = await _context.Slides
            .Where(item => item.Id == id && item.IsDeleted == false)
            .FirstOrDefaultAsync();
        
        _logger.LogInformation($"Get slider image {slider.Id} successfully");
        return slider;
    }

    public IPagedList<AdminSliderListViewModel> GetSlidersWithPaginationAdmin(int? page)
    {
        int pageSize = 5;    
        int pageNumber = page ?? 1;

        var sliders = _context.Slides
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == false)
            .Select(item => new AdminSliderListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ImagePath = item.ImagePath
            })
            .ToPagedList(pageNumber, pageSize);
        return sliders;
    }

    public IPagedList<AdminSliderTrashViewModel> GetSlidersWithPaginationAdminTrash(int? page)
    {
        int pageSize = 5;    
        int pageNumber = page ?? 1;

        var sliders = _context.Slides
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .Where(item => item.IsDeleted == true)
            .Select(item => new AdminSliderTrashViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ImagePath = item.ImagePath
            })
            .ToPagedList(pageNumber, pageSize);
        return sliders;
    }
}