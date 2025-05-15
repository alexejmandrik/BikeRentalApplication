using BikeRentalApplication.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks; 
using System.Windows;
using System.Windows.Input;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public class AllBookingsVM : INotifyPropertyChanged
    {
        public class DisplayableBookingItem : INotifyPropertyChanged
        {
            public BikeBooking Booking { get; }
            public Bike RentedBike { get; }
            public User BookingUser { get; }

            public DisplayableBookingItem(BikeBooking booking, Bike rentedBike, User user)
            {
                Booking = booking;
                RentedBike = rentedBike;
                BookingUser = user;
            }


            public string UserName => BookingUser.UserName;

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

        private void OpenAdminCommentsWindowMethod()
        {
            AdminCommentsWindow adminCommentsWindow = new AdminCommentsWindow();
            SetCenterPositionAndOpen(adminCommentsWindow);
        }

        private RelayCommand openAdminCommentsWindow;
        public RelayCommand OpenAdminCommentsWindow
        {
            get
            {
                return openAdminCommentsWindow ?? new RelayCommand(obj =>
                {
                    OpenAdminCommentsWindowMethod();
                });
            }
        }

        public void SetCenterPositionAndOpen(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        public ICommand CancelBookingCommand { get; }
        public ICommand ViewBookingDetailsCommand { get; }
        public ICommand RefreshBookingsCommand { get; }

        public ICommand GoToMainWindowCommand { get; }
        public ICommand OpenBonusesWindowCommand { get; }
        public ICommand OpenMapWindowCommand { get; }
        public ICommand OpenProfileWindowCommand { get; }


        public AllBookingsVM()
        {
            DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();

          
            CancelBookingCommand = new RelayCommand(async (p) => await CancelBookingActionAsync(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            ViewBookingDetailsCommand = new RelayCommand((p) => ViewBookingDetailsAction(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync());

            _ = LoadActiveBookingsAsync();
        }


        private async Task LoadActiveBookingsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка всех заказов...";
            DisplayableBookings.Clear();

            try
            {
                var allBooking = await Task.Run(() => DataWorker.GetAllBookings());

                if (allBooking != null)
                {
                    foreach (var booking in allBooking)
                    {
                        await Task.Run(() => DataWorker.UpdateBookingStatusIfNeeded(booking));
                    }
                    var allBookings = await Task.Run(() => DataWorker.GetAllBookings());
                    var filteredBookings = allBookings
                        .Where(b => b.BookingStatus == "Активно" || b.BookingStatus == "Забронировано")
                        .ToList();

                    foreach (var booking in filteredBookings)
                    {
                        var bike = await Task.Run(() => DataWorker.GetBikeById(booking.BikeId));
                        var user = await Task.Run(() => DataWorker.GetUserById(booking.UserId));

                        DisplayableBookings.Add(new DisplayableBookingItem(booking, bike, user));
                    }
                }

                LoadingMessage = DisplayableBookings.Any()
                    ? string.Empty
                    : "Нет заказов для отображения.";

                IsHistoryEmpty = !DisplayableBookings.Any();
            }
            catch (Exception ex)
            {
                LoadingMessage = "Ошибка при загрузке заказов.";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }




        private async Task CancelBookingActionAsync(DisplayableBookingItem itemToCancel)
        {
            if (itemToCancel == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите отменить бронирование для {itemToCancel.BikeName} " +
                                           $"(ID: {itemToCancel.Id})?",
                                           "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                LoadingMessage = "Отмена бронирования...";
                try
                {
                    BikeBooking bookingToDeleteStub = new BikeBooking { Id = itemToCancel.Id };

                    bool success = await Task.Run(() => DataWorker.DeleteBikeBooking(bookingToDeleteStub));

                    if (success)
                    {
                        MessageBox.Show("Бронирование успешно отменено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadActiveBookingsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось отменить бронирование (возможно, оно уже было удалено или не найдено).", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене бронирования: {ex.Message}", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                    LoadingMessage = string.Empty;
                }
            }
        }

        private void ViewBookingDetailsAction(DisplayableBookingItem itemDetails)
        {
            if (itemDetails == null) return;
            MessageBox.Show($"Просмотр деталей для бронирования велосипеда: {itemDetails.BikeName}\n" +
                              $"ID брони: {itemDetails.Id}\n" +
                              $"Статус: {itemDetails.BookingStatus}\n" +
                              $"Начало: {itemDetails.StartDateTime:g}\n" +
                              $"Окончание: {itemDetails.EndDateTime:g}",
                              "Детали бронирования", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}