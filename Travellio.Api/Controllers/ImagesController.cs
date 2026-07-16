using Microsoft.AspNetCore.Mvc;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class ImagesController(IImageStorageService imageStorageService) : ControllerBase
{
    // POST: Api/Images
    [HttpPost]
    [RequestSizeLimit(ImageContentTypes.MaxFileSizeBytes)]
    public async Task<ActionResult<UploadedImage>> PostImage(IFormFile file, CancellationToken cancellationToken)
    {
        switch (file.Length)
        {
            case 0:
                return BadRequest("File is empty.");
            case > ImageContentTypes.MaxFileSizeBytes:
                return BadRequest($"File exceeds the maximum allowed size of {ImageContentTypes.MaxFileSizeBytes / 1024 / 1024}MB.");
        }

        if (!ImageContentTypes.Extensions.ContainsKey(file.ContentType))
        {
            return BadRequest(
                $"Unsupported content type '{file.ContentType}'. Allowed types: {string.Join(", ", ImageContentTypes.Extensions.Keys)}.");
        }

        var uploadedImage = await imageStorageService.UploadImageAsync(file, cancellationToken);
        return Ok(uploadedImage);
    }
}