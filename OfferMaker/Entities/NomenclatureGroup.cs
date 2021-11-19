using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace OfferMaker
{
    /// <summary>
    /// Именованная группа номенклатур для каталога.
    /// </summary>
    public class NomenclatureGroup 
    {
        ObservableCollection<Nomenclature> nomenclatures;

        public int Id { get; set; }

        public string Name { get; set; }

        public ObservableCollection<Nomenclature> Nomenclatures 
        { 
            get
            {
                if (nomenclatures == null) nomenclatures = new ObservableCollection<Nomenclature>();
                return nomenclatures;
            }
            set
            {
                nomenclatures = value;
            }
        } 
    }
}
