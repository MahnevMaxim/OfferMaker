using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Nomenclature : BaseModel
    {
        decimal costPrice;
        decimal markup;
        decimal price;
        decimal profit;
        string currencyCharCode;


        public int Id { get; set; }

        public string Title { get; set; }

        public Category Category { get; set; }

        public ObservableCollection<string> Description { get; set; }

        public decimal CostPrice
        {
            get => costPrice;
            set
            {
                costPrice = value;
                OnPropertyChanged();
            }
        }

        public decimal Markup
        {
            get => markup;
            set
            {
                markup = value;
                OnPropertyChanged();
            }
        }

        public decimal Price 
        { 
            get => CostPrice * Markup;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        public decimal Profit
        {
            get => Price - CostPrice;
            set
            {
                profit = value;
                OnPropertyChanged();
            }
        }

        public string CurrencyCharCode
        {
            get => currencyCharCode;
            set
            {
                currencyCharCode = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastChangePriceDate { get; set; }

        public int ActualPricePeriod { get; set; }

        public bool IsPriceActual { get; set; }

        public ObservableCollection<string> Photos { get; set; }
    }
}
