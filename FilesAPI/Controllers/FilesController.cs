using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        

        public FilesController()
        {
            
        }

        [HttpGet("{fileId}",Name = "GetFile")]
        public IActionResult Get(int fileId)
        {

            return Ok();
        }
    }
}
