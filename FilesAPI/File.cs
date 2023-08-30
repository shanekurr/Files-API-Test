using Microsoft.EntityFrameworkCore;

namespace FilesAPI
{
    public class FilesContext : DbContext
    {
        public DbSet<File> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }

        public string DbPath { get; }

        public FilesContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "files.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public List<FileVersion> Versions { get; set; } = new List<FileVersion>();
    }

    public class FileVersion
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int Version { get; set; }
        public File File { get; set; } = null!;
        public string Path { get; set; } = null!;
        public DateTime Created { get; set; }
    }
}
