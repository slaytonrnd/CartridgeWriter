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

namespace CartridgeWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeviceManager dm = new DeviceManager();
        private Cartridge c = null;

        public MainWindow()
        {
            InitializeComponent();

            cboDevice.ItemsSource = dm.Devices;
            cboPrinterType.ItemsSource = Machine.GetAllTypes();
            cboMaterialCurrent.ItemsSource = Material.GetAllNames();
            cboMaterialChangeTo.ItemsSource = Material.GetAllNames();
        }

        private void cmdRead_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(cboDevice.Text) || String.IsNullOrEmpty(cboPrinterType.Text))
            {
                MessageBox.Show("I need a Device and Printer Type before I can read.");
                return;
            }
                

            c = dm.ReadCartridge(cboDevice.Text, Machine.FromType(cboPrinterType.Text));

            if (c == null)
                return;

            LoadControls();
        }

        private void cmdWrite_Click(object sender, RoutedEventArgs e)
        {
            if (c == null)
            {
                MessageBox.Show("I need to Read before I can Write.");
                return;
            }
                

            UpdateCartridge();

            // Write the cartridge.
            byte[] result = new byte[3];
            result = dm.WriteCartridge(cboDevice.Text, c);

            LoadControls();
        }

        private void LoadControls()
        {
            txtEEPROMUID.Text = c.EEPROMUID.HexString();
            txtKeyFragment.Text = c.KeyFragment;
            txtSerialNumberCurrent.Text = c.SerialNumber.ToString("f1");
            txtSerialNumberChangeTo.Text = txtSerialNumberCurrent.Text;
            cboMaterialCurrent.Text = c.Material.Name;
            cboMaterialChangeTo.Text = cboMaterialCurrent.Text;
            txtManufacturingLotCurrent.Text = c.ManufacturingLot;
            txtManufacturingLotChangeTo.Text = txtManufacturingLotCurrent.Text;
            txtManufacturingDateCurrent.Text = c.ManfuacturingDate.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            txtManufacturingDateChangeTo.Text = txtManufacturingDateCurrent.Text;
            txtLastUseDateCurrent.Text = c.UseDate.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
            txtLastUseDateChangeTo.Text = txtLastUseDateCurrent.Text;
            txtInitialQuantityCurrent.Text = c.InitialMaterialQuantity.ToString();
            txtInitialQuantityChangeTo.Text = txtInitialQuantityCurrent.Text;
            txtCurrentQuantityCurrent.Text = c.CurrentMaterialQuantity.ToString();
            txtCurrentQuantityChangeTo.Text = txtCurrentQuantityCurrent.Text;
            txtVersionCurrent.Text = c.Version.ToString();
            txtVersionChangeTo.Text = txtVersionCurrent.Text;
            txtSignatureCurrent.Text = c.Signature;
            txtSignatureChangeTo.Text = txtSignatureCurrent.Text;
        }

        /// <summary>
        /// Update any changed Cartridge properties.
        /// </summary>
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
                c.InitialMaterialQuantity = double.Parse(txtSerialNumberChangeTo.Text);

            if (!txtCurrentQuantityCurrent.Text.Equals(txtCurrentQuantityChangeTo.Text))
                c.CurrentMaterialQuantity = double.Parse(txtCurrentQuantityChangeTo.Text);

            if (!txtVersionCurrent.Text.Equals(txtVersionChangeTo.Text))
                c.Version = ushort.Parse(txtVersionChangeTo.Text);

            if (!txtSignatureCurrent.Text.Equals(txtSignatureChangeTo.Text))
                c.Signature = txtSignatureChangeTo.Text;
        }

        private void cboDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SetEnable();
        }

        private void cboPrinterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SetEnable();
        }

        private void SetEnable()
        {
            if (cboDevice.Text != string.Empty & cboPrinterType.Text != string.Empty)
                cmdRead.IsEnabled = true;
            else
            {
                cmdRead.IsEnabled = false;
                txtEEPROMUID.Text = string.Empty;
                txtKeyFragment.Text = string.Empty;
            }


            if (cmdRead.IsEnabled & !string.IsNullOrEmpty(txtEEPROMUID.Text) & !string.IsNullOrEmpty(txtKeyFragment.Text))
            {

            }
        }
    }
}
