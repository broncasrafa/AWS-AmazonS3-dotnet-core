namespace RSFBackup.Core.DTO.Files;

public class ListFilesResponse
{
    public string BucketName { get; private set; }
    public string Key { get; private set; }
    public string Owner { get; private set; }
    public long Size { get; private set; }
    public DateTime LastModified { get; private set; }

    public ListFilesResponse(string bucketName, string key, string owner, long size, DateTime lastModified)
    {
        BucketName = bucketName;
        Key = key;
        Owner = owner;
        Size = size;
        LastModified = lastModified;
    }
}
