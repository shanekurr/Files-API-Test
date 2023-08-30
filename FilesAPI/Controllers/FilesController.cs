
using Microsoft.AspNetCore.Mvc;

namespace FilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileService _fileService;

        public FilesController()
        {
            _fileService = new FileService();
        }

        [HttpGet("{fileId}", Name = "GetFile")]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public IActionResult Get(int fileId)
        {
            var file = _fileService.GetFile(fileId);
            return file.Id > 0  ? Ok(file) : NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var newFile = await _fileService.CreateFileAsync(file);
            return CreatedAtRoute("GetFile", new { fileId = newFile.Id }, newFile);
        }

        [HttpPut]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(int fileId, IFormFile file)
        {
            var updatedFile = await _fileService.UpdateFileAsync(fileId, file);
            return Ok(updatedFile);
        }

        [HttpGet("", Name = "GetFiles")]
        [ProducesResponseType(typeof(IEnumerable<FilesAPI.File>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var files = _fileService.GetFiles();
            return Ok(files);
        }

        
        [HttpDelete("{fileId}", Name = "DeleteFile")]
        [ProducesResponseType(typeof(FilesAPI.File), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int fileId)
        {
            await _fileService.DeleteFileAsync(fileId);
            return Ok();
        }
    }
}
