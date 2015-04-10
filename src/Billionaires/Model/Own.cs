namespace Billionaires.Model
{
    public class Own : BaseModel
    {
        private string _title;
        private string _credit;
        private string _image;
        private int _width;
        private int _height;

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }

        public string Credit
        {
            get { return _credit; }
            set { _credit = value; NotifyPropertyChanged(); }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged(); }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; NotifyPropertyChanged(); }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; NotifyPropertyChanged(); }
        }
    }
}