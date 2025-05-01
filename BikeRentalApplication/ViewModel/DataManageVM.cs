using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BikeRentalApp.View;
using BikeRentalApplication.Model;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public class DataManageVM : INotifyPropertyChanged
    {
        // Все велосипеды
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

        // Открытия окон
        #region METHODS TO OPEN WINDOW

        // Авторизация/регистрация
        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            SetCenterPositionAndOpen(authWindow);
        }
        // MAIN админ
        private void OpenAdminWindowMethod()
        {
            AdminWindow adminWindow = new AdminWindow();
            SetCenterPositionAndOpen(adminWindow);
        }
        // Добавление велосипеда
        private void OpenAddBikeWindowMethod()
        {
            AddBikeWindow addBikeWindow = new AddBikeWindow();
            SetCenterPositionAndOpen(addBikeWindow);
        }
        // Удаление велосипеда
        private void OpenDeleteBikeWindowMethod()
        {
            DeleteBikeWindow deleteBikeWindow = new DeleteBikeWindow();
            SetCenterPositionAndOpen(deleteBikeWindow);
        }
        // Изменение велосипеда
        private void OpenEditBikeWindowMethod()
        {
            EditBikeWindow editBikeWindow = new EditBikeWindow();
            SetCenterPositionAndOpen(editBikeWindow);
        }
        #endregion

        // Команды октрытия 
        #region COMMANDS TO OPEN WINDOWS
        private RelayCommand openAuthWindow;
        private RelayCommand openAddBikeWindow;
        private RelayCommand openDeleteBikeWindow;
        private RelayCommand openEditBikeWindow;
        public RelayCommand OpenAuthWindow
        {
            get {
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
       
        public RelayCommand OpenDeleteBikeWindow
        {
            get
            {
                return openDeleteBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenDeleteBikeWindowMethod();
                });
            }
        }
       
        public RelayCommand OpenEditBikeWindow
        {
            get
            {
                return openAddBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenEditBikeWindowMethod();
                });
            }
        }
        #endregion

        public  string BikeName { get; set; }
        public  string BikeDescription { get; set; }
        public  string BikeImagePath { get; set; }
        public decimal BikePrice{ get; set; }

        private RelayCommand addNewBike;
        public RelayCommand AddNewBike
        {
            get
            {
                return addNewBike ?? new RelayCommand(obj =>
                {
                    Window wnd = obj as Window;
                    bool resultStr = false;
                    if (BikeName == null || BikeName.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "NameBlock");
                    }
                    if(BikeDescription == null || BikeDescription.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "DescriptionBlock");
                    }
                    if (BikeImagePath == null || BikeImagePath.Replace(" ", "").Length == 0)
                    {


                        // Проверка, подгрузилась ли картинка и если нет, то вставить дефолтную картинку типо НЕТ ФОТО


                        SetRedBlockControl(wnd, "PathBlock");
                    }
                    if( BikePrice == 0)
                    {
                        SetRedBlockControl(wnd, "PriceBlock");
                    }
                    else
                    {
                        resultStr = DataWorker.CreateBike(BikeName, BikeDescription, BikeImagePath, BikePrice);
                        //UpdateAllDataViews();
                        SetNullValuesAddBikeView();
                        MessageBox.Show("Успешно добавлено!");                        
                        wnd.Close();
                    }
                }
                );
            }
        }

        private void SetNullValuesAddBikeView()
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
   

        /*#region UPDATE VIEWS
        private void UpdateAllDataViews()
        {
            UpdateAllBikeView();
        }

        private void UpdateAllBikeView()
        {
            AllBikes = DataWorker.GetAllBikes();
            MainWindow.AllBikesView.ItemsSource = null;
            MainWindow.AllBikesView.Items.Clear();
            MainWindow.AllBikesView.ItemsSource = AllBikes;
            MainWindow.AllBikesView.Items.Refresh();
        }
        #endregion*/

        public void SetCenterPositionAndOpen(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
