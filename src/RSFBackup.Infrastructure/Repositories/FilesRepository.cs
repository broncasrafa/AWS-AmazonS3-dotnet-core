using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using RSFBackup.Core.DTO.Files;
using RSFBackup.Core.Interfaces.Repositories;

namespace RSFBackup.Infrastructure.Repositories;

public class FilesRepository : IFilesRepository
{
    private readonly IAmazonS3 _amazonS3Client;

    public FilesRepository(IAmazonS3 amazonS3Client)
    {
        _amazonS3Client = amazonS3Client;
    }


    public async Task<IEnumerable<ListFilesResponse>> ListFilesAsync(string bucketName)
    {
        var response = await _amazonS3Client.ListObjectsAsync(bucketName);
        return response.S3Objects.Select(obj => new ListFilesResponse(
            obj.BucketName,
            obj.Key,
            obj.Owner.DisplayName,
            obj.Size, 
            obj.LastModified));
    }

    public async Task<AddFileResponse> UploadFilesAsync(string bucketName, IFormFileCollection files)
    {
        var preSignedUrls = new List<PreSignedUrlFileResponse>();
        var expires = DateTime.Now.AddDays(1);

        foreach (var file in files)
        {
            var request = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                InputStream = file.OpenReadStream(),
                Key = file.FileName,
                CannedACL = S3CannedACL.NoACL
            };

            using var transferUtility = new TransferUtility(_amazonS3Client);
            await transferUtility.UploadAsync(request);

            var expirePreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName, 
                Key = file.FileName, 
                Expires = expires,
            };

            var preSignedUrl = _amazonS3Client.GetPreSignedURL(expirePreSignedUrlRequest);
            preSignedUrls.Add(new PreSignedUrlFileResponse(file.FileName, preSignedUrl, expires));
        }

        return new AddFileResponse(bucketName, preSignedUrls);
    }

    public async Task DownloadFileAsync(string bucketName, string filename)
    {
        string pathToSave = @"C:\Users\bronc\OneDrive\Área de Trabalho\aws";
        var request = new TransferUtilityDownloadRequest
        {
            BucketName = bucketName,
            Key = filename,
            FilePath = pathToSave
        };

        using var transferUtility = new TransferUtility(_amazonS3Client);
        await transferUtility.DownloadAsync(request);
    }
}
