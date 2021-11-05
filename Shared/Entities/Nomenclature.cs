using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Nomenclature : INomenclature
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Description { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Markup { get; set; }

        public decimal Price { get; set; }

        public decimal Profit { get; set; }

        public ICurrency Currency { get; set; }

        public DateTime? LastChangePriceDate { get; set; }

        public int ActualPricePeriod { get; set; }

        public bool IsPriceActual { get; set; }

        public IEnumerable<string> Photos { get; set; }
    }
}
