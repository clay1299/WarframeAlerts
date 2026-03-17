namespace WarframeAlerts.Model;
public class VoidFissuresNotification
{
    public List<MongoId> ActiveId { get; set; }
    public int MissionSelectedIndex { get; set; }
    public int RelicSelectedIndex { get; set; }
    public string MissionType { get; set; }
    public string Relic { get; set; }
    public bool? IsHard { get; set; }
    public bool IsActive { get; set; }
}
