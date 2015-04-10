using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace Billionaires.Helpers
{
    public class SelectionChangedBehaviour
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof (ICommand),
            typeof (SelectionChangedBehaviour),
            new PropertyMetadata(PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var selector = (LongListSelector)depObj;
            if (selector != null)
            {
                selector.SelectionChanged += SelectionChanged;
            }
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand) element.GetValue(CommandProperty);
        }

        public static void SetCommand(UIElement element, ICommand command)
        {
            element.SetValue(CommandProperty, command);
        }

        private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (LongListSelector)sender;
            if (selector != null)
            {
                if (selector.SelectedItem == null)
                    return;

                var command = selector.GetValue(CommandProperty) as ICommand;
                if (command != null)
                {
                    var selected = selector.SelectedItem;
                    selector.SelectedItem = null;
                    command.Execute(selected);
                }
            }
        }
    }
}