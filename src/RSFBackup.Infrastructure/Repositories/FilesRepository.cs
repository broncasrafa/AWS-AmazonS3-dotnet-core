using Microsoft.AspNetCore.Http;
using RSFBackup.Core.DTO.Files;
using RSFBackup.Core.Interfaces.Repositories;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Newtonsoft.Json;


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

    public async Task<DeleteFileResponse> DeleteFileAsync(string bucketName, string filename)
    {
        var request = new DeleteObjectsRequest { BucketName = bucketName };
        request.AddKey(filename);
        
        var response = await _amazonS3Client.DeleteObjectsAsync(request);

        return new DeleteFileResponse(response.DeletedObjects.Count);
    }

    public async Task<string> AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request)
    {
        DateTime createdOnUtc = DateTime.UtcNow;
        
        string s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:dd}/{request.Id}";
        
        var putObjectRequest = new PutObjectRequest
        {
            BucketName=bucketName,
            Key = s3Key,
            ContentBody = JsonConvert.SerializeObject(request)
        };

        await _amazonS3Client.PutObjectAsync(putObjectRequest);

        return s3Key;
    }

    public async Task<GetJsonObjectResponse> GetJsonObjectAsync(string bucketName, string filename)
    {
        GetObjectRequest request = new GetObjectRequest { BucketName = bucketName, Key = filename };
        GetObjectResponse response = await _amazonS3Client.GetObjectAsync(request);
        using var sr = new StreamReader(response.ResponseStream);
        string content = sr.ReadToEnd();
        return JsonConvert.DeserializeObject<GetJsonObjectResponse>(content);
    }
}
