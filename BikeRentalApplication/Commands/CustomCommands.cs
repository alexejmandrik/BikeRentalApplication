// Commands/CustomCommands.cs
using System.Windows.Input;

namespace BikeRentalApplication.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ShowAlert = new RoutedUICommand(
            "Show Alert", "ShowAlert", typeof(CustomCommands));
    }
}
