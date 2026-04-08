using Notifications.Wpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WarframeAlerts.Infrastucture.Command;
using WarframeAlerts.Model;
using WarframeAlerts.Model.DTO;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.ViewModel;

public class VoidFissuresPageViewModel : BaseViewModel
{
    private readonly IWorldStateParser _worldStateParser;
    private readonly IApiTranslator _translator;
    private readonly INotificationService _notificationService;
    private bool _isHard = false;
    private const int _updateInterval = 70;

    private List<string> _relicSource;
    private List<string> _missionTypeSource;
    private string _missionTypeSelected;
    private int _missionTypeSelectedIndex;
    private string _relicSelected;
    private int _relicSelectedIndex;
    private bool? _selectedNotifyWay = null;

    public List<string> MissionTypeSource
    {
        get => _missionTypeSource;
        set
        {
            _missionTypeSource = value;
            OnPropertyChanged();
        }
    }
    public List<string> RelicSource
    {
        get => _relicSource;
        set
        {
            _relicSource = value;
            OnPropertyChanged();
        }
    }

    public string MissionTypeSelected
    {
        get => _missionTypeSelected;
        set
        {
            _missionTypeSelected = value;
            OnPropertyChanged();
        }
    }
    public int MissionTypeSelectedIndex
    {
        get => _missionTypeSelectedIndex;
        set
        {
            _missionTypeSelectedIndex = value;
            OnPropertyChanged();
        }
    }

    public string RelicSelected
    {
        get => _relicSelected;
        set
        {
            _relicSelected = value;
            OnPropertyChanged();
        }
    }

    public int RelicSelectedIndex
    {
        get => _relicSelectedIndex;
        set
        {
            _relicSelectedIndex = value;
            OnPropertyChanged();
        }
    }

    public bool IsTrueChecked
    {
        get => _selectedNotifyWay == true;
        set { if (value) SetMainValue(true); }
    }

    public bool IsFalseChecked
    {
        get => _selectedNotifyWay == false;
        set { if (value) SetMainValue(false); }
    }

    public bool IsNullChecked
    {
        get => _selectedNotifyWay == null;
        set { if (value) SetMainValue(null); }
    }

    public ObservableCollection<VoidFissuresDto> Relics { get; set; }
    public ObservableCollection<VoidFissuresDto> SortedRelic { get; set; }
    public ObservableCollection<VoidFissuresNotification> FissuresNotifications { get; set; }

    public ICommand NormalWayCommand { get; }
    public ICommand SteelWayCommand { get; }
    public ICommand CreateNotify { get; }
    public ICommand DeleteFissureCommand { get; }
    public ICommand ActiveNotificationCommand { get; }

    public VoidFissuresPageViewModel(IWorldStateParser worldStateParser, IApiTranslator translator, INotificationService notificationService)
    {
        _worldStateParser = worldStateParser ?? throw new ArgumentNullException(nameof(worldStateParser));
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        Relics ??= new ObservableCollection<VoidFissuresDto>();
        SortedRelic ??= new ObservableCollection<VoidFissuresDto>();
        FissuresNotifications ??= new ObservableCollection<VoidFissuresNotification>();

        NormalWayCommand = new RelayCommand(c => { _isHard = false; Refresh(); });
        SteelWayCommand = new RelayCommand(c => { _isHard = true; Refresh(); });
        CreateNotify = new RelayCommand(CreateNotification);
        DeleteFissureCommand = new RelayCommand<VoidFissuresNotification>(DeleteFissure);
        ActiveNotificationCommand = new RelayCommand<VoidFissuresNotification>(x => _notificationService.SaveVoidFissuresNotification(x.IsActive, x));

        Init();
    }

    private void DeleteFissure(VoidFissuresNotification notification)
    {
        FissuresNotifications.Remove(notification);
        _notificationService.DeleteVoidFissuresNotification(notification);
    }

    private async void Init()
    {
        await LoadFissuresMissions();
        StartTimer();

        var types = _translator.GetAllMissionTypes().ToList();
        MissionTypeSource = types.Take(1)
                                    .Concat(types.Skip(1).OrderBy(s => s))
                                    .ToList();
        var relics = _translator.GetAllRelics();

        relics.Insert(0, "Any");
        RelicSource = relics;

        LoadVoidFissuresNotification();
    }

    private void LoadVoidFissuresNotification()
    {
        var data = _notificationService.LoadVoidFissuresNotification();
        if (data != null)
        {
            FissuresNotifications.Clear();
            foreach (var item in data)
                FissuresNotifications.Add(item);
        }
    }

    private void ProcessNotification()
    {
        var currentGameIds = Relics.Select(r => r.Id).ToList();
        _notificationService.ClearVoidFissuresActiveIdNotifications(currentGameIds);

        var savedNotifications = _notificationService.LoadVoidFissuresNotification();
        if (savedNotifications == null) return;

        var notifyToCall = Relics.Where(r =>
        savedNotifications.Any(n =>
            n.IsActive &&
            (n.MissionSelectedIndex == 0 || n.MissionType == _translator.MissionTypeTranslate(r.MissionType)) &&
            (n.RelicSelectedIndex == 0 || n.Relic == _translator.RelicTranslate(r.Relic)) &&
            (n.IsHard == null || n.IsHard == r.Hard) &&
            (n.ActiveId == null || !n.ActiveId.Any(a => a.Oid == r.Id.Oid))
        )).ToList();

        foreach (var notify in notifyToCall)
        {
            _notificationService.ShowFissuresNotificationToast(
                _translator.MissionTypeTranslate(notify.MissionType),
                _translator.RelicTranslate(notify.Relic));
            _notificationService.SaveVoidFissuresNotification(notify.Id, new VoidFissuresNotification
            {
                MissionType = notify.MissionType,
                Relic = notify.Relic,
                IsHard = notify.Hard
            });
        }
    }

    private void CreateNotification(object obj)
    {
        if (_notificationService.AddVoidFissuresNotification(new VoidFissuresNotification
        {
            MissionSelectedIndex = MissionTypeSelectedIndex,
            RelicSelectedIndex = RelicSelectedIndex,
            MissionType = MissionTypeSelected,
            Relic = RelicSelected,
            IsHard = _selectedNotifyWay
        }))
        {
            LoadVoidFissuresNotification();
        }
        else
        {
            //to do: сделать оповещение что нельзя одно и то же добавить
        }
    }

    private async void StartTimer()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_updateInterval));
        while (await timer.WaitForNextTickAsync())
        {
            await LoadFissuresMissions();
        }
    }

    private async Task LoadFissuresMissions()
    {
        var data = await _worldStateParser.GetVoidFissuresAsync();

        if (data != null)
        {
            Relics.Clear();
            foreach (var item in data)
                Relics.Add(item);

            ProcessNotification();
        }
        Refresh();
    }

    private void Refresh()
    {
        var sortedItems = Relics
            .Where(item => item.Hard == _isHard)
            .OrderBy(item => item.Relic)
            .ToList();

        SortedRelic.Clear();
        foreach (var item in sortedItems)
        {
            SortedRelic.Add(item);
        }
    }

    private void SetMainValue(bool? newValue)
    {
        _selectedNotifyWay = newValue;
        OnPropertyChanged(nameof(IsTrueChecked));
        OnPropertyChanged(nameof(IsFalseChecked));
        OnPropertyChanged(nameof(IsNullChecked));
    }
}