using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

namespace OfferMaker
{
    public class Catalog : BaseModel
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups = new ObservableCollection<NomenclatureGroup>();
        NomenclatureGroup selectedNomenclatureGroup;
        ObservableCollection<Category> categoriesTree;
        Category selectedCat;

        #endregion Fields

        #region Propetries

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get { return nomenclatures; }
            set
            {
                nomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NomenclatureGroup> NomenclatureGroups
        {
            get { return nomenclatureGroups; }
            set
            {
                nomenclatureGroups = value;
                OnPropertyChanged();
            }
        }

        public NomenclatureGroup SelectedNomenclatureGroup
        {
            get { return selectedNomenclatureGroup; }
            set
            {
                selectedNomenclatureGroup = value;
                OnPropertyChanged();
            }
        }

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

        #region Propetries

        public ObservableCollection<Category> Categories { get; set; }

        #endregion Propetries

        #region Singleton

        private Catalog() { }

        private static readonly Catalog instance = new Catalog();

        public static Catalog GetInstance() => instance;

        #endregion Singleton

        internal override void Run() => CreateCategoriesTree();

        #region Cats

        private void CreateCategoriesTree() => CategoriesTree = new ObservableCollection<Category>() { new Category() { Title = "Все" } };
        

        public void EditCategories()
        {
            CategoriesEditor editor = new CategoriesEditor(CategoriesTree);
            MvvmFactory.CreateWindow(editor, new ViewModels.CategoriesEditorViewModel(), new Views.CategoriesEditor(), ViewMode.ShowDialog);
        }

        #endregion Cats

        #region Nomenclature

        internal void OpenNomenclurueCard(Nomenclature nomenclature)
        {
            MvvmFactory.CreateWindow(new NomenclurueCard(nomenclature), new ViewModels.NomenclatureCardViewModel(), new Views.NomenclatureCard(), ViewMode.ShowDialog);
        }

        internal void DeleteNomenclurue(Nomenclature nomenclature) => Nomenclatures.Remove(nomenclature);

        internal void DelNomGroup(NomenclatureGroup nomenclatureGroup) => NomenclatureGroups.Remove(nomenclatureGroup);

        internal void AddNomenclatureGroup() => NomenclatureGroups.Add(new NomenclatureGroup() { Name = "Новая группа" });

        internal CallResult AddNomenclatureToGroup(Nomenclature nomenclature)
        {
            if (SelectedNomenclatureGroup!=null)
            {
                SelectedNomenclatureGroup.Nomenclatures.Add(nomenclature);
                return new CallResult();
            }
            else
            {
                return new CallResult() { Error = new Error("Не выбрана группа, в которую нужно добавить номенклатуру")};
            }
        }

        #endregion Nomenclature
    }
}
