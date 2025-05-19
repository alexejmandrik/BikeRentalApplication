using BikeRentalApplication.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            BikeImagePath = selectedBike.ImagePath?.Replace("/Resources/", "").TrimStart('/', '\\') ?? string.Empty;
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
            if (wnd == null) return;

            ResetAllErrorHighlights(wnd);
            List<string> errorMessages = new List<string>();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\"));
            string relativeImagePath = BikeImagePath?.TrimStart('/', '\\') ?? string.Empty;
            string fullImagePath = Path.Combine(projectDir, "Resources", relativeImagePath);


            if (string.IsNullOrWhiteSpace(BikeName))
            {
                SetErrorHighlight(wnd, "NameBlock");
                errorMessages.Add("Название велосипеда не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(BikeDescription))
            {
                SetErrorHighlight(wnd, "DescriptionBlock");
                errorMessages.Add("Описание велосипеда не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(BikeFullDescription))
            {
                SetErrorHighlight(wnd, "FullDescriptionBlock");
                errorMessages.Add("Полное описание велосипеда не может быть пустым.");
            }
            if (string.IsNullOrWhiteSpace(BikeImagePath))
            {
                SetErrorHighlight(wnd, "PathBlock");
                errorMessages.Add("Путь к изображению не может быть пустым.");
            }
            else if (!File.Exists(fullImagePath))
            {
                SetErrorHighlight(wnd, "PathBlock");
                errorMessages.Add($"Файл изображения не найден. Проверьте путь: {fullImagePath}");
            }
            if (BikePrice <= 0)
            {
                SetErrorHighlight(wnd, "PriceBlock");
                errorMessages.Add("Цена должна быть больше нуля.");
            }

            if (errorMessages.Any())
            {
                MessageBox.Show(string.Join(Environment.NewLine, errorMessages), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string dbImagePath = "/Resources/" + relativeImagePath;

            bool result = DataWorker.EditBike(
                _originalBike,
                BikeName,
                BikeDescription,
                BikeFullDescription,
                dbImagePath,
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
                MessageBox.Show("Ошибка при сохранении изменений в базу данных!", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void UpdateAdminBikeView()
        {
            AllBikes = DataWorker.GetAllBikes();
            AllUsers = DataWorker.GetAllUsers();
            if (AdminBikeWindow.AllBikesView != null)
            {
                AdminBikeWindow.AllBikesView.ItemsSource = null;
                AdminBikeWindow.AllBikesView.ItemsSource = this.AllBikes; 
            }
        }

        private void SetControlBorder(Window wnd, string controlName, Brush borderBrush)
        {
            if (wnd.FindName(controlName) is Control control)
            {
                control.BorderBrush = borderBrush;
            }
        }

        private void SetErrorHighlight(Window wnd, string controlName)
        {
            SetControlBorder(wnd, controlName, Brushes.Red);
        }

        private void ResetAllErrorHighlights(Window wnd)
        {
            Brush defaultBrush = SystemColors.ControlDarkBrush;

            SetControlBorder(wnd, "NameBlock", defaultBrush);
            SetControlBorder(wnd, "DescriptionBlock", defaultBrush);
            SetControlBorder(wnd, "FullDescriptionBlock", defaultBrush);
            SetControlBorder(wnd, "PathBlock", defaultBrush);
            SetControlBorder(wnd, "PriceBlock", defaultBrush);
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
}