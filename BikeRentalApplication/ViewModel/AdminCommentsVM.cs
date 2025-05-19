using BikeRentalApplication.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using BikeRentalApplication.Model.Data;
using BikeRentalApplication.View;

namespace BikeRentalApplication.ViewModel
{
    public class AdminCommentsVM : INotifyPropertyChanged
    {
        public class DisplayableComment : INotifyPropertyChanged
        {
            public Comments Comment { get; }

            public DisplayableComment(Comments comment)
            {
                Comment = comment;
            }

            public string UserName => Comment.User?.UserName ?? "Неизвестный пользователь";
            public string BikeName => Comment.Bike?.Name ?? "Неизвестный велосипед";
            public string BikeImagePath => Comment.Bike?.ImagePath ?? "/Resources/default_bike.png";
            public string VisibilityStatus => Comment.Visibility ? "Виден" : "Скрыт";

            public string CommentText
            {
                get => Comment.Comment;
                set
                {
                    if (Comment.Comment != value)
                    {
                        Comment.Comment = value;
                        OnPropertyChanged();
                        // Consider if saving changes here is needed or handled elsewhere
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string propName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }

            public void RefreshVisibilityStatus()
            {
                OnPropertyChanged(nameof(VisibilityStatus));
            }
        }

        private ObservableCollection<DisplayableComment> _userComments;
        public ObservableCollection<DisplayableComment> UserComments
        {
            get => _userComments;
            set
            {
                _userComments = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsContentVisible));
                    OnPropertyChanged(nameof(IsHistoryEmpty));
                    (DeleteSelectedCommentCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (ToggleSelectedCommentVisibilityCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private DisplayableComment _selectedComment;
        public DisplayableComment SelectedComment
        {
            get => _selectedComment;
            set
            {
                if (_selectedComment != value)
                {
                    _selectedComment = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsCommentSelected));
                    (DeleteSelectedCommentCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (ToggleSelectedCommentVisibilityCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsCommentSelected => SelectedComment != null;
        public bool IsContentVisible => !IsLoading && UserComments != null && UserComments.Count > 0;
        public bool IsHistoryEmpty => !IsLoading && (UserComments == null || UserComments.Count == 0);

        private string _loadingMessage = "Загрузка комментариев...";
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OpenAuthWindowMethod()
        {
            AuthWindow authWindow = new AuthWindow();
            Application.Current.MainWindow = authWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AdminCommentsWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }
        private void OpenAdminBikeWindowMethod()
        {
            AdminBikeWindow adminBikeWindow = new AdminBikeWindow();
            Application.Current.MainWindow = adminBikeWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AdminCommentsWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private void OpenAllBookingsMethod()
        {
            AllBookingsWindow allBookingsWindow = new AllBookingsWindow();
            Application.Current.MainWindow = allBookingsWindow;

            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is AdminCommentsWindow)?
            .Close();

            Application.Current.MainWindow.Show();
        }

        private RelayCommand openAuthWindow;
        private RelayCommand openAdminBikeWindow;
        private RelayCommand openAllBookingsWindow;
        public RelayCommand OpenAuthWindow
        {
            get
            {
                return openAuthWindow ?? new RelayCommand(obj =>
                {
                    OpenAuthWindowMethod();
                });
            }
        }
        public RelayCommand OpenAdminBikeWindow
        {
            get
            {
                return openAdminBikeWindow ?? new RelayCommand(obj =>
                {
                    OpenAdminBikeWindowMethod();
                });
            }
        }
        public RelayCommand OpenAllBookingsWindow
        {
            get
            {
                return openAllBookingsWindow ?? new RelayCommand(obj =>
                {
                    OpenAllBookingsMethod();
                });
            }
        }

        public ICommand DeleteSelectedCommentCommand { get; }
        public ICommand ToggleSelectedCommentVisibilityCommand { get; }

        public AdminCommentsVM()
        {
            UserComments = new ObservableCollection<DisplayableComment>();

            DeleteSelectedCommentCommand = new RelayCommand(
                async _ => await DeleteSelectedCommentAsync(),
                _ => IsCommentSelected && !IsLoading 
            );

            ToggleSelectedCommentVisibilityCommand = new RelayCommand(
                async _ => await ToggleSelectedCommentVisibilityAsync(),
                _ => IsCommentSelected && !IsLoading 
            );


            _ = LoadCommentsAsync();
        }


        private async Task LoadCommentsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка комментариев...";

            try
            {
                var commentsFromDb = await Task.Run(() =>
                {
                    using var db = new ApplicationContext();
                    return db.Comments
                             .Include(c => c.User)
                             .Include(c => c.Bike)
                             .OrderByDescending(c => c.Id) 
                             .ToList();
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserComments.Clear();
                    foreach (var comment in commentsFromDb)
                        UserComments.Add(new DisplayableComment(comment));
                    SelectedComment = null;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки комментариев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingMessage = "Ошибка при загрузке комментариев.";
                OnPropertyChanged(nameof(UserComments));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteSelectedCommentAsync()
        {
            if (SelectedComment == null) return;

            var result = MessageBox.Show("Удалить выбранный комментарий?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var commentEntityToDelete = SelectedComment.Comment;

                IsLoading = true;
                LoadingMessage = "Удаление комментария...";

                try
                {
                    bool deleted = await Task.Run(() => DataWorker.DeleteComment(commentEntityToDelete));
                    if (deleted)
                    {
                        await LoadCommentsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении комментария.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        IsLoading = false; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении комментария: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsLoading = false; 
                }
            }
        }

        private async Task ToggleSelectedCommentVisibilityAsync()
        {
            if (SelectedComment == null) return;

            var commentEntityToToggle = SelectedComment.Comment;

            IsLoading = true;
            LoadingMessage = "Изменение видимости комментария...";

            try
            {
                bool changed = await Task.Run(() => DataWorker.ChangeIsVisibilityComment(commentEntityToToggle));

                if (changed)
                {
                    await LoadCommentsAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось изменить статус видимости.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении видимости: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Func<object, Task> executeAsync, Predicate<object> canExecute = null)
        {
            if (executeAsync == null) throw new ArgumentNullException(nameof(executeAsync));
            _execute = async (param) => await executeAsync(param);
            _canExecute = canExecute;
        }


        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}