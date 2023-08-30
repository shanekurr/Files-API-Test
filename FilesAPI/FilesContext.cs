using Microsoft.EntityFrameworkCore;

namespace FilesAPI;

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