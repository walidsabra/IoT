using Amqp;
using Amqp.Framing;
using GrovePi;
using GrovePi.I2CDevices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace C2D
{
    

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string HOST = "SabraIoT.azure-devices.net";
        private const int PORT = 5671;
        private const string DEVICE_ID = "SabraIoT01";
        private const string DEVICE_KEY = "n7SvFHqFr/vuZ2U24c9DEnJDfhYNEMzjjAOI4kSbV+c=";
        private const string Sas = "+WmMyP/ObvgYYwsItcAeftz7Y6U5raXue01/Qg8B4D4=";



        public MainPage()
        {
            this.InitializeComponent();
            ReceiveFromPartition();
        }



        static async void ReceiveFromPartition()
        {

             var address = new Address(HOST, PORT, "walid", Sas);
     //       Amqp.Address address = new Amqp.Address(
     //string.Format("softcut.servicebus.windows.net"),
     //5671, "Listen", Sas);

            string entity = Fx.Format("/devices/{0}/messages/deviceBound", DEVICE_ID);
            //string audience = Fx.Format("{0}/devices/{1}", HOST, DEVICE_ID);
            //string resourceUri = Fx.Format("{0}/devices/{1}", HOST, DEVICE_ID);

            Connection connection = await Connection.Factory.CreateAsync(address);
            //bool cbs = PutCbsToken(connection,HOST, Sas, audience);

            Session session = new Session(connection);


            //string sasToken = GetSharedAccessSignature(null, DEVICE_KEY, resourceUri, new TimeSpan(1, 0, 0));
            //bool cbs = PutCbsToken(connection, HOST, sasToken, audience);

            ReceiverLink receiver = new ReceiverLink(session, "receiver-link", entity);
           Message message = await receiver.ReceiveAsync();
            receiver.Accept(message);

            //Connect the RGB display to one of the I2C ports.
            //IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

            //display.SetText("Sending Data:\n" + message.Body.ToString()).SetBacklightRgb(50, 50, 255);
            await receiver.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }


        private static bool PutCbsToken(Connection connection, string host, string shareAccessSignature, string audience)
        {
            bool result = true;
            Session session = new Session(connection);

            string cbsReplyToAddress = "cbs-reply-to";
            var cbsSender = new SenderLink(session, "cbs-sender", "$cbs");
            var cbsReceiver = new ReceiverLink(session, cbsReplyToAddress, "$cbs");

            // construct the put-token message
            var request = new Message(shareAccessSignature);
            request.Properties = new Properties();
            request.Properties.MessageId = Guid.NewGuid().ToString();
            request.Properties.ReplyTo = cbsReplyToAddress;
            request.ApplicationProperties = new ApplicationProperties();
            request.ApplicationProperties["operation"] = "put-token";
            request.ApplicationProperties["type"] = "azure-devices.net:sastoken";
            request.ApplicationProperties["name"] = audience;
            cbsSender.Send(request);

            // receive the response
            var response = cbsReceiver.Receive();
            if (response == null || response.Properties == null || response.ApplicationProperties == null)
            {
                result = false;
            }
            else
            {
                int statusCode = (int)response.ApplicationProperties["status-code"];
                string statusCodeDescription = (string)response.ApplicationProperties["status-description"];
                if (statusCode != (int)202 && statusCode != (int)200) // !Accepted && !OK
                {
                    result = false;
                }
            }

            // the sender/receiver may be kept open for refreshing tokens
            cbsSender.Close();
            cbsReceiver.Close();
            session.Close();

            return result;
        }

    }
}
