using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class AddNomToConstructor : BaseModel, ICatalog
    {
        #region MVVVM 

        #region Fields

        CatalogFilter catalogFilter;
        ObservableCollection<Category> categoriesTree;
        Category selectedCat;

        #endregion Fields

        #region Propetries

        public CatalogFilter CatalogFilter
        {
            get => catalogFilter;
            set
            {
                catalogFilter = value;
            }
        }

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
            Nomenclatures = Global.Main.Catalog.GetNomenclatures();
            CategoriesTree = Global.Main.Catalog.CategoriesTree;
            NomenclatureGroups = Global.Main.Catalog.NomenclatureGroups;
            this.offerGroup = offerGroup;
            CatalogFilter = new CatalogFilter(this);
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

            //проверяем, есть ли такая же номенклатура в группе
            var res = offerGroup.NomWrappers.Where(n=>n.Nomenclature.Guid==SelectedNomenclature.Guid).FirstOrDefault();
            if(res==null)
            {
                Nomenclature nomenclature = Helpers.CloneObject<Nomenclature>(SelectedNomenclature);
                offerGroup.NomWrappers.Add(new NomWrapper(offerGroup, nomenclature));
            }
            else
            {
                res.Amount++;
            }

            offerGroup.OnPropertyChanged(string.Empty);
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
            foreach(var nomen in SelectedNomGroup.Nomenclatures)
            {
                var nom = Helpers.CloneObject<Nomenclature>(nomen);
                //проверяем, есть ли такая же номенклатура в группе
                var res = offerGroup.NomWrappers.Where(n => n.Nomenclature.Guid == nomen.Guid).FirstOrDefault();
                if (res == null)
                {
                    Nomenclature nomenclature = Helpers.CloneObject<Nomenclature>(nomen);
                    offerGroup.NomWrappers.Add(new NomWrapper(offerGroup, nomenclature));
                }
                else
                {
                    res.Amount++;
                }
            }
            offerGroup.OnPropertyChanged(string.Empty);
        }
      

        public void ShowAllCategory() => CatalogFilter.SetMode(FilterMode.All);

        public void ShowWithoutCategory() => CatalogFilter.SetMode(FilterMode.WithoutCat);
    }
}
