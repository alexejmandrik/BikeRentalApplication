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
        private void OpenAdminWindowMethod()
        {
            AdminWindow adminWindow = new AdminWindow();
            SetCenterPositionAndOpen(adminWindow);
        }
        private void OpenAddBikeWindowMethod()
        {
            AddBikeWindow addBikeWindow = new AddBikeWindow();
            SetCenterPositionAndOpen(addBikeWindow);
        }
        private void OpenEditBikeWindowMethod()
        {
            EditBikeWindow editBikeWindow = new EditBikeWindow();
            SetCenterPositionAndOpen(editBikeWindow);
        }
        #endregion

        #region COMMANDS TO OPEN WINDOWS
        private RelayCommand openAuthWindow;
        private RelayCommand openAddBikeWindow;
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
       
        public RelayCommand OpenEditBikeWindow
        {
            get
            {
                return openEditBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenEditBikeWindowMethod();
                });
            }
        }
        #endregion

        public static Bike SelectedBike { get; set; }
        public static Bike SelectedUser { get; set; }

        public string BikeName { get; set; }
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
                    bool valid = true;
                    if (BikeName == null || BikeName.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "NameBlock");
                        valid = false;
                    }
                    if(BikeDescription == null || BikeDescription.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "DescriptionBlock");
                        valid = false;
                    }
                    if (BikeImagePath == null || BikeImagePath.Replace(" ", "").Length == 0)
                    {


                        // Проверка, подгрузилась ли картинка и если нет, то вставить дефолтную картинку типо НЕТ ФОТО


                        SetRedBlockControl(wnd, "PathBlock");
                        valid = false;
                    }
                    if( BikePrice == 0)
                    {
                        SetRedBlockControl(wnd, "PriceBlock");
                        valid = false;
                    }
                    if (valid)
                    {
                        resultStr = DataWorker.CreateBike(BikeName, BikeDescription, BikeImagePath, BikePrice);
                        UpdateAdminView();
                        SetNullValuesBikeView();
                        MessageBox.Show("Успешно добавлено!");
                        wnd.Close();
                    }
                    else
                        return;
                }
                );
            }
        }

        private RelayCommand deleteItem;
        public RelayCommand DeleteItem
        {
            get
            {
                return deleteItem ?? new RelayCommand(obj =>
                {
                    bool resultStr = false;
                    if (SelectedBike != null)
                    {
                        resultStr = DataWorker.DeleteBike(SelectedBike);
                        UpdateAdminView();
                    }
                    SetNullValuesBikeView();
                }
               );
            }
        }

        private RelayCommand editBike;
        public RelayCommand EditBike
        {
            get
            {
                return editBike ?? new RelayCommand(obj =>
                {
                    Window wnd = obj as Window;
                    bool resultStr = false;
                    bool valid = true;
                    if (BikeName == null || BikeName.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "NameBlock");
                        valid = false;
                    }
                    if (BikeDescription == null || BikeDescription.Replace(" ", "").Length == 0)
                    {
                        SetRedBlockControl(wnd, "DescriptionBlock");
                        valid = false;
                    }
                    if (BikeImagePath == null || BikeImagePath.Replace(" ", "").Length == 0)
                    {


                        // Проверка, подгрузилась ли картинка и если нет, то вставить дефолтную картинку типо НЕТ ФОТО


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
                        resultStr = DataWorker.EditBike(SelectedBike, BikeName, BikeDescription, BikeImagePath, BikePrice);
                        UpdateAdminView();
                        SetNullValuesBikeView();
                        MessageBox.Show("Успешно изменено!");
                        wnd.Close();
                    }
                    else
                        return;
                });

            }
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
    


        #region UPDATE VIEWS

        private void UpdateAdminView()
        {
            AllBikes = DataWorker.GetAllBikes();
            AdminWindow.AllBikesView.ItemsSource = null;
            AdminWindow.AllBikesView.Items.Clear();
            AdminWindow.AllBikesView.ItemsSource = AllBikes;
            AdminWindow.AllBikesView.Items.Refresh();
        }
        #endregion

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
