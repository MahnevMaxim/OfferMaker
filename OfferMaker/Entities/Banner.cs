using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public class Banner : Image
    {
        public int Id { get; set; }

        public Banner(string guid, int creatorid, string path) : base(guid, creatorid, path)
        {

        }
    }
}
