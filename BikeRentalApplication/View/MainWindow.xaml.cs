using BikeRentalApplication.View;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BikeRentalApplication.Model;

namespace BikeRentalApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Bike> Bikes { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            List<Bike> bikesFromDb = DataWorker.GetAllBikes();
            Bikes = new ObservableCollection<Bike>(bikesFromDb);
            this.DataContext = this;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Auth authWindow = new Auth();
            authWindow.ShowDialog();
        }
    }

    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return TextDecorations.Underline;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}