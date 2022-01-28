using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public interface IImage
    {
        public int Id { get; set; }

        public string LocalPhotoPath { get; set; }

        public string Guid { get; set; }
    }
}
