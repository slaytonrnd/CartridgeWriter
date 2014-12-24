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
            new Machine {Number = new byte[8] {0x2C, 0x30, 0x47, 0x8B, 0xB7, 0xDE, 0x81, 0xE8}, Type = "fox"},
            new Machine {Number = new byte[8] {0x2C, 0x30, 0x47, 0x9B, 0xB7, 0xDE, 0x81, 0xE8}, Type = "fox2"},
            new Machine {Number = new byte[8] {0x53, 0x94, 0xD7, 0x65, 0x7C, 0xED, 0x64, 0x1D}, Type = "prodigy"},
            new Machine {Number = new byte[8] {0x76, 0xC4, 0x54, 0xD5, 0x32, 0xE6, 0x10, 0xF7}, Type = "quantum"}
        };

        private Machine() { }

        public string Type { get; private set; }
        public byte[] Number { get; private set; }

        public static Machine FromType(string type) { return Machines.Where(m => m.Type.Equals(type)).First(); }
        public static Machine FromNumber(byte[] number) { return Machines.Where(m => m.Number.SequenceEqual(number)).First(); }
        public static IEnumerable<string> GetAllTypes() { return Machines.Select(m => m.Type); }
    }
}
