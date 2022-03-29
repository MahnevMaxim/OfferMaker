using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace Shared
{
    public class Offer : ArchiveBase
    {
    }

    abstract public class ArchiveBase : OfferBase
    {
        [Required]
        public ObservableCollection<Currency> Currencies { get; set; }

        public string OldIdentifer { get; set; }
    }
}
