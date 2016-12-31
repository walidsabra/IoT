using GrovePi;
using GrovePi.I2CDevices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace C2D___PiLight
{

    public sealed partial class MainPage : Page
    {
        private IoTHubCommunicator _communicator;

        public MainPage()
        {
            this.InitializeComponent();
            _communicator = new IoTHubCommunicator();
            _communicator.MessageReceivedEvent += _communicator_MessageReceivedEvent;
            _communicator.ReceiveDataFromAzure(); //start listening

        }

        private void _communicator_MessageReceivedEvent(object sender, string e)
        {
            //update UI
            txtReceived.Text = e;
            //start listening again
            _communicator.ReceiveDataFromAzure();

            //Connect the RGB display to one of the I2C ports.
            IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

            display.SetText("R Data:\n" + e).SetBacklightRgb(50, 50, 255);

        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            _communicator.SendDataToAzure(txtSend.Text);
        }
    }
}