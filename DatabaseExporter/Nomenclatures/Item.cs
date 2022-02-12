using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseExporter.Nomenclatures
{
    public partial class Item
    {
        public Item()
        {
            Descriptions = new HashSet<Description>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double CostPrice { get; set; }
        public double? MarkUp { get; set; }

        public virtual ICollection<Description> Descriptions { get; set; }
    }
}
