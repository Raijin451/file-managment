using Microsoft.AspNetCore.Http;

namespace file_managmentUI.Models
{
    public class FileUploadDTO
    {
        public IFormFile File { get; set; }
    }
}
