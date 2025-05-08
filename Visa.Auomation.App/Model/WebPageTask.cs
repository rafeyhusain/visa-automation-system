using System.Text.Json.Serialization;

public class WebPageTask
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("data")]
    public List<WebPageData> Data { get; set; } = new();
}