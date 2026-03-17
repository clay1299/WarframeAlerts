namespace WarframeAlerts.Model.DTO;
public class VoidFissuresDto
{
    public MongoId Id { get; set; }
    public DateTime Activation { get; set; }
    public DateTime Expiry { get; set; }
    public Mission MissionName { get; set; }
    public string MissionType { get; set; }
    public string Relic { get; set; }
    public bool Hard { get; set; }
}