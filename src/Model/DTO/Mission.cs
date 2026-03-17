using Newtonsoft.Json;

namespace WarframeAlerts.Model.DTO;
public class Mission
{
    [JsonProperty("value")]
    public string Planet { get; set; }
    [JsonProperty("enemy")]
    public string Enemy { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}
