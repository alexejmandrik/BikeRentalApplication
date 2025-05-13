using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace BikeRentalApplication.Helpers // Или ваше предпочтительное пространство имен
{
    public class LocalizationManager : INotifyPropertyChanged
    {
        public static LocalizationManager Instance { get; } = new LocalizationManager();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? LanguageChanged; // Событие для уведомления ViewModel

        private CultureInfo _currentCulture;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    Thread.CurrentThread.CurrentCulture = _currentCulture;
                    Thread.CurrentThread.CurrentUICulture = _currentCulture;
                    LoadLanguageResources(_currentCulture.Name);
                    OnPropertyChanged();
                    LanguageChanged?.Invoke(this, EventArgs.Empty); // Уведомляем подписчиков
                }
            }
        }

        public List<CultureInfo> SupportedLanguages { get; } = new List<CultureInfo>
        {
            new CultureInfo("ru-RU"),
            new CultureInfo("en-US")
        };

        private LocalizationManager()
        {
            // Установите культуру по умолчанию, например, русскую
            _currentCulture = new CultureInfo("ru-RU"); // Важно инициализировать
        }

        public void SwitchLanguage(string cultureName)
        {
            CurrentCulture = new CultureInfo(cultureName);
        }

        // Внутри LocalizationManager.cs, метод LoadLanguageResources

        private void LoadLanguageResources(string cultureName)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            var existingDict = dictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources."));
            if (existingDict != null)
            {
                dictionaries.Remove(existingDict);
            }

            var newDict = new ResourceDictionary
            {
               
                Source = new Uri($"pack://application:,,,/BikeRentalApplication;component/Resources/StringResources.{cultureName}.xaml", UriKind.Absolute)
            };
            dictionaries.Add(newDict);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Вспомогательный метод для получения локализованной строки из ViewModel или Code-behind
        public string GetString(string key)
        {
            var resource = Application.Current.TryFindResource(key);
            // Возвращаем ключ в скобках, если ресурс не найден, для облегчения отладки
            return resource as string ?? $"[{key}]";
        }
    }
}