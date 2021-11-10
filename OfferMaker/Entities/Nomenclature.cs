using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Nomenclature 
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Category Category { get; set; }

        public ObservableCollection<string> Description { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Markup { get; set; }

        public decimal Price { get; set; }

        public decimal Profit { get; set; }

        public int CurrencyIsoCode { get; set; }

        public DateTime? LastChangePriceDate { get; set; }

        public int ActualPricePeriod { get; set; }

        public bool IsPriceActual { get; set; }

        public ObservableCollection<string> Photos { get; set; }
    }
}
