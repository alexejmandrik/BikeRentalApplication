using Microsoft.EntityFrameworkCore;
using BikeRentalApplication.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class BikeVM : INotifyPropertyChanged
{
    private readonly Bike _bikeModel;

    public int Id => _bikeModel?.Id ?? 0;
    public string Name => _bikeModel?.Name;
    public string Description => _bikeModel?.Description;
    public string ImagePath => _bikeModel?.ImagePath;
    public decimal Price => _bikeModel?.Price ?? 0m;

    // ✅ Свойство комментариев
    public List<Comments> Comments { get; }

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
