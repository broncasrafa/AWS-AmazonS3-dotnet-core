using Amazon.S3;
using Amazon.S3.Model;
using RSFBackup.Core.DTO.Bucket;
using RSFBackup.Core.Interfaces.Repositories;

namespace RSFBackup.Infrastructure.Repositories;

public class BucketRepository : IBucketRepository
{
    private readonly IAmazonS3 _amazonS3Client;

    public BucketRepository(IAmazonS3 amazonS3Client)
    {
        _amazonS3Client = amazonS3Client;
    }

    

    public async Task<bool> IsBucketS3ExistAsync(string bucketName)
    {
        return await _amazonS3Client.DoesS3BucketExistAsync(bucketName);
    }

    public async Task<CreateS3BucketResponse> CreateBucketAsync(string bucketName)
    {
        var request = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
        var response = await _amazonS3Client.PutBucketAsync(request);
        return new CreateS3BucketResponse(response.ResponseMetadata.RequestId, bucketName);
    }

    public async Task<IEnumerable<ListS3BucketsResponse>> GetAllBucketsAsync()
    {
        var response = await _amazonS3Client.ListBucketsAsync();
        return response.Buckets.Select(bucket => new ListS3BucketsResponse(bucket.BucketName, bucket.CreationDate));
    }

    public async Task<bool> DeleteBucketAsync(string bucketName)
    {
        var response = await _amazonS3Client.DeleteBucketAsync(bucketName);
        return response != null;
    }
}
