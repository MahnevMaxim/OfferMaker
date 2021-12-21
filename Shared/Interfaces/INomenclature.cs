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

        public string Guid { get; set; }

        public string Title { get; set; }

        public string CategoryGuid { get; set; }

        public ObservableCollection<Description> Descriptions { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Markup { get; set; }

        public string CurrencyCharCode { get; set; }

        public DateTime? LastChangePriceDate { get; set; }

        public int ActualPricePeriod { get; set; }

        public Image Image { get; set; }

        public ObservableCollection<Image> Images { get; set; }
    }
}
