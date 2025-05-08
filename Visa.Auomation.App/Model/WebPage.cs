using System.Text.Json.Serialization;

public class WebPage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("tasks")]
    public List<WebPageTask> Tasks { get; set; } = new();
}