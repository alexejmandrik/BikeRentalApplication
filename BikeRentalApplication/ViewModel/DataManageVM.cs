using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BikeRentalApplication.Model;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public enum BikeSortType
    {
        ByNameAscending,
        ByPriceAscending,
        ByPriceDescending
    }

    public class DataManageVM : INotifyPropertyChanged
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
                    ImagePath = "/Resources/Logo.png",
                    Description = "Описание для неизвестного велосипеда отсутствует.",
                    FullDescription = "Полное описание для неизвестного велосипеда отсутствует."
                };
            }

            public int Id => Booking.Id;
            public DateTime StartDateTime => Booking.StartDateTime;
            public DateTime EndDateTime => Booking.EndDateTime;
            public string BookingStatus => Booking.BookingStatus;
            public decimal Price => Booking.Price;
            public string BikeName => RentedBike.Name;
            public string BikeImagePath => RentedBike.ImagePath;
            public string FormattedPrice => Price > 0 ? string.Format(new CultureInfo("ru-BY"), "{0:C}", Price) : "Бесплатно";
            public Visibility PriceVisibility => Price > 0 ? Visibility.Visible : Visibility.Collapsed;

            public Visibility CancelButtonVisibility
            {
                get
                {
                    return (BookingStatus == "Активно" || BookingStatus == "Подтверждено" || BookingStatus == "Забронировано")
                           && EndDateTime > DateTime.Now
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChangedInner([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private bool _isHistoryEmpty;
        public bool IsHistoryEmpty
        {
            get => _isHistoryEmpty;
            set
            {
                if (_isHistoryEmpty != value)
                {
                    _isHistoryEmpty = value;
                    OnPropertyChanged();
                }
            }
        }


        #region VIEW BIKES OR MY BOOKING
        private bool isBikeSection = true;
        private bool isBookingSection = false;
        public bool IsBikeSection
        {
            get => isBikeSection;
            set
            {
                if (isBikeSection != value)
                {
                    isBikeSection = value;
                    OnPropertyChanged();
                    if (isBikeSection) 
                    {
                        IsBookingSection = false; 
                        RefreshAndSortBikes();
                    }
                }
            }
        }
        public bool IsBookingSection
        {
            get => isBookingSection;
            set
            {
                if (isBookingSection != value)
                {
                    isBookingSection = value;
                    OnPropertyChanged();
                    if (isBookingSection) 
                    {
                        IsBikeSection = false; 
                        _ = LoadActiveBookingsAsync();
                    }
                    OnPropertyChanged(nameof(IsBookingContentVisible));
                }
            }
        }

        public ICommand SwitchCommand { get; }
        private void SwitchMode(object? obj)
        {
            if (IsBikeSection)
            {
                IsBookingSection = true; 
            }
            else
            {
                IsBikeSection = true;
            }
        }
        #endregion

        #region BIKE LIST

        private List<Bike> _masterBikeList;
        private List<Bike> _currentBikeListView;

        public List<Bike> AllBikes
        {
            get { return _currentBikeListView; }
            set
            {
                _currentBikeListView = value;
                NotifyPropertyChanged("AllBikes");
            }
        }

        private BikeSortType _currentSortType = BikeSortType.ByNameAscending;

        public ICommand SortByNameCommand { get; }
        public ICommand SortByPriceAscCommand { get; }
        public ICommand SortByPriceDescCommand { get; }

        private void SortBikes(BikeSortType sortType)
        {
            _currentSortType = sortType;
            if (_masterBikeList == null)
            {
                this.AllBikes = new List<Bike>();
                return;
            }

            IEnumerable<Bike> sortedEnumerable;
            switch (sortType)
            {
                case BikeSortType.ByNameAscending:
                    sortedEnumerable = _masterBikeList.OrderBy(b => b.Name);
                    break;
                case BikeSortType.ByPriceAscending:
                    sortedEnumerable = _masterBikeList.OrderBy(b => b.Price);
                    break;
                case BikeSortType.ByPriceDescending:
                    sortedEnumerable = _masterBikeList.OrderByDescending(b => b.Price);
                    break;
                default:
                    sortedEnumerable = _masterBikeList;
                    break;
            }
            this.AllBikes = sortedEnumerable.ToList();
        }

        private void RefreshAndSortBikes()
        {
            _masterBikeList = DataWorker.GetAllBikes() ?? new List<Bike>();
            SortBikes(_currentSortType);
        }

        public static Bike SelectedBike { get; set; } 
                                                     
        #endregion

        #region BOOKING LIST PROPERTIES AND METHODS
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

        private bool _isLoadingBookings;
        public bool IsLoadingBookings
        {
            get => _isLoadingBookings;
            set
            {
                if (_isLoadingBookings != value)
                {
                    _isLoadingBookings = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsBookingContentVisible));
                }
            }
        }

        public bool IsBookingContentVisible => IsBookingSection && !IsLoadingBookings;

        private string _loadingBookingsMessage;
        public string LoadingBookingsMessage
        {
            get => _loadingBookingsMessage;
            set
            {
                if (_loadingBookingsMessage != value)
                {
                    _loadingBookingsMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CancelBookingCommand { get; }
        public ICommand ViewBookingDetailsCommand { get; }
        public ICommand RefreshBookingsCommand { get; }

        private async Task LoadActiveBookingsAsync()
        {
            IsLoadingBookings = true;
            LoadingBookingsMessage = "Загрузка ваших заказов...";

            if (DisplayableBookings == null)
                DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();
            else
                DisplayableBookings.Clear();

            bool errorOccurred = false;

            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    LoadingBookingsMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
                    errorOccurred = true; 
                    return;
                }

                var userBookingInfos = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                if (userBookingInfos != null)
                {
                    foreach (var booking in userBookingInfos)
                    {
                        await Task.Run(() => DataWorker.UpdateBookingStatusIfNeeded(booking));
                    }

                    var updatedUserBookings = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                    foreach (var booking in updatedUserBookings)
                    {
                        if (booking.BookingStatus == "Активно" || booking.BookingStatus == "Забронировано" || booking.BookingStatus == "Подтверждено")
                        {
                            var bike = await Task.Run(() => DataWorker.GetBikeById(booking.BikeId));
                            DisplayableBookings.Add(new DisplayableBookingItem(booking, bike));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorOccurred = true;
                LoadingBookingsMessage = "Ошибка при загрузке заказов.";
                MessageBox.Show($"Произошла ошибка при загрузке заказов: {ex.Message}\n{ex.StackTrace}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingBookings = false; 

                if (SessionManager.CurrentUser == null || errorOccurred)
                {
                    IsHistoryEmpty = true;
                }
                else
                {
                    IsHistoryEmpty = !DisplayableBookings.Any();
                }
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
                IsLoadingBookings = true; 
                LoadingBookingsMessage = "Отмена бронирования...";
                try
                {
                    BikeBooking bookingToDeleteStub = new BikeBooking { Id = itemToCancel.Id };
                    bool bonusUpdated = DataWorker.SetBunusCounterDown(bookingToDeleteStub, SessionManager.CurrentUser);

                    bool success = await Task.Run(() => DataWorker.DeleteBikeBooking(bookingToDeleteStub));
                    if (success)
                    {
                        MessageBox.Show("Бронирование успешно отменено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadActiveBookingsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось отменить бронирование (возможно, оно уже было удалено или не найдено).", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                        await LoadActiveBookingsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене бронирования: {ex.Message}", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    await LoadActiveBookingsAsync(); 
                }
            }
        }

        private void ViewBookingDetailsAction(DisplayableBookingItem itemDetails)
        {
            if (itemDetails == null) return;
            MessageBox.Show($"Просмотр деталей для бронирования велосипеда: {itemDetails.BikeName}\n" +
                              $"ID брони: {itemDetails.Id}\n" +
                              $"Статус: {itemDetails.BookingStatus}\n" +
                              $"Начало: {itemDetails.StartDateTime:dd.MM.yyyy HH:mm}\n" +
                              $"Окончание: {itemDetails.EndDateTime:dd.MM.yyyy HH:mm}\n" +
                              $"Цена: {itemDetails.FormattedPrice}",
                              "Детали бронирования", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecuteBookingRelatedCommand(object parameter)
        {
            return IsBookingSection && parameter is DisplayableBookingItem && !IsLoadingBookings;
        }
        private bool CanExecuteRefreshBookingCommand(object parameter)
        {
            return IsBookingSection && !IsLoadingBookings;
        }
        #endregion

        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is MainWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private void OpenHistoryWindowMethod()
        {
            HistoryWindow historyWindow = new HistoryWindow();
            Application.Current.MainWindow = historyWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is MainWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private void OpenBikeWindowMethod(Bike bikeToOpen)
        {
            if (bikeToOpen == null)
            {
                MessageBox.Show("Велосипед для открытия не выбран или не передан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BikeWindow bikeWindow = new BikeWindow(bikeToOpen);
            Application.Current.MainWindow = bikeWindow;

            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is MainWindow)?
                .Close();

            Application.Current.MainWindow.Show();
        }


        private RelayCommand openAuthWindow;
        private RelayCommand openHistoryWindow;
        private RelayCommand openBikeBookingWindow;
        private RelayCommand openBikeWindow;

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
        public RelayCommand OpenHistoryWindow
        {
            get
            {
                return openHistoryWindow ?? (openHistoryWindow = new RelayCommand(obj =>
                {
                    OpenHistoryWindowMethod();
                }));
            }
        }

        public RelayCommand OpenBikeBookingWindow
        {
            get
            {
                return openBikeBookingWindow ?? (openBikeBookingWindow = new RelayCommand(obj =>
                {
                    Bike bike = obj as Bike;
                    OpenBikeBookingWindowMethod(bike);
                }));
            }
        }

        public RelayCommand OpenBikeWindow
        {
            get
            {
                return openBikeWindow ?? (openBikeWindow = new RelayCommand(obj =>
                {
                    Bike selectedBikeFromDoubleClick = obj as Bike;
                    if (selectedBikeFromDoubleClick != null)
                    {
                        OpenBikeWindowMethod(selectedBikeFromDoubleClick);
                    }
                    else if (SelectedBike != null)
                    {
                        OpenBikeWindowMethod(SelectedBike);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось определить велосипед для открытия.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }

        private void OpenBikeBookingWindowMethod(Bike bikeToBook)
        {
            if (bikeToBook == null)
            {
                MessageBox.Show("Велосипед для бронирования не выбран или не передан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SessionManager.CurrentUser == null)
            {
                MessageBox.Show("Для бронирования велосипеда необходимо авторизоваться.");
                return;
            }

            int userId = SessionManager.CurrentUser.Id;

            var bikeBookingWindow = new BikeBookingWindow(bikeToBook, userId);

            SetCenterPositionAndOpen(bikeBookingWindow);

            if (bikeBookingWindow.DialogResult == true)
            {
                if (IsBookingSection)
                {
                    _ = LoadActiveBookingsAsync();
                }
                RefreshAndSortBikes();
            }
        }
        public void SetCenterPositionAndOpen(Window window)
        {
            Window ownerWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive && w != window);
            if (ownerWindow == null && Application.Current.MainWindow != window)
            {
                ownerWindow = Application.Current.MainWindow;
            }

            window.Owner = ownerWindow;
            window.WindowStartupLocation = ownerWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        public DataManageVM()
        {
            _masterBikeList = new List<Bike>();
            _currentBikeListView = new List<Bike>();
            _displayableBookings = new ObservableCollection<DisplayableBookingItem>();

            SwitchCommand = new RelayCommand(SwitchMode);

            SortByNameCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByNameAscending));
            SortByPriceAscCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByPriceAscending));
            SortByPriceDescCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByPriceDescending));

            RefreshAndSortBikes();

            CancelBookingCommand = new RelayCommand(async (p) => await CancelBookingActionAsync(p as DisplayableBookingItem), CanExecuteBookingRelatedCommand);
            ViewBookingDetailsCommand = new RelayCommand((p) => ViewBookingDetailsAction(p as DisplayableBookingItem), CanExecuteBookingRelatedCommand);
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync(), CanExecuteRefreshBookingCommand);

            IsHistoryEmpty = true; 
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface ICloseRequestable
    {
        event EventHandler<bool?> RequestClose;
    }
}