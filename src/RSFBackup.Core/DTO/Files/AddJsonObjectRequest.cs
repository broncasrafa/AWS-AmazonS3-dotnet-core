using Newtonsoft.Json;

namespace RSFBackup.Core.DTO.Files;

public class AddJsonObjectRequest
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("timesent")]
    public DateTime TimeSent { get; set; }
    [JsonProperty("data")]
    public string Data { get; set; }
}