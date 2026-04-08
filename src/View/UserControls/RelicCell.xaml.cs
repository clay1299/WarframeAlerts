using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;
using WarframeAlerts.Service.Interface;

namespace WarframeAlerts.View.UserControls
{
    public partial class RelicCell : UserControl
    {
        private readonly IApiTranslator _translator;

        CancellationTokenSource _cancelTokenSource;

        public string RelicPath
        {
            get => (string)GetValue(RelicPathProperty);
            set => SetValue(RelicPathProperty, value);
        }

        public static readonly DependencyProperty RelicPathProperty =
            DependencyProperty.Register(
                nameof(RelicPath),
                typeof(string),
                typeof(RelicCell),
                new PropertyMetadata(null));

        public string Time
        {
            get => (string)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(
                nameof(Time),
                typeof(string),
                typeof(RelicCell),
                new PropertyMetadata(null));

        public string Planet
        {
            get => (string)GetValue(PlanetProperty);
            set => SetValue(PlanetProperty, value);
        }

        public static readonly DependencyProperty PlanetProperty =
            DependencyProperty.Register(
                nameof(Planet),
                typeof(string),
                typeof(RelicCell),
                new PropertyMetadata(null));

        public string MissionType
        {
            get => (string)GetValue(MissionTypeProperty);
            set => SetValue(MissionTypeProperty, value);
        }

        public static readonly DependencyProperty MissionTypeProperty =
            DependencyProperty.Register(
                nameof(MissionType),
                typeof(string),
                typeof(RelicCell),
                new PropertyMetadata(null, MissionTypeChanged));

        public string RelicType
        {
            get => (string)GetValue(RelicTypeProperty);
            set => SetValue(RelicTypeProperty, value);
        }

        public static readonly DependencyProperty RelicTypeProperty =
            DependencyProperty.Register(
                nameof(RelicType),
                typeof(string),
                typeof(RelicCell),
                new PropertyMetadata(null, RelicTypeChanged));

        public DateTime ExpiryTime
        {
            get => (DateTime)GetValue(ExpiryTimeProperty);
            set => SetValue(ExpiryTimeProperty, value);
        }

        public static readonly DependencyProperty ExpiryTimeProperty =
            DependencyProperty.Register(
                nameof(ExpiryTime),
                typeof(DateTime),
                typeof(RelicCell),
                new PropertyMetadata(OnExpiryTimeChanged));


        private static readonly Dictionary<string, string> RelicToPath = new Dictionary<string, string>
        {
            {"VoidT1", "/Resources/Image/lith.png" },
            {"VoidT2", "/Resources/Image/meso.png" },
            {"VoidT3", "/Resources/Image/neo.png" },
            {"VoidT4", "/Resources/Image/acsi.png" },
            {"VoidT5", "/Resources/Image/requviem.png" },
            {"VoidT6", "/Resources/Image/omnia.png" }
        };

        public RelicCell()
        {
            InitializeComponent();

            _translator = App.ServiceProvider.GetRequiredService<IApiTranslator>();

            Unloaded += (s, e) => StopTimer();
        }

        private static void RelicTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RelicCell)d;

            if (e.NewValue is string relic && RelicToPath.TryGetValue(relic, out string path))
            {
                control.RelicPath = path;

                control.RelicType = control._translator.RelicTranslate(relic);
            }
        }

        private static void MissionTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RelicCell)d;

            if (e.NewValue is string missiontype && missiontype.Contains("_"))
            {
                control.MissionType = control._translator.MissionTypeTranslate(missiontype);
            }
        }

        private static void OnExpiryTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RelicCell)d;
            if (e.NewValue is DateTime expiry)
            {
                control.UpdateTime(expiry);
                control.StartTimer(expiry);
            }
        }

        private async void StartTimer(DateTime expiry)
        {
            StopTimer();

            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;

            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
                while (await timer.WaitForNextTickAsync(token))
                {
                    UpdateTime(expiry);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void StopTimer()
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();
            _cancelTokenSource = null;
        }

        private void UpdateTime(DateTime expiry)
        {
            string result = "";
            TimeSpan timeLeft = expiry - DateTime.Now;

            if (timeLeft > TimeSpan.Zero)
            {
                result += $"{timeLeft:hh\\:mm\\:ss}";
            }
            else
            {
                result += "00:00:00";
            }

            Time = result;
        }
    }
}