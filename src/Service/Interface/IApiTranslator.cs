using WarframeAlerts.Model.DTO;
using WarframeAlerts.Model.Enum;

namespace WarframeAlerts.Service.Interface;

public interface IApiTranslator
{
    void SetLanguage(Languages language);
    Mission SolNodesTranslate(string solNodes);
    string? RelicTranslate(string relic);
    string? MissionTypeTranslate(string missionType);
    List<string>? GetAllMissionTypes();
    List<string>? GetAllRelics();
}