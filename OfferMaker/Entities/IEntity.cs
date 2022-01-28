using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public interface IEntity
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public bool IsDelete { get; set; }
    }
}
