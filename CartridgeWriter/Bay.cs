﻿// Copyright (c) 2021, Thomas Mayr <mayr.t@aon.at>
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

using System.Collections.Generic;
using System.Linq;

namespace CartridgeWriter
{
    class Bay
    {
        private static readonly IList<Bay> bays = new List<Bay>
        {
            new Bay() {code_read = "er 0 0 0 128",code_write = "ew 0 0 0 ", Name="Bay 0 Model Material"},
            new Bay() {code_read = "er 1 0 0 128",code_write = "ew 1 0 0 ", Name="Bay 0 Support Material"},
            new Bay() {code_read = "er 0 1 0 128",code_write = "ew 0 1 0 ", Name="Bay 1 Model Material"},
            new Bay() {code_read = "er 1 1 0 128",code_write = "ew 1 1 0 ", Name="Bay 1 Support Material"},

        };

        private Bay() { }

        public string code_read { get; private set; }
        public string code_write { get; private set; }
        public string Name { get; private set; }

        public static Bay FromName(string Name) { return bays.Where(b => b.Name.Equals(Name)).First(); }
        public static IEnumerable<string> GetAllNames() { return bays.Select(b => b.Name); }

    }
}
