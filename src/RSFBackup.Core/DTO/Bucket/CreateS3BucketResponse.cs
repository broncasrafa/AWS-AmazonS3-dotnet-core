namespace RSFBackup.Core.DTO.Bucket;

public class CreateS3BucketResponse
{
    public string RequestId { get; private set; }
    public string BucketName { get; private set; }

    public CreateS3BucketResponse(string requestId, string bucketName)
    {
        RequestId = requestId;
        BucketName = bucketName;
    }
}
