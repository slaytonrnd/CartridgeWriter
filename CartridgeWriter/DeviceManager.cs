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
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows;

namespace CartridgeWriter
{
    public class DeviceManager
    {
        private struct device
        {
            public string Name;
        }

        private IList<device> devices = new List<device>();

        public IEnumerable<string> Devices
        {
            get { return devices.Select(d => d.Name); }
        }

        public DeviceManager() { LoadDevices(); }

        public Cartridge ReadCartridge(string name, Machine machine)
        {
            Cartridge c = null;
            byte[] rom = null;
            byte[] flash = null;

            using (SerialPort sp = new SerialPort(ParseComPortName(devices.Where(d => d.Name.Equals(name)).Select(d => d.Name).First()), 9600))
            {
                sp.ReadTimeout = 3000;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.Handshake = Handshake.None;
                sp.DtrEnable = true;


                if (!sp.IsOpen)
                    sp.Open();

                sp.DiscardInBuffer();

                WaitForChip(sp);

                rom = ReadROM(sp);

                flash = ReadFlash(sp);

                sp.Close();
            }

            if (BitConverter.IsLittleEndian)
                rom = rom.Reverse();

            SaveFlashToFile(rom, flash);

            c = new Cartridge(flash, machine, rom);

            return c;
        }

        public byte[] WriteCartridge(string name, Cartridge c)
        {
            byte[] result = null;

            using (SerialPort sp = new SerialPort(ParseComPortName(devices.Where(d => d.Name.Equals(name)).Select(d => d.Name).First()), 9600))
            {
                sp.ReadTimeout = 3000;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.Handshake = Handshake.None;
                sp.DtrEnable = true;


                if (!sp.IsOpen)
                    sp.Open();

                sp.DiscardInBuffer();

                WaitForChip(sp);

                result = WriteFlash(sp, c.Encrypted);

                sp.Close();
            }

            return result;
        }

        private void LoadDevices()
        {
            SelectQuery q = new SelectQuery("Win32_PNPEntity", "Name LIKE '%(COM%)%'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(q))
            {
                using (ManagementObjectCollection moc = searcher.Get())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        devices.Add(new device { Name = mo["Name"].ToString() });
                    }
                }
            }
        }

        private bool PollForChip(SerialPort sp)
        {
            byte[] buffer = new byte[1];
            sp.Write("x");

            // Pause for buffer to fill.
            while (sp.BytesToRead < 1)
                Thread.Sleep(10);

            sp.Read(buffer, 0, 1);
            return Encoding.ASCII.GetString(buffer).Equals("p");
        }

        private byte[] ReadFlash(SerialPort sp)
        {
            byte[] buffer = new byte[512];

            sp.DiscardInBuffer();

            sp.Write("f");

            // Pause for buffer to fill.
            while (sp.BytesToRead < 512)
                Thread.Sleep(10);

            sp.Read(buffer, 0, 512);

            return buffer;
        }

        private byte[] ReadROM(SerialPort sp)
        {
            byte[] buffer = new byte[8];
            sp.Write("r");

            // Pause for buffer to fill.
            while (sp.BytesToRead < 8)
                Thread.Sleep(10);

            sp.Read(buffer, 0, 8);
            return buffer;
        }

        private void WaitForChip(SerialPort sp)
        {
            while (!PollForChip(sp))
                Thread.Sleep(500);

            sp.DiscardInBuffer();
        }

        private byte[] WriteFlash(SerialPort sp, byte[] flash)
        {
            byte[] result = new byte[3];

            sp.Write("w");
            sp.Write(flash, 0, flash.Length);

            //Pause for buffer to fill.
            while (sp.BytesToRead < 3)
                Thread.Sleep(10);

            sp.Read(result, 0, 3);
            return result;
        }

        // Create a file of the DS2433 chip contents.
        private void SaveFlashToFile(byte[] rom, byte[] flash)
        {
            string path = @".\EEPROMFiles";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + @"\" + rom.HexString();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DateTime now = DateTime.Now;

            path = path + @"\" + now.ToString("yyyyMMdd.HHmmss") + ".bin";

            using (FileStream fs = File.Create(path))
            {
                fs.Write(flash, 0, 512);
                fs.Flush();
                fs.Close();
            }
        }

        // Get the name of the com port.
        private string ParseComPortName(string deviceName)
        {
            string comPortName = string.Empty;

            if (String.IsNullOrEmpty(deviceName))
                return comPortName;

            int startIndex = deviceName.IndexOf("(") + 1;
            int length = deviceName.IndexOf(")") - startIndex;

            if (startIndex > 0 && length > 0)
                comPortName = deviceName.Substring(startIndex, length);

            return comPortName;
        }
    }
}
