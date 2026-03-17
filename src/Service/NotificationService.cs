using Newtonsoft.Json;
using Notifications.Wpf.Core;
using System.IO;
using System.Media;
using WarframeAlerts.Model;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.Service;
public class NotificationService : INotificationService
{
    private const string _notifySoundPath = "Data\\Sounds\\notify.wav";
    private const string _notificationVoidFissuresPath = "Data\\voidFissuresNotification.json";
    private IApiTranslator _translator;
    private TimeSpan expirationTime;

    public NotificationService(IApiTranslator translator)
    {
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));
    }

    public void SetInfinitiNotifications(bool value)
    {
        if (value)
            expirationTime = TimeSpan.MaxValue;
        else
            expirationTime = TimeSpan.FromSeconds(5);
    }

    public void ShowFissuresNotificationToast(string? missionType, string? relic)
    {
        var notificationManager = new NotificationManager();
        notificationManager.ShowAsync(new NotificationContent
        {
            Title = "Notification",
            Message = $"{missionType}\n{relic}",
            Type = NotificationType.Information
        },
        expirationTime: expirationTime);
        var player = new SoundPlayer(_notifySoundPath);
        player.Play();
    }

    public bool AddVoidFissuresNotification(VoidFissuresNotification notification)
    {
        var data = LoadVoidFissuresNotification() ?? new List<VoidFissuresNotification>();
        if (data.Any(n => n.MissionType == notification.MissionType&&
                             n.Relic == notification.Relic &&
                             n.IsHard == notification.IsHard)) 
            return false;

        notification.IsActive = true;

        data.Insert(0, notification);

        File.WriteAllText(_notificationVoidFissuresPath, JsonConvert.SerializeObject(data));

        return true;
    }

    public void SaveVoidFissuresNotification(MongoId id, VoidFissuresNotification not)
    {
        var data = LoadVoidFissuresNotification() ?? new List<VoidFissuresNotification>();
        var notification = data.FirstOrDefault(n =>
                (n.MissionSelectedIndex == 0 || n.MissionType == _translator.MissionTypeTranslate(not.MissionType)) &&
                (n.RelicSelectedIndex == 0 || n.Relic == _translator.RelicTranslate(not.Relic)) &&
                (n.IsHard == null || n.IsHard == not.IsHard));

        if (notification != null)
        {
            notification.ActiveId ??= new List<MongoId>();

            if (!notification.ActiveId.Any(a => a.Oid == id.Oid))
            {
                notification.ActiveId.Add(id);
                File.WriteAllText(_notificationVoidFissuresPath, JsonConvert.SerializeObject(data));
            }
        }
    }

    public void SaveVoidFissuresNotification(bool isActive, VoidFissuresNotification not)
    {
        var data = LoadVoidFissuresNotification() ?? new List<VoidFissuresNotification>();
        var notification = data.FirstOrDefault(n =>
                (n.MissionSelectedIndex == 0 || n.MissionType == not.MissionType) &&
                (n.RelicSelectedIndex == 0 || n.Relic == not.Relic) &&
                (n.IsHard == null || n.IsHard == not.IsHard));

        if (notification != null)
        {
            if(notification.IsActive != isActive)
            {
                notification.IsActive = isActive;
                File.WriteAllText(_notificationVoidFissuresPath, JsonConvert.SerializeObject(data));
            }
        }
    }

    public void ClearVoidFissuresActiveIdNotifications(List<MongoId> currentActiveIds)
    {
        var data = LoadVoidFissuresNotification();
        if (data == null || !data.Any()) return;

        bool isChanged = false;

        var activeOidsSet = currentActiveIds.Select(id => id.Oid).ToHashSet();

        foreach (var notification in data)
        {
            if (notification.ActiveId != null)
            {
                int removedCount = notification.ActiveId.RemoveAll(id => !activeOidsSet.Contains(id.Oid));
                if (removedCount > 0) isChanged = true;
            }
        }

        if (isChanged)
        {
            File.WriteAllText(_notificationVoidFissuresPath, JsonConvert.SerializeObject(data));
        }
    }

    public List<VoidFissuresNotification>? LoadVoidFissuresNotification()
    {
        if (!File.Exists(_notificationVoidFissuresPath))
            return null;
        return JsonConvert.DeserializeObject<List<VoidFissuresNotification>>(File.ReadAllText(_notificationVoidFissuresPath));
    }

    public void DeleteVoidFissuresNotification(VoidFissuresNotification not)
    {
        var data = LoadVoidFissuresNotification();

        if(data != null)
        {
            var deleted = data.FirstOrDefault(n =>
                (n.MissionSelectedIndex == 0 || n.MissionType == not.MissionType) &&
                (n.RelicSelectedIndex == 0 || n.Relic == not.Relic) &&
                (n.IsHard == null || n.IsHard == not.IsHard));

            if(deleted != null)
            {
                data.Remove(deleted);
                File.WriteAllText(_notificationVoidFissuresPath, JsonConvert.SerializeObject(data));
            }
        }

    }
}
