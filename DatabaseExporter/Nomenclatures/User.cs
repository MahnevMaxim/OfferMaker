using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseExporter.Nomenclatures
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Email { get; set; }
        public byte[] Foto { get; set; }
    }
}
