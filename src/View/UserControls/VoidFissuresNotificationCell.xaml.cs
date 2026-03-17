using System.Windows;
using System.Windows.Controls;

namespace WarframeAlerts.View.UserControls;
public partial class VoidFissuresNotificationCell : UserControl
{
    public string MissionType
    {
        get => (string)GetValue(MissionTypeProperty);
        set => SetValue(MissionTypeProperty, value);
    }

    public string Relic
    {
        get => (string)GetValue(RelicProperty);
        set => SetValue(RelicProperty, value);
    }

    public string PathIcon
    {
        get => (string)GetValue(PathIconProperty);
        set => SetValue(PathIconProperty, value);
    }

    public bool? IsHard
    {
        get => (bool?)GetValue(IsHardProperty);
        set => SetValue(IsHardProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly DependencyProperty MissionTypeProperty =
        DependencyProperty.Register(
            nameof(MissionType),
            typeof(string),
            typeof(VoidFissuresNotificationCell));

    public static readonly DependencyProperty RelicProperty =
        DependencyProperty.Register(
            nameof(Relic),
            typeof(string),
            typeof(VoidFissuresNotificationCell));

    public static readonly DependencyProperty PathIconProperty =
        DependencyProperty.Register(
            nameof(PathIcon),
            typeof(string),
            typeof(VoidFissuresNotificationCell),
            new PropertyMetadata("/Resources/Image/infinity.png"));

    public static readonly DependencyProperty IsHardProperty =
        DependencyProperty.Register(
            nameof(IsHard),
            typeof(bool?),
            typeof(VoidFissuresNotificationCell),
            new PropertyMetadata(null, IsHardChanged));

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(
            nameof(IsActive),
            typeof(bool),
            typeof(VoidFissuresNotificationCell),
            new PropertyMetadata(true));

    private static void IsHardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (VoidFissuresNotificationCell)d;
        if(e.NewValue is bool ishard)
        {
            if(ishard) control.PathIcon = "/Resources/Image/steel_path.png";
            else control.PathIcon = "/Resources/Image/normal_path.png";
        }
    }

    public VoidFissuresNotificationCell()
    {
        InitializeComponent();
    }
}
