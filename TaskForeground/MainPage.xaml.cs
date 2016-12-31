using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using IoTTasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaskForeground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly string WebServerRegistrationName = "WebServer";

        // This will hold the trigger reference that will allow us to run the task on demand
        private ApplicationTrigger backgroundTrigger;

        public MainPage()
        {
            this.InitializeComponent();
            this.RegisterbgTask();
        }

        private async void RegisterbgTask()
        {
            try
            {
                if (!BackgroundTaskRegistration.AllTasks.Any(reg => reg.Value.Name == WebServerRegistrationName))
                {
                    // Configure task parameters
                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                    builder.TaskEntryPoint = typeof(WebServer).FullName;
                    builder.Name = WebServerRegistrationName;

                    // Remember to set an ApplicationTrigger so we can run it on demand later
                    this.backgroundTrigger = new ApplicationTrigger();
                    builder.SetTrigger(backgroundTrigger);
                    builder.Register();

                    MessageDialog infoDialog = new MessageDialog("Background task successfully registered.", "Info");
                    await infoDialog.ShowAsync();
                }
                else
                {
                    // Fetch registration details and trigger if already existing
                    var registration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(reg => reg.Value.Name == WebServerRegistrationName).Value as BackgroundTaskRegistration;
                    this.backgroundTrigger = registration.Trigger as ApplicationTrigger;

                    MessageDialog infoDialog = new MessageDialog("Background task registration data successfully retrieved.", "Info");
                    await infoDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog errorDialog = new MessageDialog("There was an error while trying to register the background task.", "Error");
                errorDialog.ShowAsync();
            }
        }
    }
}
