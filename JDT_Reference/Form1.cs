using System;
using System.ComponentModel;
using System.Windows.Forms;
using Stm32ComportLib;

namespace JDT_Reference
{
    public partial class Form1 : Form
    {
        private string comPort = "COM39";
        private int baud = 115200;
        private Device device;

        public Form1()
        {
            InitializeComponent();
            OpenDevice();
        }

        private bool OpenDevice()
        {
            string port = comPort;
            device = new Device(port, baud);

            //Register for data events
            device.OnReceiving += Device_OnReceiving;
            //Register for module events
            device.OnModuleInfo += Device_OnModuleInfo;
            //Register for error events
            device.OnModuleError += Device_OnModuleError;

            device.Open();

            //GetModuleInfo is blocking, so will block for 2-3 sec and then return
            if (!device.GetModuleInfo())
            {
                device.Close();
                device = null;
                return false;
            }
            //Set measurement mode to continous
            device.SetMeasMode(Meas_Mode_Enum.Meas_Continuous);
            //Set device sample frequency
            device.SetSampleFrequency(20);
            //Enable data output
            device.SetDataMode(Data_Mode_Enum.Data_On);
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            device.Close();
            base.OnClosing(e);
        }

        private void Device_OnModuleInfo(object sender, ModuleInfoPacketEvent e)
        {
            string s = "ModuleInfo: " + e.ProductId(e.ModuleId);
            Console.WriteLine(s);
        }

        private void Device_OnModuleError(object sender, ModuleErrorPacketEvent e)
        {
            Console.WriteLine($"Error: {e.Error}");
        }

        private void Device_OnReceiving(object sender, DataPacketEvent e)
        {
            bool presence = e.Presence;
            double raw = e.Signal;
            double filtered = e.Filtered;
            double threshold = e.Threshold;
            double objectTemp = e.ObjectTemperature;
            double ambientTemp = e.AmbientTemperature;

            string s = string.Format("{0},{1},{2},{3},{4},{5}", presence, raw, filtered, threshold, objectTemp, ambientTemp);
            Console.WriteLine(s);
        }
    }
}
