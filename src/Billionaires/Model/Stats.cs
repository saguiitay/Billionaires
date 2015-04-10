using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Billionaires.Model
{
    public class Stats : BaseModel
    {
        private int _ytdRel;
        private int _ytd;
        private int _lastRel;
        private int _last;
        private List<int> _hold;
        private int _net;
        private int _rank;
        private readonly NumberFormatInfo _myNfi;

        public Stats()
        {
            _myNfi = new NumberFormatInfo
                {
                    CurrencyNegativePattern = 1,
                    CurrencySymbol = "$"
                };
        }

        public int Rank
        {
            get { return _rank; }
            set { _rank = value; NotifyPropertyChanged(); }
        }

        public int Net
        {
            get { return _net; }
            set { _net = value; NotifyPropertyChanged(); NotifyPropertyChanged("NetValue"); }
        }

        public string NetValue
        {
            get { return string.Format("${0:0.0#}B", _net / 100f); }
        }

        public List<int> Hold
        {
            get { return _hold; }
            set { _hold = value; NotifyPropertyChanged(); }
        }

        public int Last
        {
            get { return _last; }
            set { _last = value; NotifyPropertyChanged(); NotifyPropertyChanged("LastText"); }
        }

        public string LastText
        {
            get
            {
                var unit = "M";
                var val = _last/100f;
                if (Math.Abs(val) > 1000)
                {
                    val = val/1000f;
                    unit = "B";
                }
                
                return val.ToString("C", _myNfi) + unit;
            }
        }

        public int Last_rel
        {
            get { return _lastRel; }
            set { _lastRel = value; NotifyPropertyChanged(); NotifyPropertyChanged("LastRelText"); }
        }

        public string LastRelText
        {
            get { return string.Format("{0:0.0#}", _lastRel / 100f); }
        }

        public Brush LastColor
        {
            get { return new SolidColorBrush(_last < 0 ? Colors.Red : Colors.Green); }
        }



        public int Ytd
        {
            get { return _ytd; }
            set { _ytd = value; NotifyPropertyChanged(); NotifyPropertyChanged("YtdText"); }
        }

        public string YtdText
        {
            get
            {
                var unit = "M";
                var val = _ytd / 100f;
                if (Math.Abs(val) > 1000)
                {
                    val = val / 1000f;
                    unit = "B";
                }
                return val.ToString("C", _myNfi) + unit;
            }
        }

        public int Ytd_rel
        {
            get { return _ytdRel; }
            set { _ytdRel = value; NotifyPropertyChanged(); NotifyPropertyChanged("YtdRelText"); }
        }

        public string YtdRelText
        {
            get { return string.Format("{0:0.0#}", _ytdRel / 100f); }
        }

        public Brush YtdColor
        {
            get { return new SolidColorBrush(_ytd < 0 ? Colors.Red : Colors.Green); }
        }

    }
}