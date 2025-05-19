using BikeRentalApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BikeRentalApplication.ViewModel
{
    public class BikeBookingVM : INotifyPropertyChanged
    {
        public int AvailablePoints => SessionManager.CurrentUser?.BonusCounter ?? 0;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int _userId;

        public BikeBookingVM(Bike bike, int userId)
        {
            BikeToBook = bike ?? throw new ArgumentNullException(nameof(bike));
            _userId = userId;

            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            StartTime = 9;
            EndTime = 10;

            ConfirmBookingCommand = new RelayCommand(ExecuteConfirmBooking);
            CancelCommand = new RelayCommand(ExecuteCancel);

            UpdateTotalCost();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime? _startDate;
        private DateTime? _endDate;
        private int _startTime;
        private int _endTime;
        private string _comment;
        private decimal _totalCost;

        public Bike BikeToBook { get; }
        public List<int> AvailableHours { get; } = Enumerable.Range(0, 24).ToList();

        public event EventHandler<bool?> RequestClose;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (!Equals(_startDate, value))
                {
                    _startDate = value;
                    OnPropertyChanged();
                    (ConfirmBookingCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    UpdateTotalCost();
                }
            }
        }

        public int StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged();
                    (ConfirmBookingCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    UpdateTotalCost();

                    if (StartDate.HasValue && EndDate.HasValue && StartDate.Value.Date == EndDate.Value.Date && value >= EndTime)
                    {
                        EndTime = Math.Min(23, value + 1);
                    }
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (!Equals(_endDate, value))
                {
                    _endDate = value;
                    OnPropertyChanged();
                    (ConfirmBookingCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    UpdateTotalCost();
                }
            }
        }

        public int EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged();
                    (ConfirmBookingCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    UpdateTotalCost();
                }
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalCost
        {
            get => _totalCost;
            private set
            {
                if (_totalCost != value)
                {
                    _totalCost = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _usePoints;
        public bool UsePoints
        {
            get => _usePoints;
            set
            {
                if (_usePoints != value)
                {
                    _usePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }



        public ICommand ConfirmBookingCommand { get; }
        public ICommand CancelCommand { get; }

        private bool CanExecuteConfirmBooking(object parameter)
        {
            if (!StartDate.HasValue || !EndDate.HasValue) return false;

            try
            {
                var start = StartDate.Value.Date.AddHours(StartTime);
                var end = EndDate.Value.Date.AddHours(EndTime);

                if (end <= start) return false;

                var maxDuration = TimeSpan.FromDays(7);
                return (end - start) <= maxDuration;
            }
            catch
            {
                return false;
            }
        }


        private void ExecuteConfirmBooking(object parameter)
        {
            ErrorMessage = string.Empty;

            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ErrorMessage = "Выберите дату начала и окончания.";
                return;
            }

            DateTime finalStartDateTime = StartDate.Value.Date.AddHours(StartTime);
            DateTime finalEndDateTime = EndDate.Value.Date.AddHours(EndTime);

            if (finalEndDateTime <= finalStartDateTime)
            {
                ErrorMessage = "Время окончания должно быть позже времени начала.";
                return;
            }

            if ((finalEndDateTime - finalStartDateTime).TotalDays > 7)
            {
                ErrorMessage = "Нельзя бронировать велосипед более чем на 7 дней.";
                return;
            }

            decimal totalPrice = TotalCost;
            string success;

            if (UsePoints)
            {
                int requiredPoints = (int)(totalPrice * 10);
                if (SessionManager.CurrentUser.BonusCounter < requiredPoints)
                {
                    ErrorMessage = "Недостаточно бонусов для оплаты.";
                    return;
                }

                success = DataWorker.CreateBikeBooking(
                    _userId,
                    BikeToBook.Id,
                    finalStartDateTime,
                    finalEndDateTime,
                    string.IsNullOrWhiteSpace(Comment) ? null : Comment,
                    "Забронировано",
                    totalPrice,
                    true);

                if (success == "Успешно забронировано!")
                {
                    bool subtracted = DataWorker.SetBonusCounterDownBooking(requiredPoints, SessionManager.CurrentUser);
                    if (subtracted)
                    {
                        MessageBox.Show("Успешно забронировано!\n Oплачено бонусами");
                        OnPropertyChanged(nameof(AvailablePoints));
                        ErrorMessage = string.Empty;
                        OnRequestClose(true);
                    }
                    else
                    {
                        ErrorMessage = "Ошибка при списании бонусов.";
                    }
                }
                else
                {
                    ErrorMessage = "Ошибка бронирования: " + success;
                }
            }
            else
            {
                success = DataWorker.CreateBikeBooking(
                    _userId,
                    BikeToBook.Id,
                    finalStartDateTime,
                    finalEndDateTime,
                    string.IsNullOrWhiteSpace(Comment) ? null : Comment,
                    "Забронировано",
                    totalPrice,
                    false);

                bool addBonus = DataWorker.SetBunusCounterUp(SessionManager.CurrentUser, totalPrice);

                if (success == "Успешно забронировано!" && addBonus)
                {
                    MessageBox.Show("Успешно забронировано!");
                    OnPropertyChanged(nameof(AvailablePoints));
                    ErrorMessage = string.Empty;
                    OnRequestClose(true);
                }
                else
                {
                    ErrorMessage = "Ошибка бронирования: " + success;
                }
            }
        }


        private void ExecuteCancel(object parameter)
        {
            OnRequestClose(false);
        }

        protected virtual void OnRequestClose(bool? dialogResult)
        {
            RequestClose?.Invoke(this, dialogResult);
        }

        private void UpdateTotalCost()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                try
                {
                    var start = StartDate.Value.Date.AddHours(StartTime);
                    var end = EndDate.Value.Date.AddHours(EndTime);

                    if (end > start)
                    {
                        TimeSpan duration = end - start;
                        int totalHours = (int)Math.Ceiling(duration.TotalHours);
                        TotalCost = totalHours * BikeToBook.Price;
                        return;
                    }
                }
                catch
                {
                    // ignore parsing issues
                }
            }

            TotalCost = 0;
        }
    }
}
