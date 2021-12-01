using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public class Discount : BaseEntity
    {
        Offer offer;
        decimal discountSum;
        decimal percentage;

        public bool IsEnabled { get; set; }

        public bool IsPercentage
        {
            get;
            set;
        }

        public decimal Percentage
        {
            get => percentage;
            set
            {
                percentage = value;
                if (offer != null)
                {
                    discountSum = offer.TotalSum - (100 - percentage) / 100 * offer.TotalSum;
                    OnPropertyChanged(nameof(PriceWithDiscount));
                    OnPropertyChanged(nameof(DiscountSum));
                    OnPropertyChanged();
                }
            }
        }

        public decimal DiscountSum
        {
            get => discountSum;
            set
            {
                discountSum = value;
                if(offer!=null)
                {
                    if (discountSum < 0)
                    {
                        discountSum = 0;
                        percentage = 0;

                    }
                    else if (discountSum > offer.TotalSum)
                    {
                        discountSum = offer.TotalSum;
                        percentage = 100;
                    }
                    else
                    {
                        percentage = 100 - PriceWithDiscount * 100 / offer.TotalSum;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PriceWithDiscount));
                    OnPropertyChanged(nameof(Percentage));
                }
            }
        }

        public decimal PriceWithDiscount
        {
            get => offer.TotalSum - DiscountSum;
            set
            {
                if(offer!=null)
                {
                    if (value > offer.TotalSum)
                    {
                        percentage = 0;
                        discountSum = 0;
                    }
                    else if (value < 0)
                    {
                        percentage = 100;
                        discountSum = offer.TotalSum;
                    }
                    else
                    {
                        discountSum = offer.TotalSum - value;
                        percentage = 100 - PriceWithDiscount * 100 / offer.TotalSum;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DiscountSum));
                    OnPropertyChanged(nameof(Percentage));
                }
            }
        }

        private Discount() { }

        public Discount(Offer offer) => this.offer = offer;
    }
}
