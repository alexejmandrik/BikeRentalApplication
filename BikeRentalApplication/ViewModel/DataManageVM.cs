using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BikeRentalApplication.View;
using BikeRentalApplication.Model;


namespace BikeRentalApplication.ViewModel
{
    public class DataManageVM : INotifyPropertyChanged
    {
        private List<Bike> allBikes = DataWorker.GetAllBikes();
        public List<Bike> AllBikes
        {
            get { return allBikes; }
            set
            {
                allBikes = value;
                NotifyPropertyChanged("AllBikes");
            }
        }

        #region METHODS TO OPEN WINDOW
        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            SetCenterPositionAndOpen(authWindow);
        }

        private void OpenMyBookingsWindowMethod()
        {
            MyBookingsWindow myBookingsWindow = new MyBookingsWindow();
            SetCenterPositionAndOpen(myBookingsWindow);
        }
        private void OpenBikeBookingWindowMethod(Bike bikeToBook)
        {
            if (bikeToBook == null)
            {
                MessageBox.Show("Велосипед для бронирования не выбран или не передан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int userId = SessionManager.CurrentUser.Id;

            BikeBookingVM bookingVM = new BikeBookingVM(bikeToBook, userId);

            BikeBookingWindow bikeBookingWindow = new BikeBookingWindow();

            bikeBookingWindow.DataContext = bookingVM;

            EventHandler<bool?> requestCloseHandler = null;
            requestCloseHandler = (s, dialogResult) => {
                bookingVM.RequestClose -= requestCloseHandler; 
                bikeBookingWindow.DialogResult = dialogResult;
                // Если окно открыто через ShowDialog(), оно закроется само.
                // Если через Show(), то можно добавить bikeBookingWindow.Close();
            };
            if (bookingVM.GetType().GetEvent("RequestClose") != null)
            {
                bookingVM.RequestClose += requestCloseHandler;
            }


            SetCenterPositionAndOpen(bikeBookingWindow);
        }
        #endregion

        #region COMMANDS TO OPEN WINDOWS
        private RelayCommand openAuthWindow;
        private RelayCommand openBikeBookingWindow;
        private RelayCommand openMyBookingsWindow;
        public RelayCommand OpenAuthWindow
        {
            get
            {
                return openAuthWindow ?? (openAuthWindow = new RelayCommand(obj =>
                {
                    OpenAuthWindowMethod();
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
        public RelayCommand OpenMyBookingsWindow
        {
            get
            {
                return openMyBookingsWindow ?? (openMyBookingsWindow = new RelayCommand(obj =>
                {
                    OpenMyBookingsWindowMethod();
                }));
            }
        }
        #endregion

        public static Bike SelectedBike { get; set; }
        public static User SelectedUser { get; set; }

        private string _bikeName;
        public string BikeName { get => _bikeName; set { _bikeName = value; NotifyPropertyChanged(nameof(BikeName)); } }

        private string _bikeDescription;
        public string BikeDescription { get => _bikeDescription; set { _bikeDescription = value; NotifyPropertyChanged(nameof(BikeDescription)); } }

        private string _bikeImagePath;
        public string BikeImagePath { get => _bikeImagePath; set { _bikeImagePath = value; NotifyPropertyChanged(nameof(BikeImagePath)); } }

        private decimal _bikePrice;
        public decimal BikePrice { get => _bikePrice; set { _bikePrice = value; NotifyPropertyChanged(nameof(BikePrice)); } }


        public void SetRedBlockControl(Window wnd, string blockName)
        {
            if (wnd == null) return;
            Control block = wnd.FindName(blockName) as Control;
            if (block != null)
            {
                block.BorderBrush = Brushes.Red;
            }
        }

        public void SetCenterPositionAndOpen(Window window)
        {
            // Пытаемся найти главное окно, если оно есть и не является открываемым окном
            Window ownerWindow = Application.Current.MainWindow;
            if (ownerWindow != null && ownerWindow == window)
            {
                ownerWindow = null;
            }

            window.Owner = ownerWindow; 
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            // if(PropertyChanged != null) // Проверка не обязательна с C# 6.0 'PropertyChanged?.Invoke(...)'
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}