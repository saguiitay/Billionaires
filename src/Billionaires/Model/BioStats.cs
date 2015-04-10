namespace Billionaires.Model
{
    public class BioStats : BaseModel
    {
        private string _family;
        private string _birth;
        private string _education;

        public string Family
        {
            get { return _family; }
            set { _family = value; NotifyPropertyChanged(); }
        }

        public string Birth
        {
            get { return _birth; }
            set { _birth = value; NotifyPropertyChanged(); }
        }

        public string Education
        {
            get { return _education; }
            set { _education = value; NotifyPropertyChanged(); }
        }
    }
}