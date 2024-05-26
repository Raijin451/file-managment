using file_managment.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace file_managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _uploadsDirectory;
        private readonly string _connectionString = "Server=.;Initial Catalog=file;User Id=hakan45;Password=hakan123;TrustServerCertificate=True";
        
        public FilesController(IWebHostEnvironment env)
        {
            _uploadsDirectory = Path.Combine(env.ContentRootPath, "Uploads");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile fileUploadDto)
        {
            if (fileUploadDto == null || fileUploadDto.Length == 0)
                return BadRequest("Invalid file");

            var filePath = Path.Combine(_uploadsDirectory, fileUploadDto.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUploadDto.CopyToAsync(stream);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Files (FileName, Size) VALUES (@FileName, @Size)", connection);
                command.Parameters.AddWithValue("@FileName", fileUploadDto.FileName);
                command.Parameters.AddWithValue("@Size", fileUploadDto.Length);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            return Ok("File uploaded successfully");
        }

        [HttpGet("list")]
        public IActionResult ListFiles()
        {
            var files = new List<FileDTO>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT FileName, Size FROM Files", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        files.Add(new FileDTO
                        {
                            FileName = reader.GetString(0),
                            Size = reader.GetInt64(1)
                        });
                    }
                }
                connection.Close();
            }
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

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Files WHERE FileName = @FileName", connection);
                command.Parameters.AddWithValue("@FileName", fileName);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            System.IO.File.Delete(filePath);
            return Ok("File deleted successfully");
        }
    }
}
