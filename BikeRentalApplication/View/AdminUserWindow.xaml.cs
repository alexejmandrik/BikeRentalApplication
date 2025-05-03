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
    /// Логика взаимодействия для AdminUserWindow.xaml
    /// </summary>
    public partial class AdminUserWindow : Window
    {
        public static ItemsControl AllUsersView;
        public AdminUserWindow()
        {
            InitializeComponent();
            DataContext = new AdminVM();
            AllUsersView = ViewAllUsers;
        }
    }
}
