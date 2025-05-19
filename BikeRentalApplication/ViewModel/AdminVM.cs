using BikeRentalApplication.Model;
using BikeRentalApplication.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;

namespace BikeRentalApplication.ViewModel
{
    public class AdminVM : INotifyPropertyChanged
    {
        #region BIKE OR USER PANEL
        private bool isBikeVisible = true;
        public bool IsBikeVisible
        {
            get => isBikeVisible;
            set { isBikeVisible = value; OnPropertyChanged(); }
        }

        private bool isUserVisible = false;
        public bool IsUserVisible
        {
            get => isUserVisible;
            set { isUserVisible = value; OnPropertyChanged(); }
        }

        public ICommand SwitchCommand { get; }
        public RelayCommand AddNewBike { get; private set; }
        public RelayCommand DeleteItem { get; private set; }
        public RelayCommand SetIsBlocked { get; private set; }
        public AdminVM()
        {
            SwitchCommand = new RelayCommand(SwitchMode);
            AddNewBike = new RelayCommand(AddNewBikeExecute);
            DeleteItem = new RelayCommand(DeleteItemExecute);
            SetIsBlocked = new RelayCommand(SetIsBlockedExecute);
        }
        private void SwitchMode(object? obj)
        {
            IsBikeVisible = !IsBikeVisible;
            IsUserVisible = !IsUserVisible;
        }
        #endregion

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
        private List<User> allUsers = DataWorker.GetAllUsers();
        public List<User> AllUsers
        {
            get { return allUsers; }
            set
            {
                allUsers = value;
                NotifyPropertyChanged("AllUsers");
            }
        }


        #region METHODS TO OPEN WINDOW
        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AdminBikeWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }
        private void OpenAddBikeWindowMethod()
        {
            AddBikeWindow addBikeWindow = new AddBikeWindow();
            SetCenterPositionAndOpen(addBikeWindow);
        }
        private void OpenEditBikeWindowMethod()
        {
            if (SelectedBike != null)
            {
                var vm = new EditBikeVM(SelectedBike);
                var editBikeWindow = new EditBikeWindow
                {
                    DataContext = vm
                };
                SetCenterPositionAndOpen(editBikeWindow);
            }
        }

        private void OpenAllBookingsWindowMethod()
        {
            AllBookingsWindow allBookingsWindow = new AllBookingsWindow();
            Application.Current.MainWindow = allBookingsWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AdminBikeWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }
        #endregion

        #region COMMANDS TO OPEN WINDOWS

        private RelayCommand openAuthWindow;
        private RelayCommand openAddBikeWindow;
        private RelayCommand openEditBikeWindow;
        private RelayCommand openAllBookingsWindow;
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

        public RelayCommand OpenAddBikeWindow
        {
            get
            {
                return openAddBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenAddBikeWindowMethod();
                });
            }
        }

        public RelayCommand OpenEditBikeWindow
        {
            get
            {
                return openEditBikeWindow ?? new RelayCommand(obj =>
                {
                    if(SelectedBike != null)
                    {
                        OpenEditBikeWindowMethod();
                    }
                    else
                    {
                        MessageBox.Show("Необходимо выбрать велосипед!");
                    }
                });
            }
        }

        public RelayCommand OpenAllBookingsWindow
        {
            get
            {
                return openAllBookingsWindow ?? new RelayCommand(obj =>
                {
                    OpenAllBookingsWindowMethod();
                });
            }
        }
        #endregion

        public static Bike SelectedBike { get; set; }
        public static User SelectedUser { get; set; }

        public string BikeName { get; set; }
        public string BikeDescription { get; set; }
        public string BikeFullDescription { get; set; }
        public string BikeImagePath { get; set; }
        public decimal BikePrice { get; set; }


        private void AddNewBikeExecute(object obj)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\"));
            string fullPath = projectDir + "\\Resources\\" + BikeImagePath;
            Window wnd = obj as Window;

            bool valid = true;

            if (string.IsNullOrWhiteSpace(BikeName))
            {
                SetRedBlockControl(wnd, "NameBlock");
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(BikeDescription))
            {
                SetRedBlockControl(wnd, "DescriptionBlock");
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(BikeFullDescription))
            {
                SetRedBlockControl(wnd, "FullDescriptionBlock");
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(BikeImagePath))
            {
                SetRedBlockControl(wnd, "PathBlock");
                valid = false;
            }
            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Фото по данному пути не существует!");
                SetRedBlockControl(wnd, "PathBlock");
                valid = false;
            }
            if (BikePrice == 0)
            {
                SetRedBlockControl(wnd, "PriceBlock");
                valid = false;
            }

            if (valid)
            {
                BikeImagePath = "/Resources/" + BikeImagePath;
                bool resultStr = DataWorker.CreateBike(BikeName, BikeDescription, BikeFullDescription, BikeImagePath, BikePrice);
                UpdateAdminBikeView();
                SetNullValuesBikeView();
                MessageBox.Show("Успешно добавлено!");
                wnd?.Close();
            }
        }

        private void DeleteItemExecute(object obj)
        {
            if (SelectedBike != null)
            {
                var result = MessageBox.Show(
                    "Вы действительно хотите удалить данный велосипед?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool resultStr = DataWorker.DeleteBike(SelectedBike);
                    UpdateAdminBikeView();
                }
            }
            else
            {
                MessageBox.Show("Необходимо выбрать велосипед!");
            }

            SetNullValuesBikeView();
        }

        private void SetIsBlockedExecute(object obj)
        {
            if (SelectedUser != null)
            {
                bool resultStr = DataWorker.ChangeIsBlockedUser(SelectedUser);
                UpdateAdminBikeView();
            }
        }

        private void UpdateAdminBikeView()
        {
            AllBikes = DataWorker.GetAllBikes();
            AllUsers = DataWorker.GetAllUsers();
            AdminBikeWindow.AllBikesView.ItemsSource = null;
            AdminBikeWindow.AllBikesView.Items.Clear();
            AdminBikeWindow.AllBikesView.ItemsSource = AllBikes;
            AdminBikeWindow.AllBikesView.Items.Refresh();
        }

        private void SetNullValuesBikeView()
        {
            BikeName = null;
            BikeDescription = null;
            BikeImagePath = null;
            BikePrice = 0;
        }

        public void SetRedBlockControl(Window wnd, string blockName)
        {
            Control block = wnd.FindName(blockName) as Control;
            block.BorderBrush = Brushes.Red;
        }

        public void SetCenterPositionAndOpen(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
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
