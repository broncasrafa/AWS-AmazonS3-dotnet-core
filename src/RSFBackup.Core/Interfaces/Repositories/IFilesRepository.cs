using Microsoft.AspNetCore.Http;
using RSFBackup.Core.DTO.Files;

namespace RSFBackup.Core.Interfaces.Repositories;

public interface IFilesRepository
{
    Task<AddFileResponse> UploadFilesAsync(string bucketName, IFormFileCollection files);
    Task<IEnumerable<ListFilesResponse>> ListFilesAsync(string bucketName);
    Task DownloadFileAsync(string bucketName, string filename);
    Task<DeleteFileResponse> DeleteFileAsync(string bucketName, string filename);
    Task<string> AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request);
    Task<GetJsonObjectResponse> GetJsonObjectAsync(string bucketName, string filename);
}
