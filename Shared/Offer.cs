using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Offer
    {
        public int Id { get; private set; }

        public User OfferCreator { get; set; }

        public User Manager { get; set; }

        public DateTime CreateDate { get; set; }

        public string CommercialInJson { get; set; }

        public string OfferName { get; set; }

        public Customer Customer { get; set; }

        public decimal TotalSum { get; set; }

        public List<Groups> Groups { get; set; }

    }
}
