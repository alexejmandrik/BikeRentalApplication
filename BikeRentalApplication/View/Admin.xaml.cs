using BikeRentalApplication.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace BikeRentalApplication.View
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void AddBikeView_Click(object sender, RoutedEventArgs e)
        {
            ClearAddForm();
            MessageBox.Show("Форма добавления велосипеда активна.");
        }

        private void DeleteBikeView_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функционал 'Удалить велосипед' еще не реализован.");
        }

        private void ChangeStatusView_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функционал 'Изменить статус' еще не реализован.");
        }

        private void BlockUserView_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функционал 'Заблокировать пользователя' еще не реализован.");
        }

        private void AddBikeButton_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = ImagePathTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string priceStr = PriceTextBox.Text.Trim();
            if (!decimal.TryParse(priceStr, out decimal priceDec))
                return;
               

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Пожалуйста, введите название велосипеда.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную положительную цену.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                PriceTextBox.SelectAll();
                return;
            }

            try
            {
                bool isAdd = DataWorker.CreateBike(name, description, imagePath, price);
                if(isAdd)
                {
                    MessageBox.Show("Велосипед успешно добавлен!");
                    ClearAddForm();
                }
                else
                {
                    MessageBox.Show("Ошибка добавления!");
                }
                    
            }
            catch (Exception ex)
            {
                // Обработка ошибок сохранения в БД
                MessageBox.Show($"Произошла ошибка при добавлении велосипеда: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearAddForm()
        {
            ImagePathTextBox.Clear();
            NameTextBox.Clear();
            DescriptionTextBox.Clear();
            PriceTextBox.Clear();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Auth MainWindow = new Auth();
            MainWindow.Show();
            this.Close();
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
}