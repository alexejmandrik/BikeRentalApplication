using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using BikeRentalApp.View;
using BikeRentalApplication.Model;
using BikeRentalApplication.View;

namespace BikeRentalApplication.View
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }
        #region
        private bool isRegisterMode = false;
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";
            string username = LoginUsernameTextBox.Text;
            string password = LoginPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Логин и пароль не могут быть пустыми.";
                return;
            }

            bool isAuthenticated = DataWorker.AuthenticateUser(username, password);

            if (isAuthenticated)
            {

                StatusTextBlock.Foreground = Brushes.Green;
                StatusTextBlock.Text = "Вход выполнен успешно!";
                MessageBox.Show($"Добро пожаловать, {username}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                string role = DataWorker.GetUserRole(username);
                if (role == "user")
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.ShowDialog();
                }
                else
                {
                    AdminBikeWindow adminPage = new AdminBikeWindow();
                    adminPage.ShowDialog();
                }
            }
            else
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Неверный логин или пароль.";
                LoginPasswordBox.Clear();
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";
            string username = RegisterUsernameTextBox.Text;
            string Name = RegisterNameTextBox.Text;
            string Surname = RegisterSurnameTextBox.Text;
            string Patronymic = RegisterPatronymicTextBox.Text;
            string phoneNumber = RegisterPhoneTextBox.Text;
            string password = RegisterPasswordBox.Password;
            string confirmPassword = RegisterConfirmPasswordBox.Password;
            string userStatus = "user";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(Patronymic) ||
                string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Все поля регистрации обязательны.";
                return;
            }
            if (DataWorker.SearchUserByUserName(username))
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Пользователь с таким логином уже существует.";
                return;
            }

            if (Name.Any(char.IsDigit) || Surname.Any(char.IsDigit) || Patronymic.Any(char.IsDigit))
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "ФИО не должно содержать цифры.";
                return;
            }

            string phonePattern = @"^\+375\d{9}$";

            if (!Regex.IsMatch(phoneNumber, phonePattern))
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Номер телефона должен начинаться с +375 и содержать 9 цифр после.";
                return;
            }

            if (password != confirmPassword)
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Пароли не совпадают.";
                RegisterPasswordBox.Clear();
                RegisterConfirmPasswordBox.Clear();
                return;
            }

            if (password.Length < 6)
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Пароль должен быть не менее 6 символов.";
                return;
            }
            

            bool isRegistered = DataWorker.CreateUser(username, Name, Surname, Patronymic, phoneNumber, password, userStatus);

            if (isRegistered)
            {
                StatusTextBlock.Foreground = Brushes.Green;
                StatusTextBlock.Text = "Регистрация прошла успешно! Теперь вы можете войти.";
                ClearRegisterFields();
                SwitchToLoginMode();
            }
            else
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "Ошибка регистрации. Возможно, такой логин уже занят.";
            }
        }
        private void SwitchLink_Click(object sender, RoutedEventArgs e)
        {
            isRegisterMode = !isRegisterMode;
            StatusTextBlock.Text = "";

            if (isRegisterMode)
            {
                SwitchToRegisterMode();
            }
            else
            {
                SwitchToLoginMode();
            }
        }

        private void SwitchToRegisterMode()
        {
            isRegisterMode = true;
            TitleTextBlock.Text = "Регистрация";
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            (SwitchLink.Inlines.FirstInline as Run).Text = "Уже есть аккаунт? Войти";
            ClearLoginFields();
            RegisterUsernameTextBox.Focus();
        }

        private void SwitchToLoginMode()
        {
            isRegisterMode = false;
            TitleTextBlock.Text = "Авторизация";
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            (SwitchLink.Inlines.FirstInline as Run).Text = "Нет аккаунта? Зарегистрироваться";
            ClearRegisterFields();
            LoginUsernameTextBox.Focus();
        }

        private void ClearLoginFields()
        {
            LoginUsernameTextBox.Clear();
            LoginPasswordBox.Clear();
        }
        private void ClearRegisterFields()
        {
            RegisterUsernameTextBox.Clear();
            RegisterNameTextBox.Clear();
            RegisterSurnameTextBox.Clear();
            RegisterPatronymicTextBox.Clear();
            RegisterPhoneTextBox.Clear();
            RegisterPasswordBox.Clear();
            RegisterConfirmPasswordBox.Clear();
        }

        private static System.Collections.Generic.Dictionary<string, string> registeredUsers =
            new System.Collections.Generic.Dictionary<string, string>();
    }
#endregion
}