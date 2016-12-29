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

using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;
using Windows.ApplicationModel.Background;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DeviceRandomValues
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "SabraIoT.azure-devices.net";
        static string deviceKey = "n7SvFHqFr/vuZ2U24c9DEnJDfhYNEMzjjAOI4kSbV+c=";

        public MainPage()
        {

            this.InitializeComponent();
            Task.Delay(100).Wait();

            pushData();
        }

        private async void pushData()
        {
            // Connect the RGB display to one of the I2C ports.
            IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("SabraIoT01", deviceKey), TransportType.Http1);

            while (true)
            {
                Task.Delay(100).Wait();


                
                try
                {
                    double avgWindSpeed = 10; // m/s
                    Random rand = new Random();
             
                        double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                        var telemetryDataPoint = new
                        {
                            deviceId = "SabraIoT01",
                            windSpeed = currentWindSpeed,
                            app = "WM"
                        };
                        var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                        var message = new Message(Encoding.ASCII.GetBytes(messageString));

                        await deviceClient.SendEventAsync(message);

                        display.SetText("Sending Data:\n" + "WM" + " - " + currentWindSpeed).SetBacklightRgb(50, 50, 255);
                    
                }
                catch (Exception)
                {

                    display.SetText("Sending Data:\n" + "No Data").SetBacklightRgb(50, 50, 255);
                }

            }
        }
    }
}
