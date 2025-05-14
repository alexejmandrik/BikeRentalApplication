using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BikeRentalApplication.Model;
using BikeRentalApplication.Helpers;
using BikeRentalApplication.View;
using System; 
using System.Linq; 

namespace BikeRentalApplication.ViewModel
{
    public class AuthVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _titleText = "";
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; OnPropertyChanged(); }
        }

        private bool isLoginVisible = true;
        public bool IsLoginVisible
        {
            get => isLoginVisible;
            set { isLoginVisible = value; OnPropertyChanged(); UpdateLocalizedTexts(); }
        }

        private bool isRegisterVisible = false;
        public bool IsRegisterVisible
        {
            get => isRegisterVisible;
            set { isRegisterVisible = value; OnPropertyChanged(); }
        }

        private string _switchLinkText = ""; 
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
        public ICommand ChangeLanguageCommand { get; }


        public AuthVM()
        {
            SwitchCommand = new RelayCommand(SwitchMode);
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage); 

            LocalizationManager.Instance.LanguageChanged += OnLanguageChanged;
            UpdateLocalizedTexts();
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateLocalizedTexts();
        }

        private void UpdateLocalizedTexts()
        {
            TitleText = IsLoginVisible ? LocalizationManager.Instance.GetString("AuthVM.Title.Login")
                                       : LocalizationManager.Instance.GetString("AuthVM.Title.Register");
            SwitchLinkText = IsLoginVisible ? LocalizationManager.Instance.GetString("AuthVM.SwitchLink.ToRegister")
                                            : LocalizationManager.Instance.GetString("AuthVM.SwitchLink.ToLogin");
            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(SwitchLinkText));
        }

        private void ChangeLanguage(object? parameter)
        {
            if (parameter is string cultureName)
            {
                LocalizationManager.Instance.SwitchLanguage(cultureName);
            }
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
                StatusMessage = LocalizationManager.Instance.GetString("Status.EnterLoginPassword");
                return;
            }

            bool isAuthenticated = DataWorker.AuthenticateUser(LoginUsername, Password);
            if (isAuthenticated)
            {
                if (DataWorker.CheckBlocking(LoginUsername))
                {
                    LoginUsername = "";
                    Password = "";
                    MessageBox.Show(LocalizationManager.Instance.GetString("Status.AccountBlocked"));
                    return;
                }
                SessionManager.CurrentUser = DataWorker.GetUserByUsername(LoginUsername);
                StatusMessage = LocalizationManager.Instance.GetString("Status.LoginSuccessful");

                string result = DataWorker.GetUserRole(LoginUsername);
                if (result == "admin")
                {
                    OpenAdminBikeWindowMethod();
                    LoginUsername = "";
                    Password = "";
                }
                else
                {
                    OpenMainWindowMethod();
                    LoginUsername = "";
                    Password = "";
                }
            }
            else
            {
                Password = ""; 
                StatusMessage = LocalizationManager.Instance.GetString("Status.InvalidLoginOrPassword");
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
                StatusMessage = LocalizationManager.Instance.GetString("Status.FillAllRequiredFields");
                return;
            }

            if (RegisterName.Any(char.IsDigit) ||
                RegisterSurname.Any(char.IsDigit) ||
                (!string.IsNullOrWhiteSpace(RegisterPatronymic) && RegisterPatronymic.Any(char.IsDigit)))
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.NameNoDigits");
                return;
            }

            if (!Regex.IsMatch(RegisterPhone, @"^\+375\d{9}$"))
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.InvalidPhoneFormat");
                return;
            }

            if (RegisterPhone.Any(char.IsLetter))
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.PhoneNoLetters");
                return;
            }

            if (RegisterPassword.Length < 5)
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.PasswordTooShort");
                return;
            }

            if (RegisterPassword != RegisterConfirmPassword)
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.PasswordsDoNotMatch");
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
                    RegisterUsername = "";
                    RegisterName = "";
                    RegisterSurname = "";
                    RegisterPatronymic = "";
                    RegisterPhone = "";
                    RegisterPassword = "";
                    RegisterConfirmPassword = "";
                    MessageBox.Show(LocalizationManager.Instance.GetString("Status.RegistrationSuccessful"));
                    SessionManager.CurrentUser = DataWorker.GetUserByUsername(RegisterUsername);
                    OpenMainWindowMethod();
                }
                else
                {
                    StatusMessage = LocalizationManager.Instance.GetString("Status.RegistrationFailedUserExists");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = LocalizationManager.Instance.GetString("Status.UnexpectedError");
            }
        }


        public void SetCenterPositionAndOpen(Window window)
        {
            Window? ownerWindow = Application.Current.Windows.OfType<AuthWindow>().FirstOrDefault();
            if (ownerWindow != null)
            {
                window.Owner = ownerWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else 
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            window.Show();
        }
    }
}