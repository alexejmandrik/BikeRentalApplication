using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BikeRentalApplication.Controls
{
    public class ValidatedTextBox : TextBox
    {
        public static readonly DependencyProperty DigitsOnlyTextProperty =
            DependencyProperty.Register(
                nameof(DigitsOnlyText),
                typeof(string),
                typeof(ValidatedTextBox),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null,
                    CoerceDigitsOnly),
                ValidateDigitsOnly);

        public string DigitsOnlyText
        {
            get => (string)GetValue(DigitsOnlyTextProperty);
            set => SetValue(DigitsOnlyTextProperty, value);
        }

        private static bool ValidateDigitsOnly(object value) =>
            value is string str && str.All(char.IsDigit);

        private static object CoerceDigitsOnly(DependencyObject d, object baseValue)
        {
            if (baseValue is string str)
                return new string(str.Where(char.IsDigit).ToArray());
            return string.Empty;
        }
    }
}
