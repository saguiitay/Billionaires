using System.Windows;
using System.Windows.Input;

namespace Billionaires.Helpers
{
    public class TapBehaviour
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(TapBehaviour),
            new PropertyMetadata(PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var element = (FrameworkElement)depObj;
            if (element != null)
            {
                element.Tap += Tapped;
            }
        }

        private static void Tapped(object sender, GestureEventArgs e)
        {
            var selector = (FrameworkElement )sender;
            if (selector != null)
            {
                var command = selector.GetValue(CommandProperty) as ICommand;
                if (command != null)
                {
                    command.Execute(selector.DataContext);
                }
            }
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommand(UIElement element, ICommand command)
        {
            element.SetValue(CommandProperty, command);
        }
    }
}