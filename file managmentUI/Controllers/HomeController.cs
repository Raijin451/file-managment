using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using file_managmentUI.Models;

public class HomeController : Controller
{
    private readonly APIService _apiService;

    public HomeController(APIService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var files = await _apiService.GetAsync<List<FileDTO>>("values/list");
        return View(files);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return RedirectToAction("Index");

        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            var form = new MultipartFormDataContent();
            form.Add(fileContent, "fileUploadDto", file.FileName); // Ensure "File" matches the expected key in API

            var response = await _apiService.PostAsync("values/upload", form);
            response.EnsureSuccessStatusCode(); // Check if the request was successful
        }

        return RedirectToAction("Index");
    }



    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var stream = await _apiService.GetFileStreamAsync($"values/download/{fileName}");
        return File(stream, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> DeleteFile(string fileName)
    {
        await _apiService.DeleteAsync($"values/delete/{fileName}");
        return RedirectToAction("Index");
    }
}
