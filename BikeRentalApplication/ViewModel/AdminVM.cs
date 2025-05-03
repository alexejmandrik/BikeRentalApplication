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

namespace BikeRentalApplication.ViewModel
{
    public class AdminVM : INotifyPropertyChanged
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



        #region METHODS TO OPEN WINDOW
        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            SetCenterPositionAndOpen(authWindow);
        }
        private void OpenAdminBikeWindowMethod()
        {
            AdminBikeWindow adminBikeWindow = new AdminBikeWindow();
            SetCenterPositionAndOpen(adminBikeWindow);
        }
        private void OpenAdminUserWindowMethod()
        {
            AdminUserWindow adminUserWindow = new AdminUserWindow();
            SetCenterPositionAndOpen(adminUserWindow);
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
        private RelayCommand openAdminBikeWindow;
        private RelayCommand openAdminUserWindow;
        private RelayCommand openAddBikeWindow;
        private RelayCommand openEditBikeWindow;
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
                    OpenEditBikeWindowMethod();
                });
            }
        }
        public RelayCommand OpenAdminBikeWindow
        {
            get
            {
                return openAdminBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenAdminBikeWindowMethod();
                });
            }
        }
        public RelayCommand OpenAdminUserWindow
        {
            get
            {
                return openAdminUserWindow ?? new RelayCommand(obj =>
                {
                    OpenAdminUserWindowMethod();
                });
            }
        }
        #endregion

        public static Bike SelectedBike { get; set; }
        public static User SelectedUser { get; set; }

        public string BikeName { get; set; }
        public string BikeDescription { get; set; }
        public string BikeImagePath { get; set; }
        public decimal BikePrice { get; set; }


        #region CRUD BIKE METHODS
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
                            resultStr = DataWorker.CreateBike(BikeName, BikeDescription, BikeImagePath, BikePrice);
                            UpdateAdminBikeView();
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
                            UpdateAdminBikeView();
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
                            UpdateAdminBikeView();
                            SetNullValuesBikeView();
                            MessageBox.Show("Успешно изменено!");
                            wnd.Close();
                        }
                        else
                            return;
                    });

                }
            }
        #endregion

        private RelayCommand setIsBlocked;
        public RelayCommand SetIsBlocked
        {
            get
            {
                return setIsBlocked ?? new RelayCommand(obj =>
                {
                    bool resultStr = false;
                    if (SelectedUser != null)
                    {
                        resultStr = DataWorker.ChangeIsBlockedUser(SelectedUser);
                        UpdateAdminUserView();
                    }
                }
               );
            }
        }


        #region UPDATE VIEWS
        private void UpdateAdminBikeView()
        {
            AllBikes = DataWorker.GetAllBikes();
            AdminBikeWindow.AllBikesView.ItemsSource = null;
            AdminBikeWindow.AllBikesView.Items.Clear();
            AdminBikeWindow.AllBikesView.ItemsSource = AllBikes;
            AdminBikeWindow.AllBikesView.Items.Refresh();
        }
        private void UpdateAdminUserView()
        {
            AllUsers= DataWorker.GetAllUsers();
            AdminUserWindow.AllUsersView.ItemsSource = null;
            AdminUserWindow.AllUsersView.Items.Clear();
            AdminUserWindow.AllUsersView.ItemsSource = AllUsers;
            AdminUserWindow.AllUsersView.Items.Refresh();
        }
        #endregion

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

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
