using BikeRentalApplication.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks; // Для Task
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

            // Прокси-свойства из BikeBooking для удобства привязки
            public int Id => Booking.Id; // ID самого бронирования
            public DateTime StartDateTime => Booking.StartDateTime;
            public DateTime EndDateTime => Booking.EndDateTime;
            public string BookingStatus => Booking.BookingStatus;
            public decimal Price => Booking.Price;

            // Свойства из Bike для отображения
            public string BikeName => RentedBike.Name;
            public string BikeImagePath => RentedBike.ImagePath;

            // Вспомогательные свойства для UI
            public string FormattedPrice => Price > 0 ? $"{Price:C}" : "Бесплатно";
            public Visibility PriceVisibility => Price > 0 ? Visibility.Visible : Visibility.Collapsed;

            public Visibility CancelButtonVisibility
            {
                get
                {
                    // Пример логики: отменить можно только активные или подтвержденные бронирования
                    // и если дата окончания еще не наступила
                    return (BookingStatus == "Активно" || BookingStatus == "Подтверждено") && EndDateTime > DateTime.Now
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }

            // INotifyPropertyChanged, если вдруг эти обертки будут меняться после создания
            // (обычно для этого сценария не требуется, но на всякий случай)
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Коллекция будет содержать экземпляры нашего внутреннего класса DisplayableBookingItem
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

        // Команды для навигации из шапки (если они управляются этим ViewModel)
        public ICommand GoToMainWindowCommand { get; }
        public ICommand OpenBonusesWindowCommand { get; }
        public ICommand OpenMapWindowCommand { get; }
        public ICommand OpenProfileWindowCommand { get; }


        public MyBookingsVM()
        {
            DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();

            // Инициализация команд
            // Параметр команды (object p) будет экземпляром DisplayableBookingItem
            CancelBookingCommand = new RelayCommand(async (p) => await CancelBookingActionAsync(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            ViewBookingDetailsCommand = new RelayCommand((p) => ViewBookingDetailsAction(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync());

            // Команды навигации (примеры, их реализация зависит от вашей системы навигации)
            // GoToMainWindowCommand = new RelayCommand(o => NavigationService.NavigateTo<MainWindow>());
            // OpenProfileWindowCommand = new RelayCommand(o => { /* логика открытия окна профиля */ });

            // Загружаем бронирования при создании ViewModel
            // Вызов асинхронного метода из конструктора. 
            // Лучше использовать "fire and forget" с отловом ошибок внутри или специальный InitializeAsync.
            _ = LoadActiveBookingsAsync();
        }


        private async Task LoadActiveBookingsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка ваших заказов...";
            DisplayableBookings.Clear(); // Очищаем коллекцию ObservableCollection

            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    LoadingMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
                    IsLoading = false;
                    return;
                }

                // Предполагаем, что DataWorker.GetUserBookings возвращает List<BikeBooking>
                // Если DataWorker методы синхронные, их стоит обернуть в Task.Run для асинхронности,
                // чтобы не блокировать UI поток. В идеале, сами методы DataWorker должны быть асинхронными.
                var userBookingInfos = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                if (userBookingInfos != null)
                {
                    foreach (var bookingInfo in userBookingInfos) // bookingInfo это BikeBooking
                    {
                        // Получаем детали велосипеда по BikeId из bookingInfo
                        Bike bikeDetails = await Task.Run(() => DataWorker.GetBikeById(bookingInfo.BikeId));

                        // bikeDetails может быть null, если велосипед не найден
                        // Конструктор DisplayableBookingItem обработает это (предоставит значения по умолчанию)

                        DisplayableBookings.Add(new DisplayableBookingItem(bookingInfo, bikeDetails));
                    }
                }

                if (!DisplayableBookings.Any())
                {
                    LoadingMessage = "У вас пока нет активных заказов.";
                }
                else
                {
                    LoadingMessage = string.Empty; // Очищаем сообщение, если заказы есть
                }
            }
            catch (Exception ex)
            {
                LoadingMessage = "Ошибка при загрузке заказов."; // Краткое сообщение для UI
                // Полное сообщение для отладки/логирования
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
                        await LoadActiveBookingsAsync(); // Assuming this method exists and re-fetches data
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

            // Здесь должна быть логика открытия окна с деталями бронирования.
            // Вы можете передать itemDetails.Booking и itemDetails.RentedBike
            // (или сам itemDetails) в новое окно/ViewModel.
            // Например:
            // var detailsView = new BookingDetailsWindow();
            // var detailsVM = new BookingDetailsViewModel(itemDetails.Booking, itemDetails.RentedBike);
            // detailsView.DataContext = detailsVM;
            // detailsView.Owner = Application.Current.MainWindow; // или текущее активное окно
            // detailsView.ShowDialog();

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