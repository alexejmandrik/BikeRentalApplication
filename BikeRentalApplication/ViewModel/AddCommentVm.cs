using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BikeRentalApplication.Model;

namespace BikeRentalApplication.ViewModel
{
    public class AddCommentVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Bike _bikeToBook;
        public Bike BikeToBook
        {
            get => _bikeToBook;
            set
            {
                _bikeToBook = value;
                OnPropertyChanged();
            }
        }

        private string _bookingComment;
        public string BookingComment
        {
            get => _bookingComment;
            set
            {
                _bookingComment = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmBookingCommand { get; }
        public ICommand CancelCommand { get; }

        public AddCommentVM(Bike selectedBike)
        {
            BikeToBook = selectedBike;
            ConfirmBookingCommand = new RelayCommand(ConfirmBooking);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void ConfirmBooking(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BookingComment))
                {
                    MessageBox.Show("Пожалуйста, введите комментарий.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DataWorker.AddComment(BikeToBook, SessionManager.CurrentUser, BookingComment, false);

                MessageBox.Show("Комментарий успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseWindow(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении комментария: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object obj)
        {
            CloseWindow(obj);
        }

        private void CloseWindow(object obj)
        {
            if (obj is Window window)
            {
                window.Close();
            }
        }
    }
}
