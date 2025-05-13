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

        // Инициализация языком по умолчанию (например, русским)
        // Это также загрузит начальный словарь ресурсов
        LocalizationManager.Instance.SwitchLanguage("ru-RU");
    }
}

