using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BikeRentalApplication.Model;
using BikeRentalApplication.View;


namespace BikeRentalApplication.ViewModel
{
    public class AuthVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _titleText = "Вход";
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; OnPropertyChanged(); }
        }

        private bool isLoginVisible = true;
        public bool IsLoginVisible
        {
            get => isLoginVisible;
            set { isLoginVisible = value; OnPropertyChanged(); UpdateTexts(); }
        }

        private bool isRegisterVisible = false;
        public bool IsRegisterVisible
        {
            get => isRegisterVisible;
            set { isRegisterVisible = value; OnPropertyChanged(); }
        }

        private string _switchLinkText = "Зарегистрироваться";
        public string SwitchLinkText
        {
            get => _switchLinkText;
            set { _switchLinkText = value; OnPropertyChanged(); }
        }

        private string statusMessage = "";
        public string StatusMessage
        {
            get => statusMessage;
            set { statusMessage = value; OnPropertyChanged(); }
        }

        public string LoginUsername { get; set; } = "";
        public string Password { get; set; } = "";
        public string RegisterUsername { get; set; } = "";
        public string RegisterSurname { get; set; } = "";
        public string RegisterName { get; set; } = "";
        public string RegisterPatronymic { get; set; } = "";
        public string RegisterPhone { get; set; } = "";
        public string RegisterPassword { get; set; } = "";
        public string RegisterConfirmPassword { get; set; } = "";

        public ICommand SwitchCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public AuthVM()
        {
            SwitchCommand = new RelayCommand(SwitchMode);
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);

            UpdateTexts();
        }

        private void UpdateTexts()
        {
            TitleText = IsLoginVisible ? "Вход" : "Регистрация";
            SwitchLinkText = IsLoginVisible ? "Нет аккаунта? Зарегистрироваться" : "Уже есть аккаунт? Войти";
        }

        private void OpenAdminBikeWindowMethod()
        {
            AdminBikeWindow adminBikeWindow = new AdminBikeWindow();
            Application.Current.MainWindow = adminBikeWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AuthWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private void OpenMainWindowMethod()
        {
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AuthWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private void SwitchMode(object? obj)
        {
            IsLoginVisible = !IsLoginVisible;
            IsRegisterVisible = !IsRegisterVisible;
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

            if (!Regex.IsMatch(LoginUsername, @"^.{1,50}$"))
            {
                StatusMessage = "Логин должен содержать от 1 до 50 символов.";
                return;
            }

            bool isAuthenticated = DataWorker.AuthenticateUser(LoginUsername, Password);
            if (isAuthenticated)
            {
                if (DataWorker.CheckBlocking(LoginUsername))
                {
                    LoginUsername = "";
                    Password = "";
                    MessageBox.Show("Ваш аккаунт заблокирован.");
                    return;
                }

                SessionManager.CurrentUser = DataWorker.GetUserByUsername(LoginUsername);
                StatusMessage = "Вход выполнен успешно.";

                string result = DataWorker.GetUserRole(LoginUsername);
                if (result == "admin")
                {
                    OpenAdminBikeWindowMethod();
                }
                else
                {
                    OpenMainWindowMethod();
                }
                LoginUsername = "";
                Password = "";
            }
            else
            {
                Password = "";
                StatusMessage = "Неверный логин или пароль.";
            }
        }

        private async void Register(object? obj)
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(RegisterUsername) ||
                string.IsNullOrWhiteSpace(RegisterName) ||
                string.IsNullOrWhiteSpace(RegisterSurname) ||
                string.IsNullOrWhiteSpace(RegisterPhone) ||
                string.IsNullOrWhiteSpace(RegisterPassword) ||
                string.IsNullOrWhiteSpace(RegisterConfirmPassword))
            {
                StatusMessage = "Пожалуйста, заполните все обязательные поля.";
                return;
            }

            if (!Regex.IsMatch(RegisterUsername, @"^.{2,50}$") ||
                !Regex.IsMatch(RegisterName, @"^.{2,50}$") ||
                !Regex.IsMatch(RegisterSurname, @"^.{2,50}$") ||
                (!string.IsNullOrWhiteSpace(RegisterPatronymic) && !Regex.IsMatch(RegisterPatronymic, @"^.{2,50}$")))
            {
                StatusMessage = "Все поля должны содержать от 2 до 50 символов.";
                return;
            }

            if (Regex.IsMatch(RegisterName, @"\d") ||
                Regex.IsMatch(RegisterSurname, @"\d") ||
                (!string.IsNullOrWhiteSpace(RegisterPatronymic) && Regex.IsMatch(RegisterPatronymic, @"\d")))
            {
                StatusMessage = "Имя, фамилия и отчество не должны содержать цифры.";
                return;
            }

            if (!Regex.IsMatch(RegisterPhone, @"^\+375\d{9}$"))
            {
                StatusMessage = "Неверный формат номера телефона. Пример: +375291234567";
                return;
            }

            if (RegisterPassword.Length < 5)
            {
                StatusMessage = "Пароль должен содержать не менее 5 символов.";
                return;
            }

            if (RegisterPassword != RegisterConfirmPassword)
            {
                StatusMessage = "Пароли не совпадают.";
                return;
            }

            try
            {
                bool success = await DataWorker.CreateUserAsync(
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
                    MessageBox.Show("Регистрация прошла успешно!");
                    SessionManager.CurrentUser = DataWorker.GetUserByUsername(RegisterUsername);
                    OpenMainWindowMethod();
                }
                else
                {
                    StatusMessage = "Пользователь с таким логином уже существует.";
                }
            }
            catch
            {
                StatusMessage = "Произошла непредвиденная ошибка.";
            }
        }

    }
}
