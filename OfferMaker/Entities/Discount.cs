using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class Discount : BaseEntity
    {
        Offer offer;
        decimal discountSum;
        decimal percentage;
        decimal priceWithDiscount;
        decimal totalSum;
        bool isEnabled;

        public decimal TotalSum 
        { 
            get => totalSum;
            set
            {
                totalSum = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public decimal Percentage
        {
            get => percentage;
            set
            {
                percentage = value;
                CalculateByPercentage();
            }
        }

        public decimal DiscountSum
        {
            get => discountSum;
            set
            {
                discountSum = value;
                CalculateByDiscountSum();
            }
        }

        [JsonIgnore]
        public decimal PriceWithDiscount
        {
            get => priceWithDiscount;
            set
            {
                priceWithDiscount = value;
                CalculateByPriceWithDiscount();
            }
        }

        private Discount() { }

        public Discount(Offer offer) => this.offer = offer;

        public void SetOffer(Offer offer) => this.offer = offer;

        internal void CalculateByPriceWithDiscount()
        {
            //независимо от режима расчёта
            if (offer == null || TotalSum <= 0) return;
            if (priceWithDiscount > TotalSum)
            {
                percentage = 0;
                discountSum = 0;
                priceWithDiscount = TotalSum;
            }
            else if (priceWithDiscount < 0)
            {
                priceWithDiscount = 0;
                percentage = 100;
                discountSum = TotalSum;
            }
            else
            {
                discountSum = TotalSum - priceWithDiscount;
                percentage = 100 - priceWithDiscount * 100 / TotalSum;
            }
            offer.UpdateDiscountUI();
        }

        internal void CalculateByDiscountSum()
        {
            //независимо от режима расчёта
            if (offer == null || TotalSum <= 0) return;
            if (discountSum < 0)
            {
                discountSum = 0;
                percentage = 0;
                priceWithDiscount = TotalSum;
            }
            else if (discountSum > TotalSum)
            {
                discountSum = TotalSum;
                percentage = 100;
                priceWithDiscount = 0;
            }
            else
            {
                percentage = 100 - priceWithDiscount * 100 / TotalSum;
                priceWithDiscount = TotalSum - discountSum;
            }
            offer.UpdateDiscountUI();
        }

        internal void CalculateByPercentage()
        {
            //независимо от режима расчёта
            if (offer == null || TotalSum <= 0) return;
            discountSum = TotalSum - (100 - percentage) / 100 * TotalSum;
            priceWithDiscount = TotalSum - discountSum;
            offer.UpdateDiscountUI();
        }

        internal void CalculateByTotalSum()
        {
            if (TotalSum <= 0)
            {
                discountSum = 0;
                priceWithDiscount = 0;
            }
            else
            {
                discountSum = TotalSum - (100 - percentage) / 100 * TotalSum;
                priceWithDiscount = TotalSum - discountSum;
            }
            offer.UpdateDiscountUI();
        }
    }
}
