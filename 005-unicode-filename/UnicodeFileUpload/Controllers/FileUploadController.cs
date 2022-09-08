using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace UnicodeFileUpload.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(ILogger<FileUploadController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Action for upload large file
    /// </summary>
    /// <remarks>
    /// Request to this action will not trigger any model binding or model validation,
    /// because this is a no-argument action
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    [Route(nameof(UploadLargeFile))]
    public async Task<IActionResult> UploadLargeFile()
    {
        var request = HttpContext.Request;

        // validation of Content-Type
        // 1. first, it must be a form-data request
        // 2. a boundary should be found in the Content-Type
        if (!request.HasFormContentType ||
            !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
            string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        {
            return new UnsupportedMediaTypeResult();
        }

        var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
        var section = await reader.ReadNextSectionAsync();

        // This sample try to get the first file from request and save it
        // Make changes according to your needs in actual use
        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                out var contentDisposition);

            if (hasContentDispositionHeader &&
                (contentDisposition != null) &&
                contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                // WARNING: Don't trust any file name, file extension, and file data from the request unless you trust them completely
                // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                // In short, it is necessary to restrict and verify the upload
                // Here, we just use the temporary folder and a random file name

                // BEST PRACTICE: Get the temporary folder, and combine a random file name with it
                // var fileName = Path.GetRandomFileName ();
                string fileName;
                if (!string.IsNullOrEmpty(contentDisposition.FileNameStar.Value))
                    fileName = contentDisposition.FileNameStar.Value;
                else
                    fileName = contentDisposition.FileName.Value;

                var saveToPath = Path.Combine(Path.GetTempPath(), fileName);

                using (var targetStream = System.IO.File.Create(saveToPath))
                {
                    await section.Body.CopyToAsync(targetStream);
                }
                _logger.LogInformation("Filename: {0}", contentDisposition.FileName.Value);
                _logger.LogInformation("UTF-8 filename: {0}", contentDisposition.FileNameStar.Value);
                _logger.LogInformation("file received: {0}", saveToPath);

                return Ok();
            }

            section = await reader.ReadNextSectionAsync();
        }

        // If the code runs to this location, it means that no files have been saved
        return BadRequest("No files data in the request.");
    }

}
