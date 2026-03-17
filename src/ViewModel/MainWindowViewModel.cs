using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarframeAlerts.Infrastucture.Command;
using WarframeAlerts.Service.Interface;
using WarframeAlerts.View.Pages;

namespace WarframeAlerts.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    private string _text;
    private Page _page;

    public ICommand NavVoidFissuresCommand { get; }
    public ICommand NavSettingsCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand ExitCommand { get; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    public Page ActivePage
    {
        get => _page;
        set
        {
            if (_page != value)
            {
                _page = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel(IWorldStateParser worldStateParser)
    {

        NavVoidFissuresCommand = new RelayCommand(c => ActivePage = App.ServiceProvider.GetRequiredService<VoidFissuresPage>());
        NavSettingsCommand = new RelayCommand(c => ActivePage = App.ServiceProvider.GetRequiredService<SettingsPage>());
        OpenCommand = new RelayCommand(c => Application.Current.MainWindow.Show());
        ExitCommand = new RelayCommand(c => Application.Current.Shutdown());
        ActivePage = App.ServiceProvider.GetRequiredService<VoidFissuresPage>();
    }


}
