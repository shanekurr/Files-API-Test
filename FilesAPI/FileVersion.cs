using System.Text.Json.Serialization;

namespace FilesAPI;

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