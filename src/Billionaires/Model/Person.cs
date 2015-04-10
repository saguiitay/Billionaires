// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Input;
using System.Windows.Media.Imaging;
using Billionaires.ViewModels;

namespace Billionaires.Model
{
    public class Person : BaseModel
    {
        private Name _name;
        private int? _age;
        private Stats _stats;
        //private ImageInfo _imageInfo;
        private bool _hasImage;
        private WriteableBitmap _image;
        private PersonDetails _details;
        private string _finalDay;
        private string _source;
        private string _industry;
        private string _gender;
        private string _place;
        //private string _status;
        //private int _children;
        private ICommand _navigate;
        public string Id { get; set; }

        public Person()
        {
            Navigate = new NavigateToPersonCommand();
        }

        public ICommand Navigate
        {
            get { return _navigate; }
            set { _navigate = value; NotifyPropertyChanged(); }
        }


        public Name Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }

        //public int? Age
        //{
        //    get { return _age; }
        //    set { _age = value; NotifyPropertyChanged(); }
        //}

        //public int Children
        //{
        //    get { return _children; }
        //    set { _children = value; NotifyPropertyChanged(); }
        //}

        //public string Status
        //{
        //    get { return _status; }
        //    set { _status = value; NotifyPropertyChanged(); }
        //}

        public string Place
        {
            get { return _place; }
            set { _place = value; NotifyPropertyChanged(); }
        }

        public string Gender
        {
            get { return _gender; }
            set { _gender = value; NotifyPropertyChanged(); }
        }

        public string Industry
        {
            get { return _industry; }
            set { _industry = value; NotifyPropertyChanged(); }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; NotifyPropertyChanged(); }
        }

        public string Final_day
        {
            get { return _finalDay; }
            set { _finalDay = value; NotifyPropertyChanged(); }
        }

        public Stats Stats
        {
            get { return _stats; }
            set { _stats = value; NotifyPropertyChanged(); }
        }

        //public ImageInfo ImageInfo
        //{
        //    get { return _imageInfo; }
        //    set { _imageInfo = value; NotifyPropertyChanged(); }
        //}

        public WriteableBitmap Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged(); }
        }

        public bool HasImage
        {
            get { return _hasImage; }
            set { _hasImage = value; NotifyPropertyChanged(); }
        }

        public PersonDetails Details
        {
            get { return _details; }
            set { _details = value; NotifyPropertyChanged(); }
        }
    }
}
