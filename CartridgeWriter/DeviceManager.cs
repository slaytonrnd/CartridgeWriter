// Major portions ported from tool.py by David Slayton (2014); copyright below.

// Copyright (c) 2013 Matthew Goodman
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// The Software shall be used for Good, not Evil.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using CartridgeWriterExtensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CartridgeWriter
{
    public class DeviceManager
    {
        byte[] flash;
        byte[] rom;

        public Cartridge ReadCartridge(Machine machine)
        {
            Cartridge c = null;
            {
                rom = ConvertHexStringToByteArray(clear_ID(MainWindow.input_flash));
                flash = ConvertHexStringToByteArray(clear_code(MainWindow.input_flash));
            }

            if (BitConverter.IsLittleEndian)
                rom = rom.Reverse();


            if(Properties.Settings.Default.Save_to_File)
                SaveFlashToFile(rom, flash);

            c = new Cartridge(flash, machine, rom);
            return c;
        }

        // Save a file of the DS2433 chip contents
        private void SaveFlashToFile(byte[] rom, byte[] flash)
        {
            string path = @".\EEPROMFiles";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + @"\" + clear_ID(MainWindow.input_flash).Replace(" ", String.Empty);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DateTime now = DateTime.Now;
            path = path + @"\" + now.ToString("dd.MM.yyyy_HH.mm.ss") + ".txt";
            File.WriteAllText(path, MainWindow.input_flash);

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
        public static string clear_ID(string uncut)
        {
            string[] splittedStrings = uncut.Split(new[] { "000000: " }, StringSplitOptions.None);
            string clearcode = splittedStrings[1];
            string ID = clearcode.Substring(0, 23);
            return ID;
        }
    }
}
