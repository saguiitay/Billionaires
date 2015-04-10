namespace Billionaires.Model
{
    public class Name : BaseModel
    {
        private string _sort;
        private string _last;
        private string _full;

        public string Full
        {
            get { return _full; }
            set { _full = value; NotifyPropertyChanged(); }
        }

        public string Last
        {
            get { return _last; }
            set { _last = value; NotifyPropertyChanged(); }
        }

        public string Sort
        {
            get { return _sort; }
            set { _sort = value; NotifyPropertyChanged(); }
        }
    }
}