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


using System.Threading.Tasks;
using System.Text;

using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace C2D
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "SabraIoT.azure-devices.net";
        static string deviceKey = "n7SvFHqFr/vuZ2U24c9DEnJDfhYNEMzjjAOI4kSbV+c=";

        static string connectionString = "HostName=SabraIoT.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=HFW2M579RLpCa8th32k8xqEfx5/kBekyZyyAquX3IuA=";
        static string iotHubD2cEndpoint = "messages/events";
        

        public MainPage()
        {
            this.InitializeComponent();


            ReceiveC2dAsync();
        }

        private static async void ReceiveC2dAsync()
        {
            // Connect the RGB display to one of the I2C ports.
            IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

            display.SetText("Receiving Data:\n").SetBacklightRgb(50, 10, 30);
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("SabraIoT01", deviceKey), TransportType.Http1);

            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

               await deviceClient.CompleteAsync(receivedMessage);
                display.SetText(receivedMessage.GetBytes().ToString()).SetBacklightRgb(50, 100, 30);
            }
        }


    }
}   
