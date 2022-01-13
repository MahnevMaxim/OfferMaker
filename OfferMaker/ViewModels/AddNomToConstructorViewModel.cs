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

        public CatalogFilter CatalogFilter
        {
            get => addNom.CatalogFilter;
            set
            {
                addNom.CatalogFilter = value;
                OnPropertyChanged();
            }
        }
        public Category SelectedCat
        {
            get => addNom.CatalogFilter.SelectedCat;
            set
            {
                addNom.CatalogFilter.SelectedCat = value;
                OnPropertyChanged();
            }
        }

        public NomenclatureGroup SelectedNomGroup { set => addNom.SelectedNomGroup = value; }

        public Nomenclature SelectedNomenclature { set => addNom.SelectedNomenclature = value; }
    }
}
