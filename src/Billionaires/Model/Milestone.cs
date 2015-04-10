namespace Billionaires.Model
{
    public class Milestone : BaseModel
    {
        private int _year;
        private string _event;

        public int Year
        {
            get { return _year; }
            set { _year = value; NotifyPropertyChanged(); }
        }

        public string Event
        {
            get { return _event; }
            set { _event = value; NotifyPropertyChanged(); }
        }
    }
}