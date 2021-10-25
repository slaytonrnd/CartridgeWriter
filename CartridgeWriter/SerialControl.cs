// Copyright (c) 2021, Thomas Mayr <mayr.t@aon.at>
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
// DISCLAIMED. IN NO EVENT SHALL <THOMAS MAYR> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using CartridgeWriterExtensions;
using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace CartridgeWriter
{
    class SerialControl
    {
        /* Create Serial Port */
        private static SerialPort InitSerialPort(string port)
        {
            SerialPort serialPort = new SerialPort(port);
            serialPort.BaudRate = 38400;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.DtrEnable = true;
            serialPort.WriteTimeout = 500;
            serialPort.Open();
            return serialPort;
        }

        /*Read the Raw Input String from the Serial */
        public static string readSerial(string port, Bay bay)
        {
            string received;
            SerialPort serialPort = InitSerialPort(port);
            serialPort.DiscardInBuffer();
            serialPort.Write(bay.code_read);
            serialPort.Write("\r\n");
            System.Threading.Thread.Sleep(2000);
            received = serialPort.ReadExisting();
            serialPort.Close();
            if (String.IsNullOrEmpty(received)) MessageBox.Show("Nothing received! Make sure the printer is connected.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return received;
        }

        /*Write the Output stream to the printer*/
        public static void writeSerial(string port, Bay bay, Cartridge c)
        {
            string newflash = bay.code_write + c.Encrypted.CreateOutput();
            SerialPort serialPort = InitSerialPort(port);
            serialPort.DiscardOutBuffer();
            serialPort.Write(newflash);
            serialPort.Write("\r\n");
            serialPort.Close();
            System.Threading.Thread.Sleep(2000);
            MessageBox.Show("Please check if it worked, by removing and reinserting the cartridge!", "Finished!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /* Create Cartridge of the raw data */
        public static Cartridge ReadCartridge(Machine machine, string flashstring, bool save)
        {
            byte[] flash; // Content of the eeprom 
            byte[] rom; // ID of the eeprom
            try
            {
                rom = flashstring.ExtractEepromID().ToByteArray();
                flash = flashstring.ExtractEepromCode().ToByteArray();
            }
            catch
            {
                throw new Exception("Input not the right Form");
            }
            
            if (BitConverter.IsLittleEndian) rom = rom.Reverse();
            if (save) SaveFlashToFile(flashstring);
            Cartridge c = new Cartridge(flash, machine, rom);
            return c;
        }

        /* Save a file of the DS2433 chip contents */
        static private void SaveFlashToFile(string input_flash)
        {
            string path = @".\EEPROMFiles";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = path + @"\" + input_flash.ExtractEepromID().Replace(" ", String.Empty);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            DateTime now = DateTime.Now;
            path = path + @"\" + now.ToString("dd.MM.yyyy_HH.mm.ss") + ".txt";
            File.WriteAllText(path, input_flash.Replace("\r","\r\n"));
        }
    }
}
