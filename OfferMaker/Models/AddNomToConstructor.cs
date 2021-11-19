using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class AddNomToConstructor : BaseModel
    {
        #region MVVVM 

        #region Fields

        Nomenclature selectedNomenclature;
        NomenclatureGroup selectedNomenclatureGroup;
        ObservableCollection<Category> categoriesTree;
        Category selectedCat;

        #endregion Fields

        #region Propetries

        public Nomenclature SelectedNomenclature { get; set; }

        public NomenclatureGroup SelectedNomenclatureGroup { get; set; }

        public ObservableCollection<Nomenclature> Nomenclatures { get; set; }

        public ObservableCollection<NomenclatureGroup> NomenclatureGroups { get; set; }

        public ObservableCollection<Category> CategoriesTree
        {
            get { return categoriesTree; }
            set
            {
                categoriesTree = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCat
        {
            get { return selectedCat; }
            set
            {
                selectedCat = value;
                OnPropertyChanged();
            }
        }

        #endregion Propetries

        #endregion MVVVM 

        OfferGroup offerGroup;

        public AddNomToConstructor(OfferGroup offerGroup)
        {
            Nomenclatures = Global.Main.Catalog.Nomenclatures;
            CategoriesTree = Global.Main.Catalog.CategoriesTree;
            NomenclatureGroups = Global.Main.Catalog.NomenclatureGroups;
            this.offerGroup = offerGroup;
        }

        /// <summary>
        /// Добавление номенклатуры в конструктор.
        /// </summary>
        public void AddNomenclature()
        {
            offerGroup.NomWrappers.Add(new NomWrapper() { Nomenclature = SelectedNomenclature });
            Close();
        }
    }
}
