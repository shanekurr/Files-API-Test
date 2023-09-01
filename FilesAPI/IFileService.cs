namespace FilesAPI;

public interface IFileService
{
    File GetFile(int fileId);
    IEnumerable<File> GetFiles();
    Task<File> CreateFileAsync(IFormFile file);
    Task<File> UpdateFileAsync(int fileId, IFormFile file);
    Task DeleteFileAsync(int fileId);
}