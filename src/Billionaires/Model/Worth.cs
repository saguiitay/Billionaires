using System.Collections.ObjectModel;

namespace Billionaires.Model
{
    public class Worth : BaseModel
    {
        private ObservableCollection<string> _body;
        public ObservableCollection<string> Body
        {
            get { return _body; }
            set { _body = value; NotifyPropertyChanged(); NotifyPropertyChanged("BodyText"); }
        }

        public string BodyText
        {
            get
            {
                if (_body == null || _body.Count == 0)
                    return "";

                return string.Join("\r\n\r\n", _body);
            }
        }
    }
}