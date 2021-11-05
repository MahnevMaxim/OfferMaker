using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface IOffer
    {
        public int Id { get; set; }

        public IUser OfferCreator { get; set; }

        public IUser Manager { get; set; }

        public IEnumerable<string> Images { get; set; }

        public DateTime CreateDate { get; set; }

        public string CommercialInJson { get; set; }

        public string OfferName { get; set; }

        public ICustomer Customer { get; set; }

        public decimal TotalSum { get; set; }

        public IEnumerable<IGroup> Groups { get; set; }

        public bool IsHiddenTextNds { get; set; }

        public bool IsWithNds { get; set; }

        public ICurrency Currency { get; set; }
    }
}
