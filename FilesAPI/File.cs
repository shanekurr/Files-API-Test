using System.Text.Json.Serialization;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasMany(f => f.Versions)
                .WithOne(v => v.File)
                .HasForeignKey(v => v.FileId);
            modelBuilder.Entity<FileVersion>()
                .HasOne(v => v.File);
        }
    }

    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public virtual ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();
    }

    public class FileVersion
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int Version { get; set; }
        [JsonIgnore]
        public File File { get; set; } = null!;
        public string Path { get; set; } = null!;
        public DateTime Created { get; set; }
    }
}
