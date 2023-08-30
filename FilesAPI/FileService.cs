using Microsoft.EntityFrameworkCore;

namespace FilesAPI
{
    public class FileService
    {
        readonly FilesContext _context;

        public FileService()
        {
            _context = new FilesContext();
            _context.Database.EnsureCreated();
        }

        public File GetFile(int fileId)
        {
            var file = _context.Files.Include(t => t.Versions).FirstOrDefault(f => f.Id == fileId);
            return file ?? new File();
        }

        public IEnumerable<File> GetFiles()
        {
            return _context.Files.Include(t => t.Versions).ToList();
        }

        public async Task<File> CreateFileAsync(IFormFile file)
        {
            var newFile = new File
            {
                Name = file.FileName,
                ContentType = file.ContentType,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            await _context.Files.AddAsync(newFile);
            await _context.SaveChangesAsync();
            var path = this.getFilePath(Guid.NewGuid().ToString());
            await using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);
            var newVersion = new FileVersion
            {
                FileId = newFile.Id,
                Version = 1,
                Path = path,
                Created = DateTime.UtcNow
            };
            await _context.FileVersions.AddAsync(newVersion);
            await _context.SaveChangesAsync();
            return newFile;
        }

        public async Task<File> UpdateFileAsync(int fileId, IFormFile file)
        {
            var existingFile = await _context.Files.FindAsync(fileId);
            if (existingFile == null)
            {
                return null;
            }
            var path = this.getFilePath(Guid.NewGuid().ToString());
            using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);
            var newVersion = new FileVersion
            {
                FileId = existingFile.Id,
                Version = existingFile.Versions.Max(t => t.Version) + 1,
                Path = path,
                Created = DateTime.UtcNow
            };
            _context.FileVersions.Add(newVersion);
            existingFile.Modified = DateTime.UtcNow;
            existingFile = _context.Files.Update(existingFile).Entity;
            await _context.SaveChangesAsync();
            return existingFile;
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var existingFile = await _context.Files.FindAsync(fileId);
            if (existingFile == null)
            {
                return;
            }
            var versions = _context.FileVersions.Where(v => v.FileId == fileId);
            foreach (var fileVersion in versions)
            {
                try
                {
                    System.IO.File.Delete(fileVersion.Path);
                }
                catch (Exception e)
                {
                    //TODO: log error
                }
            }
            _context.Files.Remove(existingFile);
            await _context.SaveChangesAsync();
        }

        private string getFilePath(string fileName)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            path = System.IO.Path.Combine(path, "FilesAPI");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return System.IO.Path.Join(path, fileName);
        }
    }
}
