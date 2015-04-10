using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Billionaires.Helpers.Tiles;
using Microsoft.Phone.Shell;

namespace Billionaires.Views
{
    public partial class Person
    {
        private string _personId;

        public Person()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("id", out _personId))
            {
                if (!App.ViewModel.IsDataLoaded)
                {
                    App.ViewModel.People.CollectionChanged += (sender, args) => DisplayPerson();
                    if (!App.ViewModel.Loading)
                        await App.ViewModel.LoadData();
                }
                else
                {
                    DisplayPerson();
                }
            }
            base.OnNavigatedTo(e);
        }

        private void DisplayPerson()
        {
            if (DataContext != null)
                return;

            var person = App.ViewModel.People.FirstOrDefault(p => p.Id == _personId);
            if (person != null)
            {
                DataContext = person;

                UpdateButtons(person);
            }
        }

        private void UpdateButtons(Model.Person person)
        {
            foreach (var button in ApplicationBar.Buttons.OfType<ApplicationBarIconButton>().ToList())
            {
                if (button.Text == "unpin" || button.Text == "pin")
                {
                    ApplicationBar.Buttons.Remove(button);
                }
            }

            var existingTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(person.Id));
            if (existingTile == null)
            {

                var applicationBarIconButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Pin.png", UriKind.Relative));
                applicationBarIconButton.Text = "pin";
                applicationBarIconButton.Click += PinClick;

                ApplicationBar.Buttons.Add(applicationBarIconButton);
            }
            else
            {
                var applicationBarIconButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.UnPin.png", UriKind.Relative));
                applicationBarIconButton.Text = "unpin";
                applicationBarIconButton.Click += UnPinClick;

                ApplicationBar.Buttons.Add(applicationBarIconButton);
            }
        }

        private async void RefreshClick(object sender, EventArgs e)
        {
            var person = (Model.Person) DataContext;
            
            await App.ViewModel.LoadDetails(person, true);

        }

        private void UnPinClick(object sender, EventArgs e)
        {
            var person = (Model.Person) DataContext;
            var existingTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(person.Id));
            if (existingTile != null)
                existingTile.Delete();

            UpdateButtons(person);
        }


        private void PinClick(object sender, EventArgs e)
        {
            var person = (Model.Person)DataContext;

            var fileNameIconImage = "/Shared/ShellContent/" + person.Id + ".202x202.jpeg";

            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.GetDirectoryNames("/Shared/ShellContent").Length == 0)
                    userStore.CreateDirectory("/Shared/ShellContent");

                using (var stream = new IsolatedStorageFileStream(fileNameIconImage, FileMode.OpenOrCreate, userStore))
                {
                    var wbmp = new WriteableBitmap(173, 173);

                    wbmp.Render(new Image {Source = person.Image},
                                new CompositeTransform { TranslateX = 25, TranslateY = 21, ScaleX = 1.3, ScaleY = 1.3});
                    wbmp.Invalidate();

                    wbmp.WritePng(stream);
                }

                var oIcontile = new StandardTileData
                    {
                        Title = person.Name.Full,
                        BackgroundImage = new Uri("isostore:" + fileNameIconImage, UriKind.Absolute),
                    };

                // find the tile object for the application tile that using "Iconic" contains string in it.            
                var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(person.Id));
                if (tileToFind != null &&
                    tileToFind.NavigationUri.ToString().Contains(person.Id))
                {
                    tileToFind.Delete();
                }
                ShellTile.Create(new Uri("/Views/Person.xaml?id=" + person.Id, UriKind.Relative), oIcontile, false);
            }

            UpdateButtons(person);
        }
    }
}
