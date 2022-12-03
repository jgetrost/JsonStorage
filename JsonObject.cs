using System.Text.Json.Serialization;

namespace JsonStorage;

public class JsonObject {

     [JsonPropertyName("id")]
    public Guid ObjectId {get; set;}

    [JsonPropertyName("collection")]
    public string? Collection {get; set;}

    [JsonPropertyName("json")]
    public string? Json {get; set;}
}