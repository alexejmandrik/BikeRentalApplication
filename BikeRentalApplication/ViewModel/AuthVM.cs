using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions; 
using System.Windows;
using System.Windows.Input;
using BikeRentalApplication.Model;
using BikeRentalApplication.Helpers;
using BikeRentalApplication.View;
using BikeRentalApplication.View;
using System.Diagnostics.Metrics;

namespace BikeRentalApplication.ViewModel
{
    public class AuthVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string titleText = "Авторизация";
        public string TitleText
        {
            get => titleText;
            set { titleText = value; OnPropertyChanged(); }
        }

        private bool isLoginVisible = true;
        public bool IsLoginVisible
        {
            get => isLoginVisible;
            set { isLoginVisible = value; OnPropertyChanged(); }
        }

        private bool isRegisterVisible = false;
        public bool IsRegisterVisible
        {
            get => isRegisterVisible;
            set { isRegisterVisible = value; OnPropertyChanged(); }
        }

        private string switchLinkText = "Нет аккаунта? Зарегистрируйтесь";
        public string SwitchLinkText
        {
            get => switchLinkText;
            set { switchLinkText = value; OnPropertyChanged(); }
        }

        private string statusMessage = "";
        public string StatusMessage
        {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }

        private string _loginUsername = "";
        public string LoginUsername
        {
            get => _loginUsername;
            set { _loginUsername = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _registerUsername = "";
        public string RegisterUsername
        {
            get => _registerUsername;
            set { _registerUsername = value; OnPropertyChanged(); }
        }

        private string _registerSurname = "";
        public string RegisterSurname
        {
            get => _registerSurname;
            set { _registerSurname = value; OnPropertyChanged(); }
        }

        private string _registerName = "";
        public string RegisterName
        {
            get => _registerName;
            set { _registerName = value; OnPropertyChanged(); }
        }

        private string _registerPatronymic = "";
        public string RegisterPatronymic
        {
            get => _registerPatronymic;
            set { _registerPatronymic = value; OnPropertyChanged(); }
        }

        private string _registerPhone = "";
        public string RegisterPhone
        {
            get => _registerPhone;
            set { _registerPhone = value; OnPropertyChanged(); }
        }

        private string _registerPassword = "";
        public string RegisterPassword
        {
            get => _registerPassword;
            set { _registerPassword = value; OnPropertyChanged(); }
        }

        private string _registerConfirmPassword = "";
        public string RegisterConfirmPassword
        {
            get => _registerConfirmPassword;
            set { _registerConfirmPassword = value; OnPropertyChanged(); }
        }

        public ICommand SwitchCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private RelayCommand openAdminBikeWindow;

        public AuthVM()
        {
            SwitchCommand = new RelayCommand(SwitchMode);
            LoginCommand = new RelayCommand(Login);       
            RegisterCommand = new RelayCommand(Register); 
        }

        private void OpenAdminBikeWindowMethod()
        {
            AdminBikeWindow adminBikeWindow = new AdminBikeWindow();
            SetCenterPositionAndOpen(adminBikeWindow);
        }
        private void OpenMainWindowMethod()
        {
            MainWindow mainWindow = new MainWindow();
            SetCenterPositionAndOpen(mainWindow);
        }

        private void SwitchMode(object? obj)
        {
            IsLoginVisible = !IsLoginVisible;
            IsRegisterVisible = !IsRegisterVisible;
            TitleText = IsLoginVisible ? "Авторизация" : "Регистрация";
            SwitchLinkText = IsLoginVisible ? "Нет аккаунта? Зарегистрируйтесь" : "Уже есть аккаунт? Войдите";
            StatusMessage = "";

            LoginUsername = "";
            Password = "";
            RegisterUsername = "";
            RegisterName = "";
            RegisterSurname = "";
            RegisterPatronymic = "";
            RegisterPhone = "";
            RegisterPassword = "";
            RegisterConfirmPassword = "";
        }


        private void Login(object? obj)
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(LoginUsername) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Введите логин и пароль.";
                return;
            }

            bool isAuthenticated = DataWorker.AuthenticateUser(LoginUsername, Password);
            if (DataWorker.CheckBlocking(LoginUsername))
            {
                LoginUsername = "";
                Password = "";
                MessageBox.Show("Ваш аккаунт заблокирован!");
                return;
            }
            if (isAuthenticated)
            {
                StatusMessage = "Успешный вход.";

                string result = DataWorker.GetUserRole(LoginUsername);
                if(result == "admin")
                {
                    OpenAdminBikeWindowMethod();
                }
                else
                {
                    OpenMainWindowMethod();
                }
            }
            else
            {
                Password = "";
                StatusMessage = "Неверный логин или пароль.";
            }
        }


        private void Register(object? obj)
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(RegisterUsername) ||
                string.IsNullOrWhiteSpace(RegisterName) ||
                string.IsNullOrWhiteSpace(RegisterSurname) ||
                string.IsNullOrWhiteSpace(RegisterPhone) ||
                string.IsNullOrWhiteSpace(RegisterPassword) ||
                string.IsNullOrWhiteSpace(RegisterConfirmPassword))
            {
                StatusMessage = "Заполните все обязательные поля.";
                return;
            }

            if (RegisterName.Any(char.IsDigit) ||
               RegisterSurname.Any(char.IsDigit) ||
               (!string.IsNullOrWhiteSpace(RegisterPatronymic) && RegisterPatronymic.Any(char.IsDigit)))
            {
                StatusMessage = "Имя, фамилия и отчество не должны содержать цифры.";
                return;
            }

            if (!Regex.IsMatch(RegisterPhone, @"^\+375\d{9}$"))
            {
                StatusMessage = "Неверный формат номера. Используйте формат: +375XXXXXXXXX";
                return;
            }
          
            if (RegisterPhone.Any(char.IsLetter))
            {
                StatusMessage = "Номер телефона не должен содержать буквы.";
                return;
            }

            if (RegisterPassword.Length < 5)
            {
                StatusMessage = "Пароль слишком короткий.";
                return;
            }
                   
            if (RegisterPassword != RegisterConfirmPassword)
            {
                StatusMessage = "Пароли не совпадают.";
                return;
            }

            bool success = DataWorker.CreateUser(
                userName: RegisterUsername,
                name: RegisterName,
                surname: RegisterSurname,
                patronymic: RegisterPatronymic,
                phoneNumber: RegisterPhone,
                password: RegisterPassword,
                userStatus: "user"
            );

            if (success)
            {
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.");
                OpenMainWindowMethod();

                LoginUsername = "";
                Password = "";
                RegisterUsername = "";
                RegisterName = "";
                RegisterSurname = "";
                RegisterPatronymic = "";
                RegisterPhone = "";
                RegisterPassword = "";
                RegisterConfirmPassword = "";

                IsLoginVisible = !IsLoginVisible;
                IsRegisterVisible = !IsRegisterVisible;
                TitleText = IsLoginVisible ? "Авторизация" : "Регистрация";
                SwitchLinkText = IsLoginVisible ? "Нет аккаунта? Зарегистрируйтесь" : "Уже есть аккаунт? Войдите";
                StatusMessage = "";
            }
            else
            {
                StatusMessage = "Ошибка регистрации. Возможно, пользователь с таким логином уже существует.";
            }
        }


        public void SetCenterPositionAndOpen(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}