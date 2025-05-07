using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BikeRentalApplication.ViewModel;

namespace BikeRentalApplication.View
{
    /// <summary>
    /// Логика взаимодействия для BikeBukingWindow.xaml
    /// </summary>
    using BikeRentalApplication.Model;

    public partial class BikeBookingWindow : Window
    {
        public BikeBookingWindow(Bike bike, int userId)
        {
            InitializeComponent();
            var viewModel = new BikeBookingVM(bike, userId);
            viewModel.RequestClose += (s, result) =>
            {
                DialogResult = result;
                Close();
            };
            DataContext = viewModel;
        }
        public BikeBookingWindow()
        {
            InitializeComponent();
        }
    }

}
