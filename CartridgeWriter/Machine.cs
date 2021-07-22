// Ported from machine.py by David Slayton (2014); copyright below.

// Copyright (c) 2013, Benjamin Vanheuverzwijn <bvanheu@gmail.com>
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
// DISCLAIMED. IN NO EVENT SHALL <BENJAMIN VANHEUVERZWIJN> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CartridgeWriter
{
    //
    // A machine is a printer from stratasys
    //
    public class Machine
    {
        private static readonly IEnumerable<Machine> Machines = new List<Machine>
        {
			new Machine {Number = new byte[8] {0xF3, 0xA9, 0x1D, 0xBE, 0x6B, 0x0B, 0x22, 0x55}, Type = "uPrint / uPrint Plus"},
			new Machine {Number = new byte[8] {0x09, 0xFB, 0xD4, 0xB6, 0x1F, 0xC0, 0xB3, 0x27}, Type = "uPrint SE / uPrint SE Plus "}
        };


        private Machine() { }

        public string Type { get; private set; }
        public byte[] Number { get; private set; }

        public static Machine FromType(string type) { return Machines.Where(m => m.Type.Equals(type)).First(); }
        public static Machine FromNumber(byte[] number) { return Machines.Where(m => m.Number.SequenceEqual(number)).First(); }
        public static IEnumerable<string> GetAllTypes() { return Machines.Select(m => m.Type); }
    }
}
