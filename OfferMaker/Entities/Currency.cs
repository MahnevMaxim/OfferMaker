using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace OfferMaker
{
    public class Currency : BaseEntity, ICurrency
    {
        bool isEnabled;

        public int Id { get; set; }

        public int IsoCode { get; set; }

        public string CharCode { get; set; }

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public int Nominal { get; set; }

        public string Symbol { get; set; }

        public DateTime RateDatetime { get; set; }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public override string ToString() => CharCode;
    }
}
