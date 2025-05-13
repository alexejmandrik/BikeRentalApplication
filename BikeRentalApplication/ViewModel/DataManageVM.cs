using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChangedInner([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private string bikeName;
        public string BikeName { get => bikeName; set { bikeName = value; OnPropertyChanged(); } }

        private string bikeDescription;
        public string BikeDescription { get => bikeDescription; set { bikeDescription = value; OnPropertyChanged(); } }

        private string bikeImagePath;
        public string BikeImagePath { get => bikeImagePath; set { bikeImagePath = value; OnPropertyChanged(); } }

        private decimal bikePrice;
        public decimal BikePrice { get => bikePrice; set { bikePrice = value; OnPropertyChanged(); } }

        public static Bike SelectedBike { get; set; }
        #endregion

        #region BOOKING LIST PROPERTIES AND METHODS
        private ObservableCollection<DisplayableBookingItem> displayableBookings;
        public ObservableCollection<DisplayableBookingItem> DisplayableBookings
        {
            get => displayableBookings;
            set
            {
                displayableBookings = value;
                OnPropertyChanged();
            }
        }

        private bool isLoadingBookings;
        public bool IsLoadingBookings
        {
            get => isLoadingBookings;
            set
            {
                isLoadingBookings = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBookingContentVisible));
            }
        }

        public bool IsBookingContentVisible => IsBookingSection && !IsLoadingBookings;

        private string loadingBookingsMessage;
        public string LoadingBookingsMessage
        {
            get => loadingBookingsMessage;
            set
            {
                loadingBookingsMessage = value;
                OnPropertyChanged();
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

            try
            {
                if (SessionManager.CurrentUser == null)
                {
                    LoadingBookingsMessage = "Пользователь не авторизован. Пожалуйста, войдите в систему.";
                    IsLoadingBookings = false;
                    return;
                }

                var userBookingInfos = await Task.Run(() => DataWorker.GetUserBookings(SessionManager.CurrentUser));

                if (userBookingInfos != null)
                {
                    foreach (var bookingInfo in userBookingInfos)
                    {
                        Bike? bikeDetails = await Task.Run(() => DataWorker.GetBikeById(bookingInfo.BikeId));
                        DisplayableBookings.Add(new DisplayableBookingItem(bookingInfo, bikeDetails));
                    }
                }

                if (!DisplayableBookings.Any())
                {
                    LoadingBookingsMessage = "У вас пока нет активных заказов.";
                }
                else
                {
                    LoadingBookingsMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                LoadingBookingsMessage = "Ошибка при загрузке заказов.";
                MessageBox.Show($"Произошла ошибка при загрузке заказов: {ex.Message}\n{ex.StackTrace}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingBookings = false;
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
                    IsLoadingBookings = false;
                    LoadingBookingsMessage = string.Empty;
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

        private bool CanExecuteBookingRelatedCommand(object parameter)
        {
            return IsBookingSection && parameter is DisplayableBookingItem && !IsLoadingBookings;
        }
        private bool CanExecuteRefreshBookingCommand(object parameter)
        {
            return IsBookingSection && !IsLoadingBookings;
        }
        #endregion


        #region WINDOW MANAGEMENT & MISC ACTIONS
        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            SetCenterPositionAndOpen(authWindow);
            if (IsBookingSection && SessionManager.CurrentUser != null)
            {
                _ = LoadActiveBookingsAsync();
            }
            // AllBikes = DataWorker.GetAllBikes(); // OLD
            RefreshAndSortBikes(); // NEW
        }

        private RelayCommand openBikeBookingWindow;

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
        private void SwitchToMyBookingsViewMethod()
        {
            if (!IsBookingSection)
            {
                IsBookingSection = true;
            }
            else
            {
                _ = LoadActiveBookingsAsync();
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
                MessageBox.Show("Для бронирования велосипеда необходимо авторизоваться.", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                OpenAuthWindowMethod();
                if (SessionManager.CurrentUser == null) return;
            }

            int userId = SessionManager.CurrentUser.Id;
            BikeBookingVM bookingVM = new BikeBookingVM(bikeToBook, userId);
            BikeBookingWindow bikeBookingWindow = new BikeBookingWindow();
            bikeBookingWindow.DataContext = bookingVM;

            if (bookingVM.GetType().GetEvent("RequestClose") is System.Reflection.EventInfo requestCloseEvent)
            {
                EventHandler<bool?> requestCloseHandler = null;
                requestCloseHandler = (s, dialogResult) => {
                    requestCloseEvent.RemoveEventHandler(bookingVM, requestCloseHandler);
                    bikeBookingWindow.DialogResult = dialogResult;
                };
                requestCloseEvent.AddEventHandler(bookingVM, requestCloseHandler);
            }

            SetCenterPositionAndOpen(bikeBookingWindow);

            if (bikeBookingWindow.DialogResult == true)
            {
                if (IsBookingSection)
                {
                    _ = LoadActiveBookingsAsync();
                }
                // AllBikes = DataWorker.GetAllBikes(); // OLD 
                RefreshAndSortBikes(); // NEW:
            }
        }
        public void SetCenterPositionAndOpen(Window window)
        {
            Window ownerWindow = Application.Current.MainWindow;
            if (ownerWindow != null && ownerWindow == window)
            {
                ownerWindow = null;
            }
            window.Owner = ownerWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        #endregion

        #region COMMANDS
        private RelayCommand _openAuthWindow;
        public RelayCommand OpenAuthWindow => _openAuthWindow ??= new RelayCommand(obj => OpenAuthWindowMethod());

        private RelayCommand _openMyBookingsViewCommand;
        public RelayCommand OpenMyBookingsViewCommand => _openMyBookingsViewCommand ??= new RelayCommand(obj => SwitchToMyBookingsViewMethod());
        #endregion

        public DataManageVM()
        {
            _currentBikeListView = new List<Bike>();
            _masterBikeList = new List<Bike>();

            SwitchCommand = new RelayCommand(SwitchMode);

            SortByNameCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByNameAscending));
            SortByPriceAscCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByPriceAscending));
            SortByPriceDescCommand = new RelayCommand(_ => SortBikes(BikeSortType.ByPriceDescending));
            
         
            RefreshAndSortBikes();

            DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();

            CancelBookingCommand = new RelayCommand(async (p) => await CancelBookingActionAsync(p as DisplayableBookingItem), CanExecuteBookingRelatedCommand);
            ViewBookingDetailsCommand = new RelayCommand((p) => ViewBookingDetailsAction(p as DisplayableBookingItem), CanExecuteBookingRelatedCommand);
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync(), CanExecuteRefreshBookingCommand);

            if (IsBookingSection) 
            {
                _ = LoadActiveBookingsAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}