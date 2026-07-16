using Microsoft.AspNetCore.Http;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public interface IImageStorageService
{
    Task<UploadedImage> UploadImageAsync(IFormFile file, CancellationToken cancellationToken);
}