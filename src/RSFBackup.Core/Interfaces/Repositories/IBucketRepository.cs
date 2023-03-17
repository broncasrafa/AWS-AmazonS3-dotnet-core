using RSFBackup.Core.DTO.Bucket;

namespace RSFBackup.Core.Interfaces.Repositories;

public interface IBucketRepository
{
    Task<bool> IsBucketS3ExistAsync(string bucketName);
    Task<IEnumerable<ListS3BucketsResponse>> GetAllBucketsAsync();
    Task<CreateS3BucketResponse> CreateBucketAsync(string bucketName);    
    Task<bool> DeleteBucketAsync(string bucketName);
}
