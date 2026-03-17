using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using View.WarframeAlerts;
using WarframeAlerts.Service;
using WarframeAlerts.Service.Interface;
using WarframeAlerts.View.Pages;
using WarframeAlerts.ViewModel;

namespace WarframeAlerts;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        SettingsService.ConfigureSettings();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Closing += (s, e) =>
        {
            e.Cancel = true;
            MainWindow.Hide();
        };

        mainWindow.Show();

    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IWorldStateParser, WorldStateParser>();
        services.AddSingleton<IApiTranslator, ApiTranslator>();
        services.AddSingleton<INotificationService, NotificationService>();


        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<MainWindow>(services =>
        {
            var mainWindow = new MainWindow();
            mainWindow.DataContext = services.GetRequiredService<MainWindowViewModel>();
            return mainWindow;
        });

        services.AddSingleton<VoidFissuresPageViewModel>();

        services.AddSingleton<VoidFissuresPage>(services =>
        {
            var page = new VoidFissuresPage();
            page.DataContext = services.GetRequiredService<VoidFissuresPageViewModel>();
            return page;
        });

        services.AddSingleton<SettingsPageViewModel>();

        services.AddSingleton<SettingsPage>(services =>
        {
            var page = new SettingsPage();
            page.DataContext = services.GetRequiredService<SettingsPageViewModel>();
            return page;
        });
    }
}
