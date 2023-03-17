namespace RSFBackup.Core.DTO.Bucket;

public class ListS3BucketsResponse
{
    public string BucketName { get; private set; }
    public DateTime CreationDate { get; private set; }

    public ListS3BucketsResponse(string bucketName, DateTime creationDate)
    {
        BucketName = bucketName;
        CreationDate = creationDate;
    }
}
