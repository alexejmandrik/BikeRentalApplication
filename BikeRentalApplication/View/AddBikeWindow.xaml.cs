﻿using BikeRentalApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BikeRentalApplication.View
{
    /// <summary>
    /// Логика взаимодействия для AddBikeWindow.xaml
    /// </summary>
    public partial class AddBikeWindow : Window
    {
        public AddBikeWindow()
        {
            InitializeComponent();
            DataContext = new AdminVM();
        }
        private void PrewiewTextInput( object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
