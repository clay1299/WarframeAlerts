using WarframeAlerts.Model.DTO;

namespace WarframeAlerts.Service.Interface;
public interface IWorldStateParser
{
    Task<string> GetAllJsonAsync();
    Task<List<VoidFissuresDto>> GetVoidFissuresAsync();
}
