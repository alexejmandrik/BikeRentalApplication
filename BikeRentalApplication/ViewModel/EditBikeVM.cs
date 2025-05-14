using BikeRentalApplication.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public class EditBikeVM : INotifyPropertyChanged
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
        private Bike _originalBike;

        public EditBikeVM(Bike selectedBike)
        {
            _originalBike = selectedBike;

            BikeName = selectedBike.Name;
            BikeDescription = selectedBike.Description;
            BikeFullDescription = selectedBike.FullDescription;
            BikeImagePath = selectedBike.ImagePath.Replace("/Resources/", "");
            BikePrice = selectedBike.Price;

            SaveCommand = new RelayCommand(SaveChanges);
        }

        #region Свойства
        private string bikeName;
        public string BikeName
        {
            get => bikeName;
            set { bikeName = value; OnPropertyChanged(); }
        }

        private string bikeDescription;
        public string BikeDescription
        {
            get => bikeDescription;
            set { bikeDescription = value; OnPropertyChanged(); }
        }

        private string bikeFullDescription;
        public string BikeFullDescription
        {
            get => bikeFullDescription;
            set { bikeFullDescription = value; OnPropertyChanged(); }
        }

        private string bikeImagePath;
        public string BikeImagePath
        {
            get => bikeImagePath;
            set { bikeImagePath = value; OnPropertyChanged(); }
        }

        private decimal bikePrice;
        public decimal BikePrice
        {
            get => bikePrice;
            set { bikePrice = value; OnPropertyChanged(); }
        }
        #endregion

        #region Команда
        public ICommand SaveCommand { get; }

        private void SaveChanges(object obj)
        {
            Window wnd = obj as Window;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\")); 
            string fullImagePath = projectDir + "\\Resources\\" + BikeImagePath;

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
            if (string.IsNullOrWhiteSpace(BikeImagePath) || !File.Exists(fullImagePath))
            {
                SetRedBlockControl(wnd, "PathBlock");
                valid = false;
            }
            if (BikePrice <= 0)
            {
                SetRedBlockControl(wnd, "PriceBlock");
                valid = false;
            }

            if (!valid) return;

            bool result = DataWorker.EditBike(
                _originalBike,
                BikeName,
                BikeDescription,
                BikeFullDescription,
                "/Resources/" + BikeImagePath,
                BikePrice
            );

            if (result)
            {
                MessageBox.Show("Изменения успешно сохранены!");
                UpdateAdminBikeView();
                wnd.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении!");
            }
        }

        #endregion

        private void UpdateAdminBikeView()
        {
            AllBikes = DataWorker.GetAllBikes();
            AllUsers = DataWorker.GetAllUsers();
            AdminBikeWindow.AllBikesView.ItemsSource = null;
            AdminBikeWindow.AllBikesView.Items.Clear();
            AdminBikeWindow.AllBikesView.ItemsSource = AllBikes;
            AdminBikeWindow.AllBikesView.Items.Refresh();
        }

        public void SetRedBlockControl(Window wnd, string blockName)
        {
            Control block = wnd.FindName(blockName) as Control;
            block.BorderBrush = Brushes.Red;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
