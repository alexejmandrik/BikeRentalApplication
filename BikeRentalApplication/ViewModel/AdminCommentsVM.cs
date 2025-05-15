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

            public string CommentText
            {
                get => Comment.Comment;
                set
                {
                    if (Comment.Comment != value)
                    {
                        Comment.Comment = value;
                        OnPropertyChanged();
                    }
                }
            }

            public string VisibilityStatus => Comment.Visibility ? "Виден" : "Скрыт";
            public string BikeImagePath => Comment.Bike?.ImagePath ?? "/Resources/default_bike.png";

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsContentVisible));
                OnPropertyChanged(nameof(IsHistoryEmpty));
            }
        }

        public bool IsContentVisible => !IsLoading && UserComments.Count > 0;
        public bool IsHistoryEmpty => !IsLoading && UserComments.Count == 0;

        private string _loadingMessage = "Загрузка комментариев...";
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenProfileWindowCommand { get; }

        public AdminCommentsVM()
        {
            UserComments = new ObservableCollection<DisplayableComment>();
            _ = LoadCommentsAsync();
        }

        private async Task LoadCommentsAsync()
        {
            IsLoading = true;

            try
            {
                var comments = await Task.Run(() =>
                {
                    using var db = new ApplicationContext();
                    return db.Comments
                             .Include(c => c.User)
                             .Include(c => c.Bike)
                             .Where(c => c.Visibility) // фильтрация по видимости
                             .ToList();
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserComments.Clear();
                    foreach (var comment in comments)
                    {
                        UserComments.Add(new DisplayableComment(comment));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки комментариев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingMessage = "Ошибка при загрузке комментариев.";
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsContentVisible));
                OnPropertyChanged(nameof(IsHistoryEmpty));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
