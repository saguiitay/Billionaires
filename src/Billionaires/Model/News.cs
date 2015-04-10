using System.Linq;
using System.Windows.Input;

namespace Billionaires.Model
{
    public class News : BaseModel
    {
        private string _date;
        private string _link;
        private string _title;
        private string[] _thumb;

        public News()
        {
            Navigate = new NavigateToNewsCommand();
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged(); }
        }

        public string Link
        {
            get { return _link; }
            set { _link = value; NotifyPropertyChanged(); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }

        public string[] Thumb
        {
            get { return _thumb; }
            set { _thumb = value; NotifyPropertyChanged(); }
        }

        public string ThumbUrl
        {
            get
            {
                if (_thumb == null || _thumb.Length == 0)
                    return string.Empty;
                return _thumb[0];
            }
        }

        public ICommand Navigate { get; set; }
    }
}