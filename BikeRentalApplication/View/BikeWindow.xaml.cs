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
using BikeRentalApplication.Model;

namespace BikeRentalApplication.View
{
    /// <summary>
    /// Логика взаимодействия для BikeWindow.xaml
    /// </summary>
    public partial class BikeWindow : Window
    {
        public BikeWindow(Bike selectedBike)
        {
            InitializeComponent();
            DataContext = new BikeVM(selectedBike);
        }
    }
}
