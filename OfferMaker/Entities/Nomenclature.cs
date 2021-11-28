using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

namespace OfferMaker
{
    public class Nomenclature : BaseEntity
    {
        decimal costPrice;
        decimal markup;
        decimal price;
        decimal profit;
        string currencyCharCode;
        DateTime? lastChangePriceDate;


        public int Id { get; set; }

        public string Title { get; set; }

        public Category Category { get; set; }

        public ObservableCollection<Description> Descriptions { get; set; }

        public decimal CostPrice
        {
            get => costPrice;
            set
            {
                costPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(Profit));
            }
        }

        public decimal Markup
        {
            get => markup;
            set
            {
                markup = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(Profit));
            }
        }

        public decimal Price 
        { 
            get => CostPrice * Markup;
            set
            {
                price = value;
                if (price != 0 && CostPrice != 0)
                {
                    markup = price / costPrice;
                    profit = costPrice * markup - costPrice;
                    LastChangePriceDate = DateTime.Now;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Profit));
                OnPropertyChanged(nameof(Markup));
            }
        }

        public decimal Profit
        {
            get => Price - CostPrice;
            set
            {
                profit = value;
                if (price != 0 && profit != 0)
                {
                    markup = (value + costPrice) / costPrice;
                    price = costPrice * markup;                    
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Markup));
                OnPropertyChanged(nameof(Price));
                
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

        public DateTime? LastChangePriceDate
        {
            get => lastChangePriceDate;
            set
            {
                lastChangePriceDate = value;
                OnPropertyChanged();
            }
        }

        public int ActualPricePeriod { get; set; }

        public bool IsPriceActual { get; set; }

        /// <summary>
        /// Колекция, т.к. не понимаю: нужна одна картинка или несколько
        /// </summary>
        public ObservableCollection<string> Photos { get; set; }
    }
}
