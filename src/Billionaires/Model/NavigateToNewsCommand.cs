using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;

namespace Billionaires.Model
{
    public class NavigateToNewsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is News;
        }

        public void Execute(object parameter)
        {
            var news = parameter as News;
            if (news == null)
                return;

            var task = new WebBrowserTask
                {
                    Uri = new Uri(news.Link)
                };
            task.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}