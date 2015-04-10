using System.Collections.ObjectModel;
using System.Linq;

namespace Billionaires.Model
{
    public class Bio : BaseModel
    {
        private ObservableCollection<string> _body;
        private ObservableCollection<Milestone> _milestones;
        private BioStats _stats;

        public ObservableCollection<string> Body
        {
            get { return _body; }
            set { _body = value; NotifyPropertyChanged(); NotifyPropertyChanged("BodyText"); }
        }

        public string BodyText
        {
            get { return string.Join("\r\n\r\n", _body); }
        }

        public ObservableCollection<Milestone> Milestones
        {
            get { return _milestones; }
            set { _milestones = value; NotifyPropertyChanged(); NotifyPropertyChanged("MilestonesText"); }
        }

        public string MilestonesText
        {
            get
            {
                if (_milestones.Count == 0)
                    return "";

                return string.Join("\r\n", _milestones.Select(m => string.Format("{0} \x25B8 {1}", m.Year, m.Event)));
            }
        }

        public BioStats Stats
        {
            get { return _stats; }
            set { _stats = value; NotifyPropertyChanged(); }
        }
    }
}