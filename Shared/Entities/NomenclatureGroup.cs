using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    public class NomenclatureGroup : INomenclatureGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ObservableCollection<Nomenclature> Nomenclatures { get; set; }
    }
}
