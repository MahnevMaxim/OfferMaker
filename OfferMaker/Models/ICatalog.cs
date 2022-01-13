using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public interface ICatalog
    {
        public ObservableCollection<Nomenclature> Nomenclatures { get; set; }
    }
}
