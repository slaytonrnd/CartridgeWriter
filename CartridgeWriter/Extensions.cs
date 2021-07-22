// Some methods, which are needed for the uPrint (SE) version, werer added by Thomas Mayr.
// Class was originally created by David Slayton (copyrigth below)

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

using System;
using System.Globalization;

namespace CartridgeWriterExtensions
{
    public static class ByteExtensions
    {
        public static byte[] Reverse(this byte[] bytes)
        {
            int len = bytes.Length;
            byte[] reversed = new byte[len];

            for (int i = 0; i < len; i++)
                reversed[len - i - 1] = bytes[i];

            return reversed;
        }

        public static string HexString(this byte[] bytes)
        {
            string hexString = string.Empty;
            foreach (byte b in bytes)
                hexString = hexString + b.ToString("x2");
            return hexString;
        }


        /*Methods needed for the uPrint Version: */

        /* convert the hexcode to a byte array, so it is useable for decryption*/
        public static byte[] ToByteArray(this string hexString)
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

        /* Creating the output string in the correct form to write it back over the serialport to the chip */
        public static string CreateOutput(this byte[] output_flash)
        {
            return "\"" + BitConverter.ToString(output_flash).Replace("-", ",") + "\"";
        }

        /* extract the hexcode of the eerprom content from the rest of the string sent over the serial port */
        public static string ExraxtEepromCode(this string uncut)
        {
            string[] splittedStrings = uncut.Split('\r');
            string code = "";
            for (int i = 4; i < 12; i++) code += splittedStrings[i].Substring(8, 48);
            return code;
        }

        /* extract the hexcode of the ID of the eeprom from the rest of the string sent over the serial port */
        public static string ExtractEepromID(this string uncut)
        {
            return uncut.Split('\r')[1].Substring(8, 24);
        }
    }
}
