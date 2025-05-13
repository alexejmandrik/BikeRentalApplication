using BikeRentalApplication.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks; 
using System.Windows;
using System.Windows.Input;

namespace BikeRentalApplication.ViewModel
{
    public class MyBookingsVM : INotifyPropertyChanged
    {
        public class DisplayableBookingItem : INotifyPropertyChanged
        {
            public BikeBooking Booking { get; }
            public Bike RentedBike { get; }

            public DisplayableBookingItem(BikeBooking booking, Bike rentedBike)
            {
                Booking = booking;
                RentedBike = rentedBike ?? new Bike
                {
                    Name = "Неизвестный велосипед",
                    ImagePath = "/Resources/DefaultBike.png",
                    Description = "Описание для неизвестного велосипеда отсутствует."
                };
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

        public ICommand CancelBookingCommand { get; }
        public ICommand ViewBookingDetailsCommand { get; }
        public ICommand RefreshBookingsCommand { get; }

        public ICommand GoToMainWindowCommand { get; }
        public ICommand OpenBonusesWindowCommand { get; }
        public ICommand OpenMapWindowCommand { get; }
        public ICommand OpenProfileWindowCommand { get; }


        public MyBookingsVM()
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
            LoadingMessage = "Загрузка ваших заказов...";
            DisplayableBookings.Clear();

            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    LoadingMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
                    IsLoading = false;
                    return;
                }

               
                var userBookingInfos = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                if (userBookingInfos != null)
                {
                    foreach (var bookingInfo in userBookingInfos) 
                    {
                        Bike bikeDetails = await Task.Run(() => DataWorker.GetBikeById(bookingInfo.BikeId));

                        DisplayableBookings.Add(new DisplayableBookingItem(bookingInfo, bikeDetails));
                    }
                }

                if (!DisplayableBookings.Any())
                {
                    LoadingMessage = "У вас пока нет активных заказов.";
                }
                else
                {
                    LoadingMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                LoadingMessage = "Ошибка при загрузке заказов."; 
                MessageBox.Show($"Произошла ошибка при загрузке заказов: {ex.Message}\n{ex.StackTrace}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
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