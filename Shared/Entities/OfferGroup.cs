using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    public class OfferGroup
    {
        public int Id { get; set; }

        public string GroupTitle { get; set; }

        public bool IsOption { get; set; }

        public ObservableCollection<NomWrapper> NomWrappers { get; set; }
    }
}
