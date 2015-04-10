using System.Collections.ObjectModel;

namespace Billionaires.Model
{
    public class PersonDetails : BaseModel
    {
        private Overview _overview;
        //private Confidence _confidence;
        //private Portfolio _portfolio;
        private ObservableCollection<News> _news;
        //private ObservableCollection<Own> _owns;
        private Worth _worth;
        private Bio _bio;

        public Overview Overview
        {
            get { return _overview; }
            set { _overview = value; NotifyPropertyChanged(); }
        }

        //public Confidence Confidence
        //{
        //    get { return _confidence; }
        //    set { _confidence = value; NotifyPropertyChanged(); }
        //}

        //public Portfolio Portfolio
        //{
        //    get { return _portfolio; }
        //    set { _portfolio = value; NotifyPropertyChanged(); }
        //}

        public ObservableCollection<News> News
        {
            get { return _news; }
            set { _news = value; NotifyPropertyChanged(); }
        }

        //public ObservableCollection<Own> Owns
        //{
        //    get { return _owns; }
        //    set { _owns = value; NotifyPropertyChanged(); }
        //}

        public Worth Worth
        {
            get { return _worth; }
            set { _worth = value; NotifyPropertyChanged(); }
        }

        public Bio Bio
        {
            get { return _bio; }
            set { _bio = value; NotifyPropertyChanged(); }
        }
    }
}