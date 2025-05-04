using BikeRentalApplication.View;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BikeRentalApplication.Model;
using BikeRentalApplication.ViewModel;
using System.Windows.Controls;

namespace BikeRentalApplication.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ItemsControl AllBikesView;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DataManageVM();
            AllBikesView = ViewAllBikes;

        }
    }
}