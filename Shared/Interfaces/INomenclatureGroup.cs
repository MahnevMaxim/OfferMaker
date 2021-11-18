using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared
{
    public interface INomenclatureGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ObservableCollection<int> NomenclaturesIds { get; set; }
    }
}
