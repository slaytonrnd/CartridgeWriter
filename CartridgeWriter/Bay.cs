using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public static Bay FromCode_read(string code_read) { return bays.Where(b => b.code_read.Equals(code_read)).First(); }
        public static Bay FromCode_send(string code_write) { return bays.Where(b => b.code_write.Equals(code_write)).First(); }
        public static Bay FromName(string Name) { return bays.Where(b => b.Name.Equals(Name)).First(); }
        public static IEnumerable<string> GetAllNames() { return bays.Select(b => b.Name); }

    }
}
