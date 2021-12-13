using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    /// <summary>
    /// Объект картинки для номенклатуры.
    /// </summary>
    public class NomImage
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public int CreatorId { get; set; }

        public string OriginalHash { get; set; }
    }
}
