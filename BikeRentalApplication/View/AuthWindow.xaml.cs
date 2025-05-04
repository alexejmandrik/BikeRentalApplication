using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using BikeRentalApplication.Model;
using BikeRentalApplication.View;
using BikeRentalApplication.ViewModel;
using System.Windows.Input;

namespace BikeRentalApplication.View
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthVM();
        }
    }
}