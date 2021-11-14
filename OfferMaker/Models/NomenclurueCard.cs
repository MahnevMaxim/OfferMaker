using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public class NomenclurueCard : BaseModel
    {
        Nomenclature Nomenclature { get; set; }

        public NomenclurueCard(Nomenclature nomenclature)
        {
            Nomenclature = nomenclature;
        }
    }
}
