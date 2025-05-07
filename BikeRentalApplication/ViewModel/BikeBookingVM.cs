using BikeRentalApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BikeRentalApplication.ViewModel
{
    public class BikeBookingVM : INotifyPropertyChanged
    {
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

            ConfirmBookingCommand = new RelayCommand(ExecuteConfirmBooking, CanExecuteConfirmBooking);
            CancelCommand = new RelayCommand(ExecuteCancel);
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

        public ICommand ConfirmBookingCommand { get; }
        public ICommand CancelCommand { get; }
        private bool CanExecuteConfirmBooking(object parameter)
        {
            if (!StartDate.HasValue || !EndDate.HasValue) return false;

            DateTime startDateTime;
            DateTime endDateTime;
            try
            {
                startDateTime = StartDate.Value.Date.AddHours(StartTime);
                endDateTime = EndDate.Value.Date.AddHours(EndTime);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return endDateTime > startDateTime;
        }

        private void ExecuteConfirmBooking(object parameter)
        {
            DateTime finalStartDateTime = StartDate.Value.Date.AddHours(StartTime);
            DateTime finalEndDateTime = EndDate.Value.Date.AddHours(EndTime);

            bool success = DataWorker.CreateBikeBooking(
                _userId,
                BikeToBook.Id,
                finalStartDateTime,
                finalEndDateTime,
                string.IsNullOrWhiteSpace(Comment) ? null : Comment,
                "aktive",
                1300
            );

            System.Diagnostics.Debug.WriteLine($"Booking attempt: User {_userId}, Bike {BikeToBook.Id}, From {finalStartDateTime} To {finalEndDateTime}");


            if (success)
            {
                System.Diagnostics.Debug.WriteLine("Бронирование успешно.");
                OnRequestClose(true);
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "Невозможно забронировать: выбранное время уже занято.",
                    "Ошибка бронирования",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                // OnRequestClose(false); // Возможно, не нужно закрывать окно при ошибке, чтобы пользователь мог исправить данные
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
    }
}