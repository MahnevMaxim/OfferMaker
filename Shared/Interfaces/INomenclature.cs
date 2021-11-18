using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;

namespace Shared
{
    public interface INomenclature
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Category Category { get; set; }

        public ObservableCollection<Description> Descriptions { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Markup { get; set; }

        [NotMapped]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal Profit { get; set; }

        public string CurrencyCharCode { get; set; }

        public DateTime? LastChangePriceDate { get; set; }

        public int ActualPricePeriod { get; set; }

        public bool IsPriceActual { get; set; }

        public ObservableCollection<string> Photos { get; set; }
    }
}
