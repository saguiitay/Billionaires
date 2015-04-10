using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Billionaires.Cache;
using Billionaires.Helpers;
using Billionaires.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Billionaires.ViewModels
{
    public class PeopleViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Person> _people;
        private readonly List<WriteableBitmap> _imagesTiles;
        private ICommand _personSelected;
        private bool _loading;
        private bool _isDataLoaded;
        private readonly HttpClient _httpClient;

        public PeopleViewModel()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            _httpClient = new HttpClient(handler);

            People = new ObservableCollection<Person>();
            _imagesTiles = new List<WriteableBitmap>();

            PersonSelected = new NavigateToPersonCommand();
        }

        /// <summary>
        /// A collection for Person objects.
        /// </summary>
        public ObservableCollection<Person> People
        {
            get
            {
                return _people;
            }
            private set
            {
                _people = value;
                NotifyPropertyChanged();                
            }
        }

        public IEnumerable<Person> RankedPeople
        {
            get
            {
                return _people.OrderBy(s =>
                    {
                        if (s.Stats != null)
                            return s.Stats.Rank.ToString("0000");
                        return 1000.ToString("0000");
                    }).ToList();
            }
        }

        /// <summary>
        /// A collection for Person objects grouped by their first character.
        /// </summary>
        public IEnumerable<AlphaKeyGroup<Person>> GroupedPeople
        {
            get
            {
                return AlphaKeyGroup<Person>.CreateGroups(
                    People,
                    s => s.Name.Sort,
                    true);
            }
        }

        /// <summary>
        /// A collection for Person objects grouped by their first character.
        /// </summary>
        public IEnumerable<AlphaKeyGroup<Person>> GroupedByIndustryPeople
        {
            get
            {
                return AlphaKeyGroup<Person>.CreateGroupsByKey(
                    People,
                    s =>
                        {
                            if (string.IsNullOrEmpty(s.Industry))
                                return "Unknown";
                            return s.Industry;
                        },
                    s =>
                        {
                            if (s.Stats != null)
                                return s.Stats.Rank.ToString("0000");
                            return 1000.ToString("0000");
                        });
            }
        }

        public ICommand PersonSelected
        {
            get { return _personSelected; }
            private set { _personSelected = value; NotifyPropertyChanged();}
        }

        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            private set { _isDataLoaded = value; NotifyPropertyChanged();}
        }

        public bool Loading
        {
            get { return _loading; }
            private set { _loading = value; NotifyPropertyChanged(); }
        }

        private async Task<string> DownloadString(Uri url, bool forceRefresh = false)
        {
            return await JsonCache.GetAsync(url.ToCacheKey(),
                                            () => _httpClient.GetStringAsync(url), 
                                            DateTime.UtcNow.AddHours(6),
                                            forceRefresh);
        }

        /// <summary>
        /// Creates and adds a few Person objects into the Items collection.
        /// </summary>
        public async Task LoadData(bool forceRefresh = false)
        {
            if (Loading)
                return;

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Loading = true;
            try
            {
                const string metadataUrl = "http://www.bloomberg.com/billionaires/db/metadata/js";

                var result = await DownloadString(new Uri(metadataUrl), forceRefresh);
                if (result == null)
                    return;

                result = result.Substring(7);

                var x2 = JsonConvert.DeserializeObject<JObject>(result);

                var d = x2["people"];
                foreach (Person person in ConsecutivePairs(d))
                {
                    People.Add(person);
                }

                NotifyPropertyChanged("People");
                NotifyPropertyChanged("GroupedPeople");
                NotifyPropertyChanged("GroupedByIndustryPeople");

                await Task.WhenAll(
                    LoadRanking(forceRefresh),
                    LoadImages(forceRefresh),
                    LoadDetails(forceRefresh))
                    .ContinueWith(_ =>
                        {
                            IsDataLoaded = true;
                            Loading = false;
                        }, uiScheduler);
            }
            catch (Exception)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MessageBox.Show(
                        "Network is not available. Local data will be used if available, but information might be missing or incorrect.",
                        "No network", MessageBoxButton.OK);
                }

                Loading = false;
                IsDataLoaded = false;
            }
        }

        private async Task LoadDetails(bool forceRefresh = false)
        {
            var tasks = People.Select(person => LoadDetails(person, forceRefresh));
            await Task.WhenAll(tasks);
        }

        public async Task LoadDetails(Person person, bool forceRefresh = false)
        {
            const string detailsUrl = "http://www.bloomberg.com/billionaires/db/people/";

            var data = await DownloadString(new Uri(detailsUrl + person.Id), forceRefresh);
            var details = JsonConvert.DeserializeObject<PersonDetails>(data);

            person.Details = details;
        }

        private async Task LoadRanking(bool forceRefresh = false)
        {
            const string statsUrl = "http://www.bloomberg.com/billionaires/db/stats/{0}";

            for (int daysBack = 0; daysBack < 14; daysBack++)
            {
                var lastDay = DateTime.Today.AddDays(-daysBack).ToString("yyyy/MM/dd");
                var result2 = await DownloadString(new Uri(string.Format(statsUrl, lastDay)), forceRefresh);
                if (result2 == "{}")
                    continue;

                var dict = JsonConvert.DeserializeObject<IDictionary<string, Stats>>(result2);
                if (dict.Count == 0)
                    continue;

                foreach (var person in People)
                {
                    Stats stats;
                    if (dict.TryGetValue(person.Id, out stats))
                    {
                        person.Stats = stats;
                    }
                }

                var peopleToRemove = People
                    .Where(p => p.Stats == null)
                    .ToList();
                foreach (var personToRemove in peopleToRemove)
                    People.Remove(personToRemove);

                NotifyPropertyChanged("People");
                NotifyPropertyChanged("RankedPeople");
                NotifyPropertyChanged("GroupedPeople");
                NotifyPropertyChanged("GroupedByIndustryPeople");

                return;
            }
        }

        private async Task LoadImages(bool forceRefresh = false)
        {
            const string imagesUrl = "http://www.bloomberg.com/billionaires/3/face-index-f5359e1f411a441d587937f5a8f1e62c.json";
            const string imagesFacesUrlRoot = "http://www.bloomberg.com/billionaires/db/portraits/chunk/3/";


            Assembly curAssembly = Assembly.GetExecutingAssembly();
            Stream resStream = curAssembly.GetManifestResourceStream("Billionaires.Assets.Placeholder.png");
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(resStream);

            foreach (var person in People)
            {
                person.Image = new WriteableBitmap(bitmapImage);
                person.HasImage = false;
            }

            var imagesResult = await DownloadString(new Uri(imagesUrl), forceRefresh);
            var facePrintsIndex = imagesResult.IndexOf("FACE_PRINTS=");
            var facesIndex = imagesResult.IndexOf("FACES=");
            var facePrints = imagesResult.Substring(facePrintsIndex + 12, facesIndex - 13);
            var faces = imagesResult.Substring(facesIndex + 6);

            var facePrintsData = JsonConvert.DeserializeObject<List<string>>(facePrints);

            var loadImageTasks = new List<Task<PortraitsData>>();
            for (int index = 0; index < facePrintsData.Count; index++)
            {
                var printsData = facePrintsData[index];
                int index1 = index;
                var task = DownloadString(new Uri(imagesFacesUrlRoot + printsData), forceRefresh)
                    .ContinueWith(t =>
                        {
                            var imageDataResult = t.Result;
                            var imageData = JsonConvert.DeserializeObject<PortraitsData>(imageDataResult);
                            imageData.index = index1;
                            return imageData;
                        });

                loadImageTasks.Add(task);
            }

            await Task.WhenAll(loadImageTasks);

            var faceLoadingTasks = new List<Task>();
            
            foreach (var task in loadImageTasks)
            {
                var imageData = task.Result;
                var image = imageData.GetImage();
                _imagesTiles.Add(imageData.GetTileImage(image));

                faceLoadingTasks.Add(LoadFaces(faces, imageData.index));
            }

            await Task.WhenAll(faceLoadingTasks);
        }

        private async Task LoadFaces(string faces, int index)
        {
            StorageFolder shared = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shared", CreationCollisionOption.OpenIfExists);
            StorageFolder shellContent = await shared.CreateFolderAsync("ShellContent", CreationCollisionOption.OpenIfExists);


            var facesData = JsonConvert.DeserializeObject<IDictionary<string, List<int>>>(faces);
            foreach (var person in People)
            {
                List<int> imageInfo;
                if (!facesData.TryGetValue(person.Id, out imageInfo))
                    continue;
                
                var imageIndex = imageInfo[0];
                if (imageIndex != index)
                    continue;
                    
                var imageLeft = imageInfo[1]*95;
                //var image = _images[imageIndex];
                var imageTile = _imagesTiles[imageIndex];

                //person.ImageInfo = new ImageInfo { Image = imageIndex, Left = imageLeft };
                var writeableBitmap = imageTile.Crop(imageLeft, 0, 95, 100);

                person.Image = writeableBitmap; //image.CropWriteableBitmap(imageLeft);
                person.HasImage = true;

                try
                {
                    var file = await shellContent.CreateFileAsync(person.Name.Full + ".jpeg", CreationCollisionOption.FailIfExists).AsTask();
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        var personTileImage = imageTile.Crop(imageLeft, 0, 95, 100);
                        personTileImage.SaveJpeg(stream, 173, 173, 0, 100);
                    }
                }
                catch (Exception)
                {
                }
            }
            
        }

        private static IEnumerable<Person> ConsecutivePairs(IEnumerable<JToken> sequence)
        {
            var result = new List<Person>();
            foreach (var d2 in sequence)
            {
                var id = d2[0];
                var person = JsonConvert.DeserializeObject<Person>(d2[1].ToString());
                person.Id = id.ToString();

                result.Add(person);
            }
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}