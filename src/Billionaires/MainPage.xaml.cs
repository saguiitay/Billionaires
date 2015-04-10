using System;
using System.Windows;
using Billionaires.ViewModels;
using Microsoft.Phone.Tasks;

namespace Billionaires
{
    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        private void AboutClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void RefreshOnClick(object sender, EventArgs e)
        {
            App.ViewModel.LoadData(true);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var exceptionData = LittleWatson.GetPreviousException();
            if (exceptionData != null)
            {
                var result = MessageBox.Show(
                    "Would you like to email the details of this error to the Developer?\n\n" +
                    "(No personal information, other than your email address, will be sent to the Developer)",
                    "An error has occurred!",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    Action actionTask =
                        () =>
                        {
                            var task = new EmailComposeTask
                            {
                                To = "saguiitay@hotmail.com",
                                Subject = "Error Report: Billionaires",
                                Body = string.Format(
                                    "An unhandled exception occurred in app:\n" +
                                    "Data: {0}",
                                    exceptionData)
                            };
                            task.Show();
                        };

                    if (Dispatcher.CheckAccess())
                        actionTask.Invoke();
                    else
                        Dispatcher.BeginInvoke(actionTask);
                }
            }
        }
    }
}