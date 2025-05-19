using System.Configuration;
using System.Data;
using System.Windows;
using BikeRentalApplication.Helpers;

namespace BikeRentalApplication;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        //LocalizationManager.Instance.SwitchLanguage("ru-RU");
    }
}

