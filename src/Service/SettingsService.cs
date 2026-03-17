using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Windows;
using WarframeAlerts.Model;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.Service;
public static class SettingsService
{
    private const string _settingPath = "Data\\settings.json";

    public static void SaveSettings(Settings settings)
    {
        if(LoadSettings().Language != settings.Language)
        {
            System.Diagnostics.Process.Start(Environment.ProcessPath);
            Application.Current.Shutdown();
        }
        File.WriteAllText(_settingPath, JsonSerializer.Serialize(settings));
    }

    public static Settings LoadSettings() =>
        JsonSerializer.Deserialize<Settings>(File.ReadAllText(_settingPath));
    

    public static void ConfigureSettings()
    {
        var settings = LoadSettings();
        var translator = App.ServiceProvider.GetRequiredService<IApiTranslator>();
        var notifications = App.ServiceProvider.GetRequiredService<INotificationService>();

        translator.SetLanguage(settings.Language);
        ChangeLanguage(settings.Language.ToString());

        notifications.SetInfinitiNotifications(settings.IsInfinitiNotifications);
    }

    private static void ChangeLanguage(string langCode)
    {
        ResourceDictionary dict = new ResourceDictionary();

        dict.Source = new Uri($"/Resources/Strings.{langCode}.xaml", UriKind.Relative);

        var mergedDicts = Application.Current.Resources.MergedDictionaries;

        if (mergedDicts.Count > 0)
        {
            mergedDicts.RemoveAt(0);
            mergedDicts.Insert(0, dict);
        }
    }
}
