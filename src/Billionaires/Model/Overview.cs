using System.Collections.ObjectModel;
using System.Linq;

namespace Billionaires.Model
{
    public class Overview : BaseModel
    {
        private ObservableCollection<string> _body;
        private ObservableCollection<string> _intel;

        public ObservableCollection<string> Body
        {
            get { return _body; }
            set { _body = value; NotifyPropertyChanged(); NotifyPropertyChanged("BodyText"); }
        }

        public string BodyText
        {
            get { return string.Join("\r\n\r\n", _body); }
        }

        public ObservableCollection<string> Intel
        {
            get { return _intel; }
            set { _intel = value; NotifyPropertyChanged(); NotifyPropertyChanged("IntelText"); }
        }

        public string IntelText
        {
            get
            {
                if (_intel.Count == 0)
                    return "";

                return string.Join("\r\n", _intel.Select(i => "\x25B8 " + i));
            }
        }
    }
}