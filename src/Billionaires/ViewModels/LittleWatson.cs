using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Billionaires.ViewModels
{
    ///<summary>
    /// LittleWatsonManager class.
    /// </summary>
    /// <remarks>
    /// Text to user: Send application error reports automatically and anonymously to southernsun to help us improve the application.
    /// </remarks>
    public class LittleWatson
    {
        #region Fields

        // ReSharper disable InconsistentNaming
        private static readonly LittleWatson _instance = new LittleWatson();
        // ReSharper restore InconsistentNaming
        private const string Filename = "LittleWatson.txt";
        private const string SettingsFilename = "LittleWatsonSettings.txt";
        private bool _allowAnonymousHttpReporting;

        #endregion

        #region Constructor

        ///<summary>
        /// Initializes static members of the LittleWatsonManager class.
        /// </summary>
        static LittleWatson()
        {
        }

        ///<summary>
        /// Prevents a default instance of the LittleWatsonManager class from being created.
        /// </summary>
        private LittleWatson()
        {
            _allowAnonymousHttpReporting = GetSetting();
        }

        #endregion

        #region Properties

        ///<summary>
        /// Gets DataManager instance.
        /// </summary>
        public static LittleWatson Instance
        {
            get { return _instance; }
        }

        ///<summary>
        /// Gets or sets a value indicating whether error reports are allowed to send anonymously to a http endpoint.
        /// </summary>
        public bool AllowAnonymousHttpReporting
        {
            get { return _allowAnonymousHttpReporting; }

            set
            {
                _allowAnonymousHttpReporting = value;
                SetSetting(_allowAnonymousHttpReporting);
            }
        }

        #endregion

        #region Public Methods

        ///<summary>
        /// Report exception.
        /// </summary>
        ///<param name="ex">The exception to report.</param>
        public static void SaveExceptionForReporting(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (TextWriter output = new StreamWriter(store.OpenFile(Filename, FileMode.OpenOrCreate)))
                    {
                        output.WriteLine(JsonConvert.SerializeObject(ex));
                    }
                }
            }
            catch
            {
            }
        }

        ///<summary>
        /// Check for previous logged exception.
        /// </summary>
        /// <returns>Return the exception if found.</returns>
        public static string GetPreviousException()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(Filename))
                    {
                        try
                        {
                            var isolatedStorageFileStream = store.OpenFile(Filename, FileMode.Open, FileAccess.Read, FileShare.None);
                            using (var reader = new StreamReader(isolatedStorageFileStream))
                            {
                                string data = reader.ReadToEnd();
                                return data;
                                //try
                                //{
                                //    var exception = JsonConvert.DeserializeObject<Exception>(data);
                                //    return exception;
                                //}
                                //catch
                                //{
                                //}
                            }
                        }
                        finally
                        {
                            store.DeleteFile(Filename);
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        ///<summary>
        /// Send error report (exception) to HTTP endpoint.
        /// </summary>
        /// <param name="exception">Exception to send.</param>
        public void SendExceptionToHttpEndpoint(Exception exception)
        {
            if (!AllowAnonymousHttpReporting)
            {
                return;
            }

            try
            {
                const string uri = "http://www.yourwebsite.com/data/post.php";

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                webRequest.BeginGetRequestStream(
                    r =>
                        {
                            try
                            {
                                var request1 = (HttpWebRequest)r.AsyncState;
                                Stream postStream = request1.EndGetRequestStream(r);

                                string info = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine,
                                                            exception.StackTrace);

                                string postData = "&amp;exception=" + HttpUtility.UrlEncode(info);
                                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                                postStream.Write(byteArray, 0, byteArray.Length);
                                postStream.Close();

                                request1.BeginGetResponse(
                                    s =>
                                        {
                                            try
                                            {
                                                var request2 = (HttpWebRequest)s.AsyncState;
                                                using (var response = (HttpWebResponse)request2.EndGetResponse(s))
                                                using (var streamResponse = response.GetResponseStream())
                                                using (var streamReader = new StreamReader(streamResponse))
                                                {
                                                    string response2 = streamReader.ReadToEnd();

                                                    streamReader.Close();
                                                    streamResponse.Close();
                                                    response.Close();
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }, request1);
                            }
                            catch
                            {
                            }
                        }, webRequest);
            }
            catch
            {
            }
        }

        #endregion

        #region Private Methods

        private bool GetSetting()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var isolatedStorageFileStream = store.OpenFile(SettingsFilename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                    using (TextReader reader = new StreamReader(isolatedStorageFileStream))
                    {
                        string content = reader.ReadToEnd();

                        if (!string.IsNullOrEmpty(content))
                        {
                            try
                            {
                                return JsonConvert.DeserializeObject<bool>(content);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private void SetSetting(bool value)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var isolatedStorageFileStream = store.OpenFile(SettingsFilename, FileMode.OpenOrCreate & FileMode.Truncate, FileAccess.Write, FileShare.None);
                    using (TextWriter output = new StreamWriter(isolatedStorageFileStream))
                    {
                        try
                        {
                            output.WriteLine(JsonConvert.SerializeObject(value));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}