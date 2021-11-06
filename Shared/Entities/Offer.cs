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

        public ObservableCollection<string> Images { get; set; }

        public DateTime CreateDate { get; set; }

        public string CommercialInJson { get; set; }

        public string OfferName { get; set; }

        public Customer Customer { get; set; }

        public decimal TotalSum { get; set; }

        public ObservableCollection<Group> Groups { get; set; }

        public bool IsHiddenTextNds { get; set; }

        public bool IsWithNds { get; set; }

        public Currency Currency { get; set; }
    }
}
