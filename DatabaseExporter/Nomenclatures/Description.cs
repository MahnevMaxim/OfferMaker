using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseExporter.Nomenclatures
{
    public partial class Description
    {
        public int ItemId { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }

        public virtual Item Item { get; set; }
    }
}
