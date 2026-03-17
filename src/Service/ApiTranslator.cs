using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using WarframeAlerts.Model.DTO;
using WarframeAlerts.Model.Enum;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.Service;
public class ApiTranslator : IApiTranslator
{
    private string _solNodesPath = "Data\\Ru\\solNodes.json";
    private string _relicPath = "Data\\Ru\\relics.json";
    private string _missionTypePath = "Data\\Ru\\missionTypes.json";

    public void SetLanguage(Languages language)
    {
        switch (language)
        {
            case Languages.Russian:
                _solNodesPath = "Data\\Ru\\solNodes.json";
                _relicPath = "Data\\Ru\\relics.json";
                _missionTypePath = "Data\\Ru\\missionTypes.json";
                break;
            case Languages.English:
                _solNodesPath = "Data\\En\\solNodes.json";
                _relicPath = "Data\\En\\relics.json";
                _missionTypePath = "Data\\En\\missionTypes.json";
                break;
        }
    }

    public Mission SolNodesTranslate(string solNodesKey)
    {
        using (StreamReader sr = new StreamReader(_solNodesPath))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == solNodesKey)
                {
                    reader.Read();

                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<Mission>(reader);
                }
            }
        }

        return new Mission { Planet = solNodesKey, Enemy = solNodesKey, Type = solNodesKey };
    }

    public string? RelicTranslate(string relic)
    {
        return GetValueByKey(_relicPath, relic);
    }

    public string? MissionTypeTranslate(string missionType)
    {
        return GetValueByKey(_missionTypePath, missionType);
    }

    public List<string>? GetAllMissionTypes()
    {
        return GetValueByKey(_missionTypePath, GetAllKeys(_missionTypePath));
    }

    public List<string>? GetAllRelics()
    {
        return GetValueByKey(_relicPath, GetAllKeys(_relicPath));
    }

    private string? GetValueByKey(string path, string key)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            JObject data = JObject.Parse(jsonContent);
            return data.SelectToken($"['{key}'].value", errorWhenNoMatch: false)?.ToString();
        }
        catch (Exception) { return null; }
    }

    private List<string>? GetValueByKey(string path, List<string> keys)
    {
        try
        {
            List<string> result = new();
            string jsonContent = File.ReadAllText(path);
            JObject data = JObject.Parse(jsonContent);
            foreach (var key in keys)
            {
                result.Add(data.SelectToken($"['{key}'].value", errorWhenNoMatch: false)?.ToString());
            }
            return result;
        }
        catch (Exception) { return null; }
    }

    public List<string> GetAllKeys(string path)
    {
        JObject obj = JObject.Parse(File.ReadAllText(path));

        return obj.Properties()
                  .Select(p => p.Name)
                  .ToList();
    }

}
