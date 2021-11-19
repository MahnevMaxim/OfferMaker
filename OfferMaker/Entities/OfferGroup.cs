using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class OfferGroup : BaseModel
    {
        string groupTitle;

        public int Id { get; set; }

        public string GroupTitle 
        {
            get => groupTitle;
            set
            {
                groupTitle = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NomWrapper> NomWrappers { get; set; } = new ObservableCollection<NomWrapper>();
    }
}
