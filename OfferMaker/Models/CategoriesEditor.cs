using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class CategoriesEditor : BaseModel
    {
        ObservableCollection<Category> Categories = new ObservableCollection<Category>(); 

        override internal void Run()
        {

        }
    }
}
