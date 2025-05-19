using BikeRentalApplication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public class HistoryVM : INotifyPropertyChanged
    {
        public class DisplayableBookingItem : INotifyPropertyChanged
        {
            public BikeBooking Booking { get; }
            public Bike RentedBike { get; }

            public ICommand OpenAddCommentWindow { get; }

            public DisplayableBookingItem(BikeBooking booking, Bike rentedBike)
            {
                Booking = booking;
                RentedBike = rentedBike ?? new Bike
                {
                    Name = "Неизвестный велосипед",
                    ImagePath = "/Resources/DefaultBike.png",
                    Description = "Описание для неизвестного велосипеда отсутствует.",
                    FullDescription = "Полное описание для неизвестного велосипеда отсутствует."
                };

                OpenAddCommentWindow = new RelayCommand(OpenCommentWindow);
            }

            private void OpenCommentWindow(object obj)
            {
                var commentWindow = new AddCommentWindow
                {
                    DataContext = new AddCommentVM(RentedBike)
                };
                commentWindow.ShowDialog();
            }

            public int Id => Booking.Id;
            public DateTime StartDateTime => Booking.StartDateTime;
            public DateTime EndDateTime => Booking.EndDateTime;
            public string BookingStatus => Booking.BookingStatus;
            public decimal Price => Booking.Price;

            public string BikeName => RentedBike.Name;
            public string BikeImagePath => RentedBike.ImagePath;

            public string FormattedPrice => Price > 0 ? $"{Price:C}" : "Бесплатно";
            public Visibility PriceVisibility => Price > 0 ? Visibility.Visible : Visibility.Collapsed;

            public Visibility CancelButtonVisibility
            {
                get
                {
                    return (BookingStatus == "Активно" || BookingStatus == "Подтверждено") && EndDateTime > DateTime.Now
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<DisplayableBookingItem> _displayableBookings;
        public ObservableCollection<DisplayableBookingItem> DisplayableBookings
        {
            get => _displayableBookings;
            set
            {
                _displayableBookings = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsContentVisible));
            }
        }

        public bool IsContentVisible => !IsLoading;

        private string _loadingMessage;
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isHistoryEmpty;
        public bool IsHistoryEmpty
        {
            get => _isHistoryEmpty;
            set
            {
                _isHistoryEmpty = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshBookingsCommand { get; }

        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is HistoryWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }
        private void OpenMainWindowMethod()
        {
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is HistoryWindow)?
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

        public HistoryVM()
        {
            DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync());

            _ = LoadActiveBookingsAsync();
        }


        private async Task LoadActiveBookingsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка истории заказов...";
            DisplayableBookings.Clear();

            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    LoadingMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
                    IsLoading = false;
                    return;
                }

                var userBookings = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                if (userBookings != null)
                {
                    foreach (var booking in userBookings)
                    {
                        await Task.Run(() => DataWorker.UpdateBookingStatusIfNeeded(booking));
                    }

                    var userBooking = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                    foreach (var booking in userBooking)
                    {
                        await Task.Run(() => DataWorker.UpdateBookingStatusIfNeeded(booking));

                        if (booking.BookingStatus == "Завершено")
                        {
                            var bike = await Task.Run(() => DataWorker.GetBikeById(booking.BikeId));
                            DisplayableBookings.Add(new DisplayableBookingItem(booking, bike));
                        }
                    }
                }

                LoadingMessage = DisplayableBookings.Any()
                    ? string.Empty
                    : "У вас пока нет завершённых заказов.";
                IsHistoryEmpty = !DisplayableBookings.Any();

            }
            catch (Exception ex)
            {
                LoadingMessage = "Ошибка при загрузке заказов.";
                MessageBox.Show($"Произошла ошибка при загрузке истории: {ex.Message}\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
