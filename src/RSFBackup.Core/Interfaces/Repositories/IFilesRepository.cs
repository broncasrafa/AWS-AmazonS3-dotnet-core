using Microsoft.AspNetCore.Http;
using RSFBackup.Core.DTO.Files;

namespace RSFBackup.Core.Interfaces.Repositories;

public interface IFilesRepository
{
    Task<AddFileResponse> UploadFilesAsync(string bucketName, IFormFileCollection files);
}
