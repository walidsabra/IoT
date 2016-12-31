using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using System;


namespace C2D___PiLight
{
    public class IoTHubCommunicator
    {
        public event EventHandler<string> MessageReceivedEvent;
        private string _iotHubConnectionString =
            "HostName=SabraIoT.azure-devices.net;SharedAccessKeyName=device;SharedAccessKey=2CsMoU01XOYEE1EpKz3k13w0I4ebOrrcRhILu+x5AjI=";

        public async Task SendDataToAzure(string message)
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(_iotHubConnectionString, TransportType.Http1);
            var msg = new Message(Encoding.UTF8.GetBytes(message));
            await deviceClient.SendEventAsync(msg);
        }


        public async Task ReceiveDataFromAzure()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(_iotHubConnectionString, TransportType.Http1);
            Message receivedMessage;
            string messageData;
            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    this.OnMessageReceivedEvent(messageData);
                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }

        protected virtual void OnMessageReceivedEvent(string s)
        {
            EventHandler<string> handler = MessageReceivedEvent;
            if (handler != null)
            {
                handler(this, s);
            }
        }
    }
}