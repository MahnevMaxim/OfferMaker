using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    public class Offer
    {
        public int Id { get; set; }

        public User OfferCreator { get; set; }

        public User Manager { get; set; }

        public DateTime CreateDate { get; set; }

        public string OfferName { get; set; }

        public Customer Customer { get; set; }

        public decimal TotalSum { get; set; }

        public ObservableCollection<OfferGroup> OfferGroups { get; set; }

        public ObservableCollection<OfferInfoBlock> OfferInfoBlocks { get; set; }

        public bool IsHiddenTextNds { get; set; }

        public bool IsWithNds { get; set; }

        public Currency Currency { get; set; }

        public ObservableCollection<string> AdvertisingsUp { get; set; }

        public ObservableCollection<string> AdvertisingsDown { get; set; }

        public string Banner { get; set; }
        
        public bool IsShowPriceDetails { get; set; }

        public bool IsCreateByCostPrice { get; set; }

        public bool IsHideNomsPrice { get; set; }

        public bool ResultSummInRub { get; set; }

        public Discount Discount { get; set; }
    }
}
