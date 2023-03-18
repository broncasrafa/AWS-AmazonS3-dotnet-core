namespace RSFBackup.Core.DTO.Files;

public class AddFileResponse
{
    public string BucketName { get; private set; }
    public IList<PreSignedUrlFileResponse> PreSignedUrls { get; private set; }

    public AddFileResponse(string bucketName, IList<PreSignedUrlFileResponse> preSignedUrls)
    {
        BucketName = bucketName;
        PreSignedUrls = preSignedUrls;
    }
}

public class PreSignedUrlFileResponse
{
    public string FileName { get; private set; }
    public string Url { get; private set; }
    public DateTime Expires { get; private set; }

    public PreSignedUrlFileResponse(string fileName, string url, DateTime expires)
    {
        FileName = fileName;
        Url = url;
        Expires = expires;
    }
}
