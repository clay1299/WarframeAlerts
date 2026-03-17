using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WarframeAlerts.Model;
public class ActiveMission
{
    [JsonProperty("_id")]
    public MongoId Id { get; set; }

    public int Region { get; set; }
    public long Seed { get; set; }

    public MongoDate Activation { get; set; }
    public MongoDate Expiry { get; set; }

    public string Node { get; set; }
    public string MissionType { get; set; }
    public string Modifier { get; set; }
    public bool? Hard { get; set; }
}

public class MongoId
{
    [JsonProperty("$oid")]
    public string Oid { get; set; }
}

public class MongoDate
{
    [JsonProperty("$date")]
    public MongoNumber Date { get; set; }
}

public class MongoNumber
{
    [JsonProperty("$numberLong")]
    public string NumberLong { get; set; }
}
