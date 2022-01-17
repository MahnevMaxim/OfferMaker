using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    public class NomWrapper
    {
        public Nomenclature Nomenclature { get; set; }

        public int Amount { get; set; }

        public decimal Markup { get; set; }

        public bool IsIncludeIntoOffer { get; set; }

        public bool IsShowPrice { get; set; }

        public string CurrencyCharCode_ { get; set; }
    }
}
