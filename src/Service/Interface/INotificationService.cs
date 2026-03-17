using Notifications.Wpf.Core;
using WarframeAlerts.Model;

namespace WarframeAlerts.Service.Interface;
public interface INotificationService
{
    void SetInfinitiNotifications(bool value);
    void ShowFissuresNotificationToast(string? missionType, string? relic);
    bool AddVoidFissuresNotification(VoidFissuresNotification notification);
    void SaveVoidFissuresNotification(MongoId id, VoidFissuresNotification not);
    void SaveVoidFissuresNotification(bool isActive, VoidFissuresNotification not);
    void ClearVoidFissuresActiveIdNotifications(List<MongoId> currentActiveIds);
    List<VoidFissuresNotification>? LoadVoidFissuresNotification();
    void DeleteVoidFissuresNotification(VoidFissuresNotification notification);
}
