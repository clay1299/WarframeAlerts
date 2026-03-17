using System.Windows;
using System.Windows.Input;
using WarframeAlerts.Infrastucture.Command;
using WarframeAlerts.Model;
using WarframeAlerts.Model.Enum;
using WarframeAlerts.Service;

namespace WarframeAlerts.ViewModel;
public class SettingsPageViewModel : BaseViewModel
{
    private List<Languages> _languages;
    private Languages _languageSelected;
    private bool _isInfinitiNotifications;
    private bool _showSaveMassage;
    private CancellationTokenSource _messageCts;

    public List<Languages> LanguageSource
    {
        get => _languages;
        set
        {
            if(_languages != value)
            {
                _languages = value;
                OnPropertyChanged();
            }
        }
    }

    public Languages LanguageSelected
    {
        get => _languageSelected;
        set
        {
            if(_languageSelected != value)
            {
                _languageSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsInfinitiNotifications
    {
        get => _isInfinitiNotifications;
        set
        {
            if(_isInfinitiNotifications != value)
            {
                _isInfinitiNotifications = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ShowSaveMassage
    {
        get => _showSaveMassage;
        set
        {
            if(_showSaveMassage != value)
            {
                _showSaveMassage = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SaveCommand { get; }

    public SettingsPageViewModel()
    {
        Init();

        SaveCommand = new RelayCommand(Save);

        LoadSettings();
    }

    private void Init()
    {
        LanguageSource = new List<Languages>
        {
            Languages.Russian,
            Languages.English
        };
    }

    private void LoadSettings()
    {
        var settings = SettingsService.LoadSettings();

        LanguageSelected = settings.Language;
        IsInfinitiNotifications = settings.IsInfinitiNotifications;
    }

    private async void Save(object obj)
    {
        _messageCts?.Cancel();
        _messageCts = new CancellationTokenSource();
        var token = _messageCts.Token;

        var settings = new Settings
        {
            Language = LanguageSelected,
            IsInfinitiNotifications = IsInfinitiNotifications
        };
        SettingsService.SaveSettings(settings);
        SettingsService.ConfigureSettings();

        ShowSaveMassage = true;

        try
        {
            await Task.Delay(1000, token);
            ShowSaveMassage = false;
        }
        catch (OperationCanceledException) { }
    }
}
