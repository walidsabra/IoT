using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;
using Windows.ApplicationModel.Background;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace RandomDeviceValues
{
    public sealed class StartupCustomClass : IBackgroundTask
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "SabraIoT.azure-devices.net";
        static string deviceKey = "n7SvFHqFr/vuZ2U24c9DEnJDfhYNEMzjjAOI4kSbV+c=";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Connect the RGB display to one of the I2C ports.
            IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("SabraIoT01", deviceKey), TransportType.Mqtt);

            while (true)
            {
                Task.Delay(100).Wait();


                display.SetText("Sending Data:\n" ).SetBacklightRgb(50, 50, 255);
                try
                {
                    double avgWindSpeed = 10; // m/s
                    Random rand = new Random();
                    while (true)
                    {
                        double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                        var telemetryDataPoint = new
                        {
                            deviceId = "SabraIoT01",
                            windSpeed = currentWindSpeed
                        };
                        var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                        var message = new Message(Encoding.ASCII.GetBytes(messageString));

                        await deviceClient.SendEventAsync(message);

                        
                    }
                }
                catch (Exception)
                {


                }
            }
        }
    }
}
