using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using System.Windows;

namespace OfferMaker
{
    public class Catalog : BaseModel
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<Nomenclature> filteredNomenclatures;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups = new ObservableCollection<NomenclatureGroup>();
        NomenclatureGroup selectedNomenclatureGroup;
        ObservableCollection<Category> categoriesTree;
        Category selectedCat;
        string searchStringInCatalog;
        CatalogFilter catalogFilter;
        ObservableCollection<Category> categories;

        #endregion Fields

        #region Propetries

        /// <summary>
        /// FilteredNomenclatures, потому-что номенклатуры отображаются через фильтр.
        /// </summary>
        public ObservableCollection<Nomenclature> FilteredNomenclatures
        {
            get { return filteredNomenclatures; }
            set
            {
                filteredNomenclatures = value;
                OnPropertyChanged();
            }
        }

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



        public string SearchStringInCatalog
        {
            set
            {
                searchStringInCatalog = value;
            }
        }

        public CatalogFilter CatalogFilter
        {
            get => catalogFilter;
            set
            {
                catalogFilter = value;
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
            }
        }

        #endregion Propetries

        #endregion MVVVM 

        #region Propetries

        #endregion Propetries

        string newCatName = "Новая категория";

        #region Singleton

        private Catalog() { }

        private static readonly Catalog instance = new Catalog();

        public static Catalog GetInstance(ObservableCollection<Nomenclature> nomenclatures,
                                          ObservableCollection<Category> categories,
                                          ObservableCollection<NomenclatureGroup> nomenclatureGroups)
        {
            instance.Nomenclatures = nomenclatures;
            instance.NomenclatureGroups = nomenclatureGroups;
            instance.Categories = categories;
            return instance;
        }

        #endregion Singleton

        internal override void Run()
        {
            CreateCategoriesTree();
            CatalogFilter = new CatalogFilter(this);
        }

        #region Categories

        internal void RemoveNomFromCat(Nomenclature nomenclature)
        {
            if (nomenclature.CategoryGuid != null)
            {
                Category cat = GetCatFromTree(nomenclature.CategoryGuid);
                if (cat != null) cat.Nomenclatures.Remove(nomenclature);
            }
        }

        private void CreateCategoriesTree()
        {
            CategoriesTree = new ObservableCollection<Category>();
            //не установленный ParentGuid даёт гарантию, что родитель отсутствует
            var roots = categories.Where(c => c.ParentGuid == null).ToList();
            SetChilds(CategoriesTree, roots);
        }

        internal void ShowAllCategory() => CatalogFilter.SetMode(FilterMode.All);

        internal void ShowWithoutCategory() => CatalogFilter.SetMode(FilterMode.WithoutCat);

        private void SetNOmenclatureCache(Category ch) => Nomenclatures.Where(n => n.CategoryGuid == ch.Guid).ToList().ForEach(n => ch.Nomenclatures.Add(n));

        private void SetChilds(ObservableCollection<Category> targetCollection, List<Category> childs)
        {
            foreach (var ch in childs)
            {
                SetNOmenclatureCache(ch);
                targetCollection.Add(ch);
                var childs_ = categories.Where(c => c.ParentGuid != null && c.ParentGuid == ch.Guid).ToList();
                SetChilds(ch.Childs, childs_);
            }
        }

        internal void AddCategory() => CategoriesTree.Add(new Category(newCatName));

        public void EditCategories()
        {
            CategoriesEditor editor = new CategoriesEditor(CategoriesTree);
            MvvmFactory.CreateWindow(editor, new ViewModels.CategoriesEditorViewModel(), new Views.CategoriesEditor(), ViewMode.ShowDialog);
        }

        /// <summary>
        /// Редактирование заголовка категории.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="position"></param>
        internal void EditCategory(Category category, Point position)
        {
            SimpleViews.CategoryEditor f = new SimpleViews.CategoryEditor(category);
            f.Top = position.Y + 10;
            f.Left = position.X;
            f.Show();
        }

        /// <summary>
        /// Удаление категории.
        /// </summary>
        /// <param name="category"></param>
        internal void DelCategory(Category category)
        {
            DeleteCatFromTree(category, CategoriesTree);
            SetCatGuidNull(category);
        }

        private void SetCatGuidNull(Category category) => nomenclatures.Where(n => n.CategoryGuid == category.Guid).ToList().ForEach(n => n.CategoryGuid = null);

        /// <summary>
        /// Обход дерева для удаления.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="coll"></param>
        private void DeleteCatFromTree(Category category, ObservableCollection<Category> coll)
        {
            var res = coll.Where(c => c.Guid == category.Guid).FirstOrDefault();
            if (res != null)
            {
                coll.Remove(res);
                return;
            }
            coll.ToList().ForEach(c => { DeleteCatFromTree(category, c.Childs); });
        }

        private Category GetCatFromTree(string guid, ObservableCollection<Category> coll = null)
        {
            if (coll == null) coll = CategoriesTree;
            var res = coll.Where(c => c.Guid == guid).FirstOrDefault();

            if (res != null)
            {
                return res;
            }

            foreach(Category cat in coll)
            {
                Category findedCat = GetCatFromTree(guid, cat.Childs);
                if (findedCat != null)
                    return findedCat;
            }

            return null;
        }

        #endregion Cats

        #region Nomenclature

        /// <summary>
        /// Все номенклатуры.
        /// </summary>
        /// <returns></returns>
        internal ObservableCollection<Nomenclature> GetNomenclatures() => Nomenclatures;

        /// <summary>
        /// Открытие карточки номенклатуры.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void OpenNomenclurueCard(Nomenclature nomenclature)
        {
            MvvmFactory.CreateWindow(new NomenclurueCard(nomenclature, this), new ViewModels.NomenclatureCardViewModel(), new Views.NomenclatureCard(), ViewMode.ShowDialog);
        }

        /// <summary>
        /// Добавление номенклатуры.
        /// </summary>
        internal void AddNomenclature()
        {
            Nomenclature nomenclature = new Nomenclature();
            Nomenclatures.Add(nomenclature);
            OpenNomenclurueCard(nomenclature);
        }

        /// <summary>
        /// Удаление номенклатуры из каталога.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void DeleteNomenclurue(Nomenclature nomenclature) => CatalogFilter.Remove(nomenclature);

        /// <summary>
        /// Клонирование номенклатуры.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void CloneNomenclurue(Nomenclature nomenclature) => CatalogFilter.Clone(nomenclature);

        /// <summary>
        /// Удаление группы номенклатур.
        /// </summary>
        /// <param name="nomenclatureGroup"></param>
        internal void DelNomGroup(NomenclatureGroup nomenclatureGroup) => NomenclatureGroups.Remove(nomenclatureGroup);

        /// <summary>
        /// Добавление группы номенклатур.
        /// </summary>
        internal void AddNomenclatureGroup() => NomenclatureGroups.Add(new NomenclatureGroup() { Name = "Новая группа" });

        /// <summary>
        /// Удаление номенклатуры из группы.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void DelNomFromNomenclatureGroup(Nomenclature nomenclature) => SelectedNomenclatureGroup.Nomenclatures.Remove(nomenclature);

        /// <summary>
        /// Добавление номенклатуры в группу.
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        internal CallResult AddNomenclatureToGroup(Nomenclature nomenclature)
        {
            if (SelectedNomenclatureGroup != null)
            {
                SelectedNomenclatureGroup.Nomenclatures.Add(nomenclature);
                return new CallResult();
            }
            else
            {
                return new CallResult() { Error = new Error("Не выбрана группа, в которую нужно добавить номенклатуру") };
            }
        }

        #endregion Nomenclature
    }
}
