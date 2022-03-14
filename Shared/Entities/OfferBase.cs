using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    abstract public class OfferBase
    {
        public int Id { get; set; }

        [Required]
        public string Guid { get; set; }

        [Required]
        public string OfferName { get; set; }

        public int OfferCreatorId { get; set; }

        public int ManagerId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public Customer Customer { get; set; }

        public ObservableCollection<OfferGroup> OfferGroups { get; set; }

        public ObservableCollection<OfferInfoBlock> OfferInfoBlocks { get; set; }

        public bool IsHiddenTextNds { get; set; }

        public bool IsWithNds { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public ObservableCollection<string> AdvertisingsUp { get; set; }

        public ObservableCollection<string> AdvertisingsDown { get; set; }

        public Banner Banner_ { get; set; }

        public bool IsShowPriceDetails { get; set; }

        public bool IsCreateByCostPrice { get; set; }

        public bool IsHideNomsPrice { get; set; }

        public bool IsResultSummInRub { get; set; }

        public Discount Discount { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsDelete { get; set; }

        public string Comment { get; set; }

        [Required]
        public ObservableCollection<Advertising> AdvertisingsUp_ { get; set; }

        [Required]
        public ObservableCollection<Advertising> AdvertisingsDown_ { get; set; }
}
}
