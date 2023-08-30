namespace FilesAPI
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public virtual ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();
    }
}
