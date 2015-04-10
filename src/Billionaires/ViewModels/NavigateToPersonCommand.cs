using System;
using System.Windows;
using System.Windows.Input;
using Billionaires.Model;

namespace Billionaires.ViewModels
{
    public class NavigateToPersonCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is Person;
        }

        public void Execute(object parameter)
        {
            var person = parameter as Person;
            if (person == null)
                return;

            ((App)Application.Current).RootFrame.Navigate(new Uri("/Views/Person.xaml?id=" + person.Id, UriKind.Relative));
        }

        public event EventHandler CanExecuteChanged;
    }
}