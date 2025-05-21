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
using System.Globalization;
using System.Collections.Generic; 

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
            public bool IsPaid => Booking.IsPaid;
            public string PaymentStatus => IsPaid ? "Оплачен" : "Не оплачен";
            public string FormattedPrice => Price > 0 ? string.Format(new CultureInfo("ru-BY"), "{0:C}", Price) : "Бесплатно";
            public Visibility PriceVisibility => Price > 0 ? Visibility.Visible : Visibility.Collapsed;

            public Visibility CancelButtonVisibility
            {
                get
                {
                    return (BookingStatus == "Активно" || BookingStatus == "Забронировано") && EndDateTime > DateTime.Now
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

        private List<DisplayableBookingItem> _allLoadedBookings = new List<DisplayableBookingItem>();

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
                if (_isHistoryEmpty != value)
                {
                    _isHistoryEmpty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsHistoryNotEmpty));
                }
            }
        }
        public bool IsHistoryNotEmpty => !_isHistoryEmpty;
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

        public ObservableCollection<string> PaymentSortOptions { get; }
        public ObservableCollection<string> BookingStatusSortOptions { get; }

        private string _selectedPaymentSortOption;
        public string SelectedPaymentSortOption
        {
            get => _selectedPaymentSortOption;
            set
            {
                if (_selectedPaymentSortOption != value)
                {
                    _selectedPaymentSortOption = value;
                    OnPropertyChanged();
                    ApplySortingAndFiltering();
                }
            }
        }

        private string _selectedBookingStatusSortOption;
        public string SelectedBookingStatusSortOption
        {
            get => _selectedBookingStatusSortOption;
            set
            {
                if (_selectedBookingStatusSortOption != value)
                {
                    _selectedBookingStatusSortOption = value;
                    OnPropertyChanged();
                    ApplySortingAndFiltering();
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplySortingAndFiltering();
                }
            }
        }
        public ICommand ResetFiltersCommand { get; }


        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is AllBookingsWindow)?.Close();
            Application.Current.MainWindow.Show();
        }
        private void OpenAdminBikeWindowMethod()
        {
            AdminBikeWindow adminBikeWindow = new AdminBikeWindow();
            Application.Current.MainWindow = adminBikeWindow;
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is AllBookingsWindow)?.Close();
            Application.Current.MainWindow.Show();
        }
        private void OpenAdminCommentsWindowMethod()
        {
            AdminCommentsWindow adminCommentsWindow = new AdminCommentsWindow();
            Application.Current.MainWindow = adminCommentsWindow;
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is AllBookingsWindow)?.Close();
            Application.Current.MainWindow.Show();
        }

        private RelayCommand openAuthWindow;
        private RelayCommand openAdminBikeWindow;
        private RelayCommand openAdminCommentsWindow;
        public RelayCommand OpenAuthWindow => openAuthWindow ??= new RelayCommand(obj => OpenAuthWindowMethod());
        public RelayCommand OpenAdminBikeWindow => openAdminBikeWindow ??= new RelayCommand(obj => OpenAdminBikeWindowMethod());
        public RelayCommand OpenAdminCommentsWindow => openAdminCommentsWindow ??= new RelayCommand(obj => OpenAdminCommentsWindowMethod());

        public void SetCenterPositionAndOpen(Window window) 
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        public ICommand CancelBookingCommand { get; }
        public ICommand ViewBookingDetailsCommand { get; }
        public ICommand RefreshBookingsCommand { get; }

        public AllBookingsVM()
        {
            DisplayableBookings = new ObservableCollection<DisplayableBookingItem>();

            PaymentSortOptions = new ObservableCollection<string> { "По умолчанию", "Сначала оплаченные", "Сначала неоплаченные" };
            _selectedPaymentSortOption = "По умолчанию";

            BookingStatusSortOptions = new ObservableCollection<string> { "По умолчанию", "Сначала активные", "Сначала забронированные" };
            _selectedBookingStatusSortOption = "По умолчанию";

            _searchText = string.Empty;
            ResetFiltersCommand = new RelayCommand(_ => ResetAllFiltersAndSorts());

            CancelBookingCommand = new RelayCommand(async (p) => await CancelBookingActionAsync(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            ViewBookingDetailsCommand = new RelayCommand((p) => ViewBookingDetailsAction(p as DisplayableBookingItem), (p) => p is DisplayableBookingItem);
            RefreshBookingsCommand = new RelayCommand(async (_) => await LoadActiveBookingsAsync());

            _ = LoadActiveBookingsAsync();
        }

        private async Task LoadActiveBookingsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка всех заказов...";
            _allLoadedBookings.Clear();

            try
            {
                var allBookingEntities = await Task.Run(() => DataWorker.GetAllBookings());

                if (allBookingEntities != null)
                {
                    foreach (var booking in allBookingEntities)
                    {
                        await Task.Run(() => DataWorker.UpdateBookingStatusIfNeeded(booking));
                    }
                    var updatedBookingEntities = await Task.Run(() => DataWorker.GetAllBookings());
                    var relevantBookings = updatedBookingEntities
                        .Where(b => b.BookingStatus == "Активно" || b.BookingStatus == "Забронировано")
                        .ToList();

                    foreach (var booking in relevantBookings)
                    {
                        var bike = await Task.Run(() => DataWorker.GetBikeById(booking.BikeId));
                        var user = await Task.Run(() => DataWorker.GetUserById(booking.UserId));
                        if (bike != null && user != null)
                        {
                            _allLoadedBookings.Add(new DisplayableBookingItem(booking, bike, user));
                        }
                    }
                }
                ApplySortingAndFiltering();
            }
            catch (Exception ex)
            {
                LoadingMessage = "Ошибка при загрузке заказов.";
                _allLoadedBookings.Clear();
                ApplySortingAndFiltering();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            } 
        } 

        private void ApplySortingAndFiltering()
        {
            if (_allLoadedBookings == null) return;

            IEnumerable<DisplayableBookingItem> filteredAndSortedBookings = _allLoadedBookings;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lowerSearchText = SearchText.ToLowerInvariant();
                filteredAndSortedBookings = filteredAndSortedBookings.Where(b => b.UserName.ToLowerInvariant().Contains(lowerSearchText));
            }

            IOrderedEnumerable<DisplayableBookingItem> sortedBookings = null;
            bool primarySortApplied = false;

            if (SelectedBookingStatusSortOption == "Сначала активные")
            {
                sortedBookings = filteredAndSortedBookings.OrderByDescending(b => b.BookingStatus == "Активно");
                primarySortApplied = true;
            }
            else if (SelectedBookingStatusSortOption == "Сначала забронированные")
            {
                sortedBookings = filteredAndSortedBookings.OrderBy(b => b.BookingStatus == "Активно");
                primarySortApplied = true;
            }

            if (SelectedPaymentSortOption == "Сначала оплаченные")
            {
                if (primarySortApplied && sortedBookings != null)
                    sortedBookings = sortedBookings.ThenByDescending(b => b.IsPaid);
                else
                {
                    sortedBookings = filteredAndSortedBookings.OrderByDescending(b => b.IsPaid);
                    primarySortApplied = true;
                }
            }
            else if (SelectedPaymentSortOption == "Сначала неоплаченные")
            {
                if (primarySortApplied && sortedBookings != null)
                    sortedBookings = sortedBookings.ThenBy(b => b.IsPaid);
                else
                {
                    sortedBookings = filteredAndSortedBookings.OrderBy(b => b.IsPaid);
                    primarySortApplied = true;
                }
            }

            if (primarySortApplied && sortedBookings != null)
            {
                sortedBookings = sortedBookings.ThenByDescending(b => b.StartDateTime);
            }
            else if (sortedBookings == null) 
            {
                sortedBookings = filteredAndSortedBookings.OrderByDescending(b => b.StartDateTime);
            }


            DisplayableBookings.Clear();
            if (sortedBookings != null)
            {
                foreach (var item in sortedBookings.ToList()) 
                {
                    DisplayableBookings.Add(item);
                }
            }


            IsHistoryEmpty = !DisplayableBookings.Any();
         
             if (IsHistoryEmpty)
            {
                if (!_allLoadedBookings.Any()) 
                {
                    LoadingMessage = "Нет активных или забронированных заказов для отображения.";
                }
                else 
                {
                    LoadingMessage = "Нет заказов, соответствующих выбранным критериям поиска/фильтрации.";
                }
            }
            else
            {
                LoadingMessage = string.Empty; 
            }
        }

        private void ResetAllFiltersAndSorts()
        {
            bool changed = false;
            if (_selectedBookingStatusSortOption != "По умолчанию")
            {
                _selectedBookingStatusSortOption = "По умолчанию";
                OnPropertyChanged(nameof(SelectedBookingStatusSortOption));
                changed = true;
            }
            if (_selectedPaymentSortOption != "По умолчанию")
            {
                _selectedPaymentSortOption = "По умолчанию";
                OnPropertyChanged(nameof(SelectedPaymentSortOption));
                changed = true;
            }
            if (!string.IsNullOrEmpty(_searchText))
            {
                _searchText = string.Empty;
                OnPropertyChanged(nameof(SearchText));
                changed = true;
            }

            if (changed || DisplayableBookings.Count != _allLoadedBookings.Count)
            {
                ApplySortingAndFiltering();
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
                        var itemInMasterList = _allLoadedBookings.FirstOrDefault(b => b.Id == itemToCancel.Id);
                        if (itemInMasterList != null)
                        {
                            _allLoadedBookings.Remove(itemInMasterList);
                        }
                        ApplySortingAndFiltering();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось отменить бронирование.", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене бронирования: {ex.Message}", "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void ViewBookingDetailsAction(DisplayableBookingItem itemDetails)
        {
            if (itemDetails == null) return;
            MessageBox.Show($"Просмотр деталей для бронирования велосипеда: {itemDetails.BikeName}\n" +
                              $"ID брони: {itemDetails.Id}\n" +
                              $"Пользователь: {itemDetails.UserName}\n" +
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