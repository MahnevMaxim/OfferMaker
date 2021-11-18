using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class NomenclatureGroup : INomenclatureGroup
    {
        ObservableCollection<int> nomenclaturesIds = new ObservableCollection<int>();

        public int Id { get; set; }

        public string Name { get; set; }

        public ObservableCollection<int> NomenclaturesIds 
        { 
            get
            {
                ObservableCollection<int> coll = new ObservableCollection<int>();
                Nomenclatures.Select(n=>n.Id).ToList<int>().ForEach(e=>coll.Add(e));
                return coll;
            }
            set { }
        } 


        public ObservableCollection<Nomenclature> Nomenclatures { get; set; } = new ObservableCollection<Nomenclature>();
    }
}
