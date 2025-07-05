namespace E_commerce_Project.Models.Services.FileService;

public interface IFileService
{
    void EnsureFolderExist(string folderPath);
    string GetUploadsFolder(string subFolder);
    
    string GetUploadsFolderByIdItem (string subfolder, string idItem);
    string GetRelativePath(string absolutePath);
    Task<string> SaveFileAsync(IFormFile file, string fullPath);

    void DeleteFile(string filePath);
    void DeleteFolder(string folderPath);
}