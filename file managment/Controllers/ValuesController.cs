using file_managment.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace file_managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly string _uploadsDirectory;

        public ValuesController(IWebHostEnvironment env)
        {
            _uploadsDirectory = Path.Combine(env.ContentRootPath, "Uploads");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDTO fileUploadDto)
        {
            if (fileUploadDto == null || fileUploadDto.File == null || fileUploadDto.File.Length == 0)
                return BadRequest("Invalid file");

            var filePath = Path.Combine(_uploadsDirectory, fileUploadDto.File.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUploadDto.File.CopyToAsync(stream);
            }

            return Ok("File uploaded successfully");
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult ListFiles()
        {
            var files = Directory.GetFiles(_uploadsDirectory)
                                .Select(filePath => new FileDTO
                                {
                                    FileName = Path.GetFileName(filePath),
                                    Size = new FileInfo(filePath).Length
                                })
                                .ToList();
            return Ok(files);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var filePath = Path.Combine(_uploadsDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }

        [HttpDelete("delete/{fileName}")]
        public IActionResult Delete(string fileName)
        {
            var filePath = Path.Combine(_uploadsDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            System.IO.File.Delete(filePath);
            return Ok("File deleted successfully");
        }
    }
}
