using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class AddNomToConstructorViewModel : BaseViewModel
    {
        AddNomToConstructor addNom;

        public override void InitializeViewModel()
        {
            addNom = (AddNomToConstructor)model;
        }

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get { return addNom.Nomenclatures; }
            set
            {
                addNom.Nomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> CategoriesTree
        {
            get { return addNom.CategoriesTree; }
            set
            {
                addNom.CategoriesTree = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NomenclatureGroup> NomenclatureGroups
        {
            get { return addNom.NomenclatureGroups; }
            set
            {
                addNom.NomenclatureGroups = value;
                OnPropertyChanged();
            }
        }

        public Nomenclature SelectedNomenclature
        {
            get { return addNom.SelectedNomenclature; }
            set
            {
                addNom.SelectedNomenclature = value;
                OnPropertyChanged();
            }
        }
    }
}
