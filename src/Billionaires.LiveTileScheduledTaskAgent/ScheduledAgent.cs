using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Windows.Storage;
using Windows.Storage.Search;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace Billionaires.LiveTileScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override async void OnInvoke(ScheduledTask task)
        {

            ShellTile tile = ShellTile.ActiveTiles.First();

            if (tile != null)
            {
                StorageFolder shared = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shared", CreationCollisionOption.OpenIfExists);
                StorageFolder shellContent = await shared.CreateFolderAsync("ShellContent", CreationCollisionOption.OpenIfExists);

                var files = await shellContent.GetFilesAsync();
                if (files.Count == 0)
                    return;
                var random = new Random();

                var file = files[random.Next(0, files.Count)] as StorageFile;

                var tileData = new StandardTileData();

                tileData.BackTitle = file.Name.Replace(".jpeg", "");
                tileData.BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/" + file.Name, UriKind.RelativeOrAbsolute);

                tile.Update(tileData);
            }
#if DEBUG_AGENT
	        ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
	        System.Diagnostics.Debug.WriteLine("Periodic task is started again: " + task.Name);
#endif

            NotifyComplete();
        }
    }
}