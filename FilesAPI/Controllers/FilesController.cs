
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var file = _context.Files.Include(t => t.Versions).FirstOrDefault(f => f.Id == fileId);
            return file != null ? Ok(file) : NotFound();
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

        [HttpPut]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(int fileId, IFormFile file)
        {
            var existingFile = await _context.Files.FindAsync(fileId);
            if (existingFile == null)
            {
                return NotFound();
            }
            var path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Guid.NewGuid().ToString());
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
            return Ok(existingFile);
        }

        [HttpGet("", Name = "GetFiles")]
        [ProducesResponseType(typeof(IEnumerable<FilesAPI.File>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var files = _context.Files.Include(t => t.Versions).ToList();
            return Ok(files);
        }

        
        [HttpDelete("{fileId}", Name = "DeleteFile")]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
            {
                return NotFound();
            }
            var versions = _context.FileVersions.Where(v => v.FileId == fileId);
            foreach (var fileVersion in versions)
            {
                try
                {
                    System.IO.File.Delete(fileVersion.Path);
                }
                catch(Exception e)
                {
                    //TODO: log error
                }
            }
            
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return Ok(file);
        }
    }
}
