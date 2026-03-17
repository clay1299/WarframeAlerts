using Newtonsoft.Json;
using System.Net.Http;
using WarframeAlerts.Model;
using WarframeAlerts.Model.DTO;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.Service;
public class WorldStateParser : IWorldStateParser
{
    private readonly HttpClient _client;
    private readonly IApiTranslator _apiTranslator;

    private const string _url = $"https://api.warframe.com/cdn/worldState.php?";

    public WorldStateParser(IApiTranslator apiTranslator)
    {
        _apiTranslator = apiTranslator ?? throw new ArgumentNullException(nameof(apiTranslator));
        _client = new HttpClient(new HttpClientHandler
        {

        })
        {
            DefaultRequestHeaders = { CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true } }
        };
    }

    public async Task<string> GetAllJsonAsync()
    {
        try
        {
            string content = await _client.GetStringAsync(_url);

            if (content.Trim().StartsWith("{"))
            {
                return content;
            }
            else
            {
                throw new Exception("came not json");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message} \n {ex}");
        }
    }

    public async Task<List<VoidFissuresDto>> GetVoidFissuresAsync()
    {
        try
        {
            string content = await _client.GetStringAsync(_url);

            if (!content.Trim().StartsWith("{"))
                throw new Exception("came not json");

            var data = JsonConvert.DeserializeObject<RootObject>(content);

            var result = new List<VoidFissuresDto>();

            foreach (var mission in data.ActiveMissions)
            {
                var dto = new VoidFissuresDto
                {
                    Id = mission.Id,
                    Activation = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(mission.Activation.Date.NumberLong)).LocalDateTime,
                    Expiry = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(mission.Expiry.Date.NumberLong)).LocalDateTime,
                    MissionName = _apiTranslator.SolNodesTranslate(mission.Node),
                    MissionType = mission.MissionType,
                    Relic = mission.Modifier,
                    Hard = mission.Hard ?? false,
                };
                result.Add(dto);
            }
            return result;
        }
        catch(Exception ex)
        {
            throw new Exception($"{ex.Message} \n {ex}");
        }
    }
}
