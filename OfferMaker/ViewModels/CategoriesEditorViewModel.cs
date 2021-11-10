using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker.ViewModels
{
    public class CategoriesEditorViewModel : BaseViewModel
    {
        CategoriesEditor modelEditor;

        public override void InitializeViewModel()
        {
            modelEditor = (CategoriesEditor)model;
        }
    }
}
