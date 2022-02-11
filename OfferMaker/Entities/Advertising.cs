using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing.Imaging;

namespace OfferMaker
{
    public class Advertising : Image, IImage
    {
        public Advertising(string guid, int creatorid, string path) : base(guid, creatorid, path)
        {
        }
    }
}
