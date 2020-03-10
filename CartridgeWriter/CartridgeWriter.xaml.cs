// Copyright (c) 2014, David Slayton <slaytonrnd@outlook.com>
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the <organization> nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL <DAVID SLAYTON> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using CartridgeWriterExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Management;
using System.Text.RegularExpressions;




namespace CartridgeWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Cartridge c = null;
        private SerialPort serialPort;
        byte[] flash;
        byte[] rom;
        List<string> devices = new List<string>();
        string sel_port = null;

        public MainWindow()
        {
            InitializeComponent();

            input_text.AcceptsReturn = true;
            input_text.IsReadOnly = true;

            cboPrinterType.ItemsSource = Machine.GetAllTypes();
            cboMaterialCurrent.ItemsSource = Material.GetAllNames();
            cboMaterialChangeTo.ItemsSource = Material.GetAllNames();
            cboBays.ItemsSource = Bay.GetAllNames();
            load_list();

        }

        //Read out the hexcode over the serial port
        public void cmdReadSerial_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(cboDevices.Text) || String.IsNullOrEmpty(cboBays.Text))
            {
                MessageBox.Show("A Serial Port and a Bay must be selected!");
                return;
            }

            

            Regex r = new Regex(@"COM(\d+)");
            Match m = r.Match(cboDevices.Text);

            sel_port = m.Groups[0].Value;


            if (InitComInterface(sel_port) == 5)
                return;

            serialPort.DiscardInBuffer();
            serialPort.Write(Bay.FromName(cboBays.Text).code_read);
            serialPort.Write("\r\n");
            System.Threading.Thread.Sleep(5000);
            input_text.Text = serialPort.ReadExisting();
            serialPort.Close();
            if (String.IsNullOrEmpty(input_text.Text))
                MessageBox.Show("I received nothing! Make sure the printer is connected.", "", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        //decrypt hexcode from the cartridge to get real values
        private void cmdRead_Click(object sender, RoutedEventArgs e)
        {

           string input_flash = input_text.Text;

            if (String.IsNullOrEmpty(cboPrinterType.Text))
            {
                MessageBox.Show("I need a Printer Type before I can read.");
                return;
            }

            if ( String.IsNullOrEmpty(input_text.Text))
            {
                MessageBox.Show("There is nothing to decrypt.");
                return;
            }

           // index_device = cboPrinterType.SelectedIndex;
            c = ReadCartridge(Machine.FromType(cboPrinterType.Text),input_flash);

            if (c == null)
                return;

            LoadControls();
        }

        //Create the new hexcode
        private void cmd_create_Click(object sender, RoutedEventArgs e)
        {
            if (c == null)
            {
                MessageBox.Show("I need to Read before I can Write.");
                return;
            }

            if (String.IsNullOrEmpty(cboBays.Text))
            {
                MessageBox.Show("A Bay must be selected!");
                return;
            }

            UpdateCartridge();
            LoadControls();
            string newflash = Bay.FromName(cboBays.Text).code_write + CreateOutput(c.Encrypted);


            if (String.IsNullOrEmpty(cboDevices.Text))
            {
                MessageBox.Show("A Serial Port must be selected!");
                return;
            }

            if (InitComInterface(sel_port) == 5)
                return;

            serialPort.DiscardInBuffer();
            serialPort.Write(newflash);
            serialPort.Write("\r\n");
            serialPort.Close();
            System.Threading.Thread.Sleep(2000);
            MessageBox.Show("Please check if it worked, by removing and reinserting the cartridge!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            clear_all_entries();
        }

        //Generate a random number as new serial number
        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            Random zufall = new Random();                            
            int number = zufall.Next(10000000, 99999999);
            txtSerialNumberChangeTo.Text = number.ToString() + ",0";
        }

        //Automatically change the current quantity value to the inital quantity value
        private void txtInitialQuantityChangeTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCurrentQuantityChangeTo.Text = txtInitialQuantityChangeTo.Text;
        }

        public Cartridge ReadCartridge(Machine machine, string flashstring)
        {
            Cartridge c = null;
            {
                rom = ConvertHexStringToByteArray(clear_ID(flashstring));
                flash = ConvertHexStringToByteArray(clear_code(flashstring));
            }

            if (BitConverter.IsLittleEndian)
                rom = rom.Reverse();


            if (Properties.Settings.Default.Save_to_File)
                SaveFlashToFile(flashstring);

            c = new Cartridge(flash, machine, rom);
            return c;
        }

        //Sets the text to the textboxes
        private void LoadControls()
        {
            //txtEEPROMUID.Text = clear_ID(input_flash);
            txtEEPROMUID.Text = c.EEPROMUID.Reverse().HexString();
            txtKeyFragment.Text = c.KeyFragment;
            txtSerialNumberCurrent.Text = c.SerialNumber.ToString("f1");
            txtSerialNumberChangeTo.Text = txtSerialNumberCurrent.Text;
            cboMaterialCurrent.Text = c.Material.Name;
            cboMaterialChangeTo.Text = cboMaterialCurrent.Text;
            txtManufacturingLotCurrent.Text = c.ManufacturingLot;
            txtManufacturingLotChangeTo.Text = txtManufacturingLotCurrent.Text;
            txtManufacturingDateCurrent.Text = c.ManfuacturingDate.ToString("dd'-'MM'-'yyyy - HH':'mm':'ss");
            txtManufacturingDateChangeTo.Text = txtManufacturingDateCurrent.Text;
            txtLastUseDateCurrent.Text = c.UseDate.ToString("dd'-'MM'-'yyyy - HH':'mm':'ss");
            txtLastUseDateChangeTo.Text = txtLastUseDateCurrent.Text;
            txtInitialQuantityCurrent.Text = c.InitialMaterialQuantity.ToString();
            txtInitialQuantityChangeTo.Text = txtInitialQuantityCurrent.Text; 
            txtCurrentQuantityCurrent.Text = c.CurrentMaterialQuantity.ToString();
            txtCurrentQuantityChangeTo.Text = c.InitialMaterialQuantity.ToString();
            txtVersionCurrent.Text = c.Version.ToString();
            txtVersionChangeTo.Text = txtVersionCurrent.Text;
            txtSignatureCurrent.Text = c.Signature;
            txtSignatureChangeTo.Text = txtSignatureCurrent.Text;

            //Random zufall = new Random();
            //txtSerialNumberChangeTo.Text = zufall.Next(10000000, 99999999).ToString() + ",0";

        }

        // Update any changed Cartridge properties.
        private void UpdateCartridge()
        {
            if (!txtSerialNumberCurrent.Text.Equals(txtSerialNumberChangeTo.Text))
                c.SerialNumber = double.Parse(txtSerialNumberChangeTo.Text);

            if (!cboMaterialCurrent.Text.Equals(cboMaterialChangeTo.Text))
                c.Material = Material.FromName(cboMaterialChangeTo.Text);

            if (!txtManufacturingLotCurrent.Text.Equals(txtManufacturingLotChangeTo.Text))
                c.ManufacturingLot = txtManufacturingLotChangeTo.Text;

            if (!txtManufacturingDateCurrent.Text.Equals(txtManufacturingDateChangeTo.Text))
                c.ManfuacturingDate = DateTime.ParseExact(txtManufacturingDateChangeTo.Text, "yyyy-MM-dd HH:mm:ss", null);

            if (!txtLastUseDateCurrent.Text.Equals(txtLastUseDateChangeTo.Text))
                c.UseDate = DateTime.ParseExact(txtLastUseDateChangeTo.Text, "yyyy-MM-dd HH:mm:ss", null);

            if (!txtInitialQuantityCurrent.Text.Equals(txtInitialQuantityChangeTo.Text))
                c.InitialMaterialQuantity = double.Parse(txtInitialQuantityChangeTo.Text)/16.3871;

            if (!txtCurrentQuantityCurrent.Text.Equals(txtCurrentQuantityChangeTo.Text))
                c.CurrentMaterialQuantity = double.Parse(txtCurrentQuantityChangeTo.Text)/16.3871;

            if (!txtVersionCurrent.Text.Equals(txtVersionChangeTo.Text))
                c.Version = ushort.Parse(txtVersionChangeTo.Text);

            if (!txtSignatureCurrent.Text.Equals(txtSignatureChangeTo.Text))
                c.Signature = txtSignatureChangeTo.Text;
        }

        // clears all entries
        private void clear_all_entries()
        {
            txtEEPROMUID.Text = null;
            txtKeyFragment.Text = null;
            txtSerialNumberCurrent.Text = null;
            txtSerialNumberChangeTo.Text = null;
            cboMaterialCurrent.Text = null;
            cboMaterialChangeTo.Text = null;
            txtManufacturingLotCurrent.Text = null;
            txtManufacturingLotChangeTo.Text = null;
            txtManufacturingDateCurrent.Text = null;
            txtManufacturingDateChangeTo.Text = null;
            txtLastUseDateCurrent.Text = null;
            txtLastUseDateChangeTo.Text = null;
            txtInitialQuantityCurrent.Text = null;
            txtInitialQuantityChangeTo.Text = null;
            txtCurrentQuantityCurrent.Text = null;
            txtCurrentQuantityChangeTo.Text = null;
            txtVersionCurrent.Text = null;
            txtVersionChangeTo.Text = null;
            txtSignatureCurrent.Text = null;
            txtSignatureChangeTo.Text = null;
            input_text.Text = null;
        }

        //Initialise the Serial Interface
        public int InitComInterface(string port)
        {
            try
            {

                serialPort = new SerialPort(port);
                serialPort.BaudRate = 38400;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.RequestToSend;
                serialPort.DtrEnable = true;
                serialPort.Open();
                return 0;
            }
            catch (Exception e)
            {
                MessageBox.Show("The specified COM port (" + sel_port + ") is not available. Please take a different!\n");
                MessageBox.Show(e.ToString());
                return 5;
            }
        }


        // Save a file of the DS2433 chip contents
        private void SaveFlashToFile(string input_flash)
        {
            string path = @".\EEPROMFiles";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + @"\" + clear_ID(input_flash).Replace(" ", String.Empty);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DateTime now = DateTime.Now;
            path = path + @"\" + now.ToString("dd.MM.yyyy_HH.mm.ss") + ".txt";
            File.WriteAllText(path, input_flash);

        }

        //Creating the output string in the correct form for writing it back to the chip
        public static string CreateOutput(byte[] output_flash)
        {
            string output;
            output = "\"" + BitConverter.ToString(output_flash).Replace("-", ",") + "\"";
            return output;
        }

        //convert the hexcode to a byte array, so it is useable for decryption
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", String.Empty);
            byte[] HexAsBytes = new byte[(hexString.Length) / 2];

            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;

        }

        //extract the hexcode from the rest of the code sent over the serial port
        private string clear_code(string uncut)
        {
            string[] splittedStrings = uncut.Split(new[] { "000000: " }, StringSplitOptions.None);
            string Dump = splittedStrings[2];
            string code = Dump.Substring(0, 48);
            for (int i = 1; i <= (Dump.Length - 48) / 76; i++)
            {
                code = code + Dump.Substring(76 * i, 48);
            }


            return code;
        }

        //extract the ID from the rest of the code sent over the serial port
         private string clear_ID(string uncut)
        {
            try
            {
                string[] splittedStrings = uncut.Split(new[] { "000000: " }, StringSplitOptions.None);
                string clearcode = splittedStrings[1];
                string ID = clearcode.Substring(0, 23);
                return ID;
            }
            catch (Exception e)
            {
                MessageBox.Show("Not the right Form");
                MessageBox.Show(e.ToString());
                return "";
            }
            }

        public static void Error_Output(string error)
        {
            MainWindow ca;
            ca = new MainWindow();
            MainWindow cb = new MainWindow();
            MessageBox.Show(error, "", MessageBoxButton.OK, MessageBoxImage.Error);
            ca.clear_all_entries();
            cb.clear_all_entries();
            return;
        }
        private void load_list()
        {
            SelectQuery q = new SelectQuery("Win32_PNPEntity", "Name LIKE '%(COM%)%'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(q))
            {
                using (ManagementObjectCollection moc = searcher.Get())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        devices.Add(mo["Name"].ToString());
                    }
                }
            }
            cboDevices.ItemsSource = devices;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chbx_Overfill_Checked(object sender, RoutedEventArgs e)
        {
            txtInitialQuantityChangeTo.Text = "500";
        }
    }

}

