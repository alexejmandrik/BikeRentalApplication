using System.Windows;
using System.Windows.Controls;

namespace BikeRentalApplication.Controls
{
    public class RoutedButton : Button
    {
        public static readonly RoutedEvent DirectClickedEvent =
            EventManager.RegisterRoutedEvent("DirectClicked", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(RoutedButton));

        public static readonly RoutedEvent BubblingClickedEvent =
            EventManager.RegisterRoutedEvent("BubblingClicked", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(RoutedButton));

        public static readonly RoutedEvent TunnelingClickedEvent =
            EventManager.RegisterRoutedEvent("TunnelingClicked", RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler), typeof(RoutedButton));

        public event RoutedEventHandler DirectClicked
        {
            add => AddHandler(DirectClickedEvent, value);
            remove => RemoveHandler(DirectClickedEvent, value);
        }

        public event RoutedEventHandler BubblingClicked
        {
            add => AddHandler(BubblingClickedEvent, value);
            remove => RemoveHandler(BubblingClickedEvent, value);
        }

        public event RoutedEventHandler TunnelingClicked
        {
            add => AddHandler(TunnelingClickedEvent, value);
            remove => RemoveHandler(TunnelingClickedEvent, value);
        }

        protected override void OnClick()
        {
            RaiseEvent(new RoutedEventArgs(DirectClickedEvent));
            RaiseEvent(new RoutedEventArgs(BubblingClickedEvent));
            RaiseEvent(new RoutedEventArgs(TunnelingClickedEvent));
            base.OnClick();
        }
    }
}
