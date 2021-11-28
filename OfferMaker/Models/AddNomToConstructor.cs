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

        ObservableCollection<Category> categoriesTree;
        Category selectedCat;

        #endregion Fields

        #region Propetries

        public Nomenclature SelectedNomenclature { get; set; }

        public NomenclatureGroup SelectedNomGroup { get; set; }

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

        /// <summary>
        /// Передаём ссылку на группу, в которую будем добавлять номенклатуру.
        /// </summary>
        /// <param name="offerGroup"></param>
        public AddNomToConstructor(OfferGroup offerGroup)
        {
            Nomenclatures = Global.Main.Catalog.Nomenclatures;
            CategoriesTree = Global.Main.Catalog.CategoriesTree;
            NomenclatureGroups = Global.Main.Catalog.NomenclatureGroups;
            this.offerGroup = offerGroup;
        }

        /// <summary>
        /// Добавление номенклатуры в конструктор.
        /// В конструкторе работаем с копией, т.к. нам не нужно, чтобы изменялся оригинальный объект, 
        /// а в конструкторе есть возможность редактирования номеклатуры и описания.
        /// </summary>
        public void AddNomenclature()
        {
            if (SelectedNomenclature == null)
            {
                OnSendMessage("Выберите номенклатуру для добавления");
                return;
            }
            Nomenclature nomenclature = Helpers.CloneObject<Nomenclature>(SelectedNomenclature);
            offerGroup.NomWrappers.Add(new NomWrapper(offerGroup, nomenclature));
            offerGroup.OnPropertyChanged(string.Empty);
            Close();
        }

        /// <summary>
        /// Добавление группы номенклатур в конструктор.
        /// </summary>
        public void AddNomenclatureGroup()
        {
            if(SelectedNomGroup==null)
            {
                OnSendMessage("Выберите группу номенклатур для добавления");
                return;
            }
            List<NomWrapper> list = new List<NomWrapper>();
            foreach(var nomen in SelectedNomGroup.Nomenclatures)
            {
                var nom = Helpers.CloneObject<Nomenclature>(nomen);
                list.Add(new NomWrapper(offerGroup, nom));
            }
            offerGroup.AddNomenclatures(list);
            Close();
        }
    }
}
