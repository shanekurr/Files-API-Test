
using Microsoft.AspNetCore.Mvc;

namespace FilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FilesContext _context;

        public FilesController()
        {
            _context = new FilesContext();
        }

        [HttpGet("{fileId}", Name = "GetFile")]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public IActionResult Get(int fileId)
        {
            return _context.Files.Find(fileId) is File file ? Ok(file) : NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(IFormFile file)
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
            var path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Guid.NewGuid().ToString());
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
            return CreatedAtRoute("GetFile", new { fileId = newFile.Id }, newFile);
        }
    }
}
