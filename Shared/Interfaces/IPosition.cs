using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    interface IPosition
    {
        public int Id { get; set; }

        public string PositionName { get; set; }

        public ObservableCollection<Permissions> Permissions { get; set; }
    }
}
