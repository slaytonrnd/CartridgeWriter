// Ported from material.py by David Slayton (2014); copyright below.

// Copyright (c) 2013, Benjamin Vanheuverzwijn <bvanheu@gmail.com> (Original version)
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
using System.Windows;

namespace CartridgeWriter
{
    //
    // A material is a type of plastic used to make 3D prototype
    // It might have some information like optimal working temperature, etc.
    //
    public class Material
    {
        private static List<Material> materials = new List<Material>
            {
                new Material() {Id=0x00,Name="ABS"},
                new Material() {Id=0x01,Name="ABS_RED"},
                new Material() {Id=0x02,Name="ABS_GRN"},
                new Material() {Id=0x03,Name="ABS_BLK"},
                new Material() {Id=0x04,Name="ABS_YEL"},
                new Material() {Id=0x05,Name="ABS_BLU"},
                new Material() {Id=0x06,Name="ABS_CST"},
                new Material() {Id=0x07,Name="ABSI"},
                new Material() {Id=0x08,Name="ABSI_RED"},
                new Material() {Id=0x09,Name="ABSI_GRN"},
                new Material() {Id=0x0a,Name="ABSI_BLK"},
                new Material() {Id=0x0b,Name="ABSI_YEL"},
                new Material() {Id=0x0c,Name="ABSI_BLU"},
                new Material() {Id=0x0d,Name="ABSI_AMB"},
                new Material() {Id=0x0e,Name="ABSI_CST"},
                new Material() {Id=0x0f,Name="ABS_S"},
                new Material() {Id=0x10,Name="PC"},
                new Material() {Id=0x11,Name="PC_RED"},
                new Material() {Id=0x12,Name="PC_GRN"},
                new Material() {Id=0x13,Name="PC_BLK"},
                new Material() {Id=0x14,Name="PC_YEL"},
                new Material() {Id=0x15,Name="PC_BLU"},
                new Material() {Id=0x16,Name="PC_CST"},
                new Material() {Id=0x17,Name="PC_S"},
                new Material() {Id=0x18,Name="ULT9085"},
                new Material() {Id=0x19,Name="ULT_RED"},
                new Material() {Id=0x1a,Name="ULT_GRN"},
                new Material() {Id=0x1b,Name="ULT_BLK"},
                new Material() {Id=0x1c,Name="ULT_YEL"},
                new Material() {Id=0x1d,Name="ULT_BLU"},
                new Material() {Id=0x1e,Name="ULT_CST"},
                new Material() {Id=0x1f,Name="ULT_S"},
                new Material() {Id=0x20,Name="PPSF"},
                new Material() {Id=0x21,Name="PPSF_RED"},
                new Material() {Id=0x22,Name="PPSF_GRN"},
                new Material() {Id=0x23,Name="PPSF_BLK"},
                new Material() {Id=0x24,Name="PPSF_YEL"},
                new Material() {Id=0x25,Name="PPSF_BLU"},
                new Material() {Id=0x26,Name="PPSF_CST"},
                new Material() {Id=0x27,Name="PPSF_S"},
                new Material() {Id=0x28,Name="P400SR"},
                new Material() {Id=0x29,Name="P401"},
                new Material() {Id=0x2a,Name="P401_RED"},
                new Material() {Id=0x2b,Name="P401_GRN"},
                new Material() {Id=0x2c,Name="P401_BLK"},
                new Material() {Id=0x2d,Name="P401_YEL"},
                new Material() {Id=0x2e,Name="P401_BLU"},
                new Material() {Id=0x2f,Name="P401_CST"},
                new Material() {Id=0x30,Name="ABS_SGRY"},
                new Material() {Id=0x31,Name="ABS_GRY"},
                new Material() {Id=0x32,Name="ABSI_GRY"},
                new Material() {Id=0x3c,Name="P430"},
                new Material() {Id=0x3d,Name="P430_RED"},
                new Material() {Id=0x3e,Name="P430_GRN"},
                new Material() {Id=0x3f,Name="P430_BLK"},
                new Material() {Id=0x40,Name="P430_YEL"},
                new Material() {Id=0x41,Name="P430_BLU"},
                new Material() {Id=0x42,Name="P430_CST"},
                new Material() {Id=0x43,Name="P430_GRY"},
                new Material() {Id=0x44,Name="P430_NYL"},
                new Material() {Id=0x45,Name="P430_ORG"},
                new Material() {Id=0x46,Name="P430_FLS"},
                new Material() {Id=0x47,Name="P430_IVR"},
                new Material() {Id=0x50,Name="ABS-M30I"},
                new Material() {Id=0x51,Name="ABS-ESD7"},
                new Material() {Id=0x5a,Name="NYL12"},
                new Material() {Id=0x64,Name="PCABSWHT"},
                new Material() {Id=0x65,Name="PCABSRED"},
                new Material() {Id=0x66,Name="PCABSGRN"},
                new Material() {Id=0x67,Name="PC-ABS"},
                new Material() {Id=0x68,Name="PCABSYEL"},
                new Material() {Id=0x69,Name="PCABSBLU"},
                new Material() {Id=0x6a,Name="PCABSCST"},
                new Material() {Id=0x6b,Name="PCABSGRY"},
                new Material() {Id=0x78,Name="SR20"},
                new Material() {Id=0x82,Name="PC_SR"},
                new Material() {Id=0x8c,Name="ABS-M30"},
                new Material() {Id=0x8d,Name="M30_RED"},
                new Material() {Id=0x8e,Name="M30_GRN"},
                new Material() {Id=0x8f,Name="M30_BLK"},
                new Material() {Id=0x90,Name="M30_YEL"},
                new Material() {Id=0x91,Name="M30_BLU"},
                new Material() {Id=0x92,Name="M30_CST"},
                new Material() {Id=0x93,Name="M30_GRY"},
                new Material() {Id=0x94,Name="M30_SGRY"},
                new Material() {Id=0x95,Name="M30_WHT"},
                new Material() {Id=0x96,Name="M30_SIL"},
                new Material() {Id=0xa0,Name="ABS_S_2"},
                new Material() {Id=0xaa,Name="ABS_SS"},
                new Material() {Id=0xab,Name="SR30"},
                new Material() {Id=0xad,Name="ULT_S2"},
                new Material() {Id=0xae,Name="SR-100"},
                new Material() {Id=0xaf,Name="ULTM-BLK"},
                new Material() {Id=0xb0,Name="SR-110"},
                new Material() {Id=0xb1,Name="SR35"},
                new Material() {Id=0xb4,Name="PC-ISO"},
                new Material() {Id=0xbe,Name="PC-ISO-T"},
                new Material() {Id=0xbf,Name="P1_5M1"},
                new Material() {Id=0xc0,Name="P1_5M2"},
                new Material() {Id=0xc1,Name="P1_5M3"},
                new Material() {Id=0xc6,Name="RDdev"},
                new Material() {Id=0xc7,Name="RDdev-S"},
                new Material() {Id=0xc8,Name="RD1"},
                new Material() {Id=0xc9,Name="RD2"},
                new Material() {Id=0xca,Name="RD3"},
                new Material() {Id=0xcb,Name="RD4"},
                new Material() {Id=0xcc,Name="RD5"},
                new Material() {Id=0xcd,Name="RD-S1"},
                new Material() {Id=0xce,Name="RD-S2"},
                new Material() {Id=0xcf,Name="RD-S3"},
                new Material() {Id=0xd0,Name="RD-S4"},
                new Material() {Id=0xd1,Name="RD-S5"},
                new Material() {Id=0xd3,Name="SR30L"},
                new Material() {Id=0xdd,Name="P430L_IVR"},
                new Material() {Id=0xfa,Name="uP430"},
                new Material() {Id=0xfb,Name="uP430_RED"},
                new Material() {Id=0xfc,Name="uP430_GRN"},
                new Material() {Id=0xfd,Name="uP430_BLK"},
                new Material() {Id=0xfe,Name="uP430_YEL"},
                new Material() {Id=0xff,Name="uP430_BLU"},
                new Material() {Id=0x100,Name="uP430_GRY"},
                new Material() {Id=0x118,Name="SR30XL"},
                new Material() {Id=0x119,Name="P430XL_IVR"},
                new Material() {Id=0x11a,Name="P430XL"},
                new Material() {Id=0x11b,Name="P430XL_RED"},
                new Material() {Id=0x11c,Name="P430XL_GRN"},
                new Material() {Id=0x11d,Name="P430XL_BLK"},
                new Material() {Id=0x11e,Name="P430XL_YEL"},
                new Material() {Id=0x11f,Name="P430XL_BLU"},
                new Material() {Id=0x12c,Name="ASA"},
                new Material() {Id=0x12d,Name="ASA_BLK"},
                new Material() {Id=0x12e,Name="ASA_LGRY"},
                new Material() {Id=0x12f,Name="ASA_RED"},
                new Material() {Id=0x130,Name="ASA_BLU"},
                new Material() {Id=0x131,Name="ASA_GRN"},
                new Material() {Id=0x132,Name="ASA_WHT"},
                new Material() {Id=0x133,Name="ASA_YEL"},
                new Material() {Id=0x134,Name="ASA_ORG"},
                new Material() {Id=0x135,Name="ASA_DGRY"},
                new Material() {Id=0x136,Name="ULT1010"},
                new Material() {Id=0x137,Name="U1010BLK"},
                new Material() {Id=0x138,Name="U1010S1"},
                new Material() {Id=0x140,Name="U9085CG"},
                new Material() {Id=0x154,Name="NYL6"},
                new Material() {Id=0x15e,Name="PCABS-FR"},
                new Material() {Id=0x168,Name="ST130"},
                new Material() {Id=0x169,Name="ST130_S"},
                // Duplicate names, ending with '_2'},
                new Material() {Id=0x1f4,Name="ABS-M30_2"},
                new Material() {Id=0x1f7,Name="M30_BLK_2"},
                new Material() {Id=0x208,Name="PC_2"},
                new Material() {Id=0x212,Name="ULT9085_2"},
                new Material() {Id=0x226,Name="ASA_2"},
                new Material() {Id=0x227,Name="ASA_BLK_2"},
                new Material() {Id=0x244,Name="NYL12_2"},
                new Material() {Id=0x384,Name="SR30_2"},
                new Material() {Id=0x385,Name="SR-110_2"},
                new Material() {Id=0x386,Name="PC_S_2"},
                new Material() {Id=0x387,Name="ULT_S_2"},
                new Material() {Id=0x388,Name="SR35_2"},
                new Material() {Id=0xe3,Name="ABS_DGRY (HP)"} //For HP Designjet 3D
            };

        private Material() { }

        public double Id { get; private set; }
        public string Name { get; private set; }

        public static Material FromId(double id)

        {
            /*If the Material is not known, add it to the list as "other", so that the program won't crash*/
            if (materials.Where(m => m.Id.Equals(id)).FirstOrDefault() == null)
            {
               materials.Add(new Material() { Id = id, Name = "other: \"0x" + ((int)id).ToString("x") + "\""});
            }
            return materials.Where(m => m.Id.Equals(id)).FirstOrDefault();
        }
        public static Material FromName(string name) { return materials.Where(m => m.Name.Equals(name)).First(); }
        public static IEnumerable<string> GetAllNames() {return materials.Select(m => m.Name); }
    }
}
