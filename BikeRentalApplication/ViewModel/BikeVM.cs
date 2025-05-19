using Microsoft.EntityFrameworkCore;
using BikeRentalApplication.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BikeRentalApplication.View;
using System.Windows;

public class BikeVM : INotifyPropertyChanged
{
    private readonly Bike _bikeModel;

    public int Id => _bikeModel?.Id ?? 0;
    public string Name => _bikeModel?.Name;
    public string Description => _bikeModel?.Description;
    public string FullDescription => _bikeModel?.FullDescription;
    public string ImagePath => _bikeModel?.ImagePath;
    public decimal Price => _bikeModel?.Price ?? 0m;

    public List<Comments> Comments { get; }

    private void OpenAuthWindowMethod()
    {
        AuthWindow authWindow = new AuthWindow();
        Application.Current.MainWindow = authWindow;

        Application.Current.Windows
        .OfType<Window>()
        .FirstOrDefault(w => w is BikeWindow)?
        .Close();

        Application.Current.MainWindow.Show();
    }
    private void OpenMainWindowMethod()
    {
        MainWindow mainWindow = new MainWindow();
        Application.Current.MainWindow = mainWindow;

        Application.Current.Windows
        .OfType<Window>()
        .FirstOrDefault(w => w is BikeWindow)?
        .Close();

        Application.Current.MainWindow.Show();
    }

    private RelayCommand openAuthWindow;
    private RelayCommand openMainWindow;

    public RelayCommand OpenAuthWindow
    {
        get
        {
            return openAuthWindow ?? new RelayCommand(obj =>
            {
                OpenAuthWindowMethod();
            });
        }
    }
    public RelayCommand OpenMainWindow
    {
        get
        {
            return openMainWindow ?? new RelayCommand(obj =>
            {
                OpenMainWindowMethod();
            });
        }
    }

    public BikeVM(Bike bike)
    {
        _bikeModel = bike ?? throw new ArgumentNullException(nameof(bike), "Модель велосипеда не может быть null для BikeVM.");

        Comments = DataWorker.GetCommentsByBikeId(bike.Id);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
