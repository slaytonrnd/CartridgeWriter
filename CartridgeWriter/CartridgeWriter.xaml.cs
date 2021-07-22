// Edited by Thomas Mayr (2021) to work fo the uPrint series; copyright below.

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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;
using System.IO;


namespace CartridgeWriter
{
    public partial class MainWindow : Window
    {
        private Cartridge c = null;
        private SerialPort serialPort; // serial port for communcating

        AboutWindow aboutWindow;
        HelpWindow helpWindow;

        public MainWindow()
        {
            InitializeComponent();
            txtRecevedCode.AcceptsReturn = true;
            txtRecevedCode.IsReadOnly = false;
            cboPrinterType.ItemsSource = Machine.GetAllTypes();
            cboMaterialCurrent.ItemsSource = Material.GetAllNames();
            cboMaterialChangeTo.ItemsSource = Material.GetAllNames();
            cboBays.ItemsSource = Bay.GetAllNames();
            cboDevices.ItemsSource = SerialPort.GetPortNames();
        }

        //Read out the hexcode over the serial port
        public void cmdReadSerial_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(cboDevices.Text) || String.IsNullOrEmpty(cboBays.Text))
            {
                MessageBox.Show("A Serial Port and a Cartridge must be selected!");
                return;
            }
            try
            {
                InitComInterface(cboDevices.Text);
                serialPort.DiscardInBuffer();
                serialPort.Write(Bay.FromName(cboBays.Text).code_read);
                serialPort.Write("\r\n");
                System.Threading.Thread.Sleep(2000);
                txtRecevedCode.Text = serialPort.ReadExisting();
                if (String.IsNullOrEmpty(txtRecevedCode.Text)) MessageBox.Show("Nothing received! Make sure the printer is connected.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            catch (Exception ex)
            {
                Error_Output(ex.Message);
            }
            serialPort.Close();
        }

        //decrypt hexcode from the cartridge to get real values
        private void cmdRead_Click(object sender, RoutedEventArgs e)
        {
            string input_flash = txtRecevedCode.Text;

            if (String.IsNullOrEmpty(cboPrinterType.Text))
            {
                MessageBox.Show("A Printer Type is needed before the data can be decrypted.");
                return;
            }

            if (String.IsNullOrEmpty(txtRecevedCode.Text))
            {
                MessageBox.Show("There is nothing to decrypt.\nMake sure to receive data before to try to decrypt.");
                return;
            }

            try
            {
                c = ReadCartridge(Machine.FromType(cboPrinterType.Text), input_flash);
            }
            catch (Exception ex)
            {
                Error_Output(ex.Message);
                return;
            }
            if (c == null) return;
            LoadControls();
            DisableInputs(true);

        }

        /* Generate a random number as new serial number */
        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (txtSerialNumberCurrent.Text != String.Empty)
            {
                Random zufall = new Random();
                int number = zufall.Next(10000000, 99999999);
                txtSerialNumberChangeTo.Text = number.ToString("f1");
            }
        }

        /* Clear all entries and restart the program  */
        private void cmdRestart_Click(object sender, RoutedEventArgs e)
        {
            clear_all_entries();
        }

        //Create and Write the new hexcode
        private void cmdWrite_Click(object sender, RoutedEventArgs e)
        {
            if (c == null)
            {
                MessageBox.Show("The data must be read and decrypted before it can be written.");
                return;
            }

            if (String.IsNullOrEmpty(cboBays.Text) | String.IsNullOrEmpty(cboDevices.Text))
            {
                MessageBox.Show("A Serial Port and a Cartridge must be selected!");
                return;
            }

            UpdateCartridge();
            LoadControls();
            try
            {
                string newflash = Bay.FromName(cboBays.Text).code_write + c.Encrypted.CreateOutput();
                InitComInterface(cboDevices.Text);
                serialPort.DiscardInBuffer();
                serialPort.Write(newflash);
                serialPort.Write("\r\n");
                serialPort.Close();
                System.Threading.Thread.Sleep(2000);
                MessageBox.Show("Please check if it worked, by removing and reinserting the cartridge!", "Finished!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                clear_all_entries();
            }
            catch (Exception ex)
            {
                Error_Output(ex.Message);
            }
        }



        /*Automatically change the current quantity value to the inital quantity value*/
        private void txtInitialQuantityChangeTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCurrentQuantityChangeTo.Text = txtInitialQuantityChangeTo.Text;
        }

        public Cartridge ReadCartridge(Machine machine, string flashstring)
        {
            byte[] flash; // Content of the eeprom 
            byte[] rom; // ID of the eeprom

            Cartridge c = null;
            {
                try
                {
                    rom = flashstring.ExtractEepromID().ToByteArray();
                    flash = flashstring.ExraxtEepromCode().ToByteArray();
                }
                catch
                {
                    throw new Exception("Input not the right Form");
                }
            }

            if (BitConverter.IsLittleEndian) rom = rom.Reverse();
            if (chbxBackup.IsChecked ?? false) SaveFlashToFile(flashstring);
            c = new Cartridge(flash, machine, rom);
            return c;
        }

        /* Sets the text to the textboxes */
        private void LoadControls()
        {
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
        }

        private void DisableInputs(Boolean disable)
        {
            cboDevices.IsEnabled = !disable;
            cboBays.IsEnabled = !disable;
            cboPrinterType.IsEnabled = !disable;
            cmdReadSerial.IsEnabled = !disable;
        }


        /* Update any changed Cartridge properties. */
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
                c.InitialMaterialQuantity = double.Parse(txtInitialQuantityChangeTo.Text) / 16.3871;

            if (!txtCurrentQuantityCurrent.Text.Equals(txtCurrentQuantityChangeTo.Text))
                c.CurrentMaterialQuantity = double.Parse(txtCurrentQuantityChangeTo.Text) / 16.3871;

            if (!txtVersionCurrent.Text.Equals(txtVersionChangeTo.Text))
                c.Version = ushort.Parse(txtVersionChangeTo.Text);

            if (!txtSignatureCurrent.Text.Equals(txtSignatureChangeTo.Text))
                c.Signature = txtSignatureChangeTo.Text;
        }

        /* clears all entries */
        private void clear_all_entries()
        {
            c = null;
            cboBays.Text = null;
            cboDevices.Text = null;
            cboPrinterType.Text = null;
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
            txtRecevedCode.Text = null;
            DisableInputs(false);
        }

        public void Error_Output(string error)
        {
            MessageBox.Show(error + "\nTry again!\nMaybe the wrong Device is connected or the wrong Printer Type is selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            clear_all_entries();
        }

        /* Open About Window */
        private void credits_Click(object sender, RoutedEventArgs e)
        {
            if (this.aboutWindow == null)
            {
                this.aboutWindow = new AboutWindow();
                this.aboutWindow.Closed += (cw, args) => this.aboutWindow = null;
                this.aboutWindow.Show();
            }
        }

        /* Open Help Window */
        private void help_Click(object sender, RoutedEventArgs e)
        {
            if (this.helpWindow == null)
            {
                this.helpWindow = new HelpWindow();
                this.helpWindow.Closed += (cw, args) => this.helpWindow = null;
                this.helpWindow.Show();
            }
        }

        /* Initialise the Serial Interface */
        public void InitComInterface(string port)
        {
                serialPort = new SerialPort(port);
                serialPort.BaudRate = 38400;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.RequestToSend;
                serialPort.DtrEnable = true;
                serialPort.WriteTimeout = 500;
                serialPort.Open();
        }


        /* Save a file of the DS2433 chip contents */
        private void SaveFlashToFile(string input_flash)
        {
            string path = @".\EEPROMFiles";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = path + @"\" + input_flash.ExtractEepromID().Replace(" ", String.Empty);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            DateTime now = DateTime.Now;
            path = path + @"\" + now.ToString("dd.MM.yyyy_HH.mm.ss") + ".txt";
            File.WriteAllText(path, input_flash);
        }
    }

}

