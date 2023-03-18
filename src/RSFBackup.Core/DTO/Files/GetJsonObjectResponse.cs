namespace RSFBackup.Core.DTO.Files;

public class GetJsonObjectResponse
{
    public Guid Id { get; private set; }
    public DateTime TimeSent { get; private set; }
    public string Data { get; private set; }

    public GetJsonObjectResponse(Guid id, DateTime timeSent, string data)
    {
        Id = id;
        TimeSent = timeSent;
        Data = data;
    }
}
