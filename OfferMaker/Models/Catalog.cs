using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using System.Windows;
using System.Collections;

namespace OfferMaker
{
    public class Catalog : BaseModel, ICatalog
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<Nomenclature> filteredNomenclatures;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups = new ObservableCollection<NomenclatureGroup>();
        NomenclatureGroup selectedNomenclatureGroup;
        ObservableCollection<Category> categoriesTree;
        string searchStringInCatalog;
        CatalogFilter catalogFilter;
        ObservableCollection<Category> categories;
        IList selectedNomenclatures = new ArrayList();

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

        public IList SelectedNomenclatures { set => selectedNomenclatures = value; }

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

        public string SearchStringInCatalog { set => searchStringInCatalog = value; }

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
            if(CatalogFilter==null)
            {
                CreateCategoriesTree();
                CatalogFilter = new CatalogFilter(this);
            }
        }

        /// <summary>
        /// Запуск редактора валют.
        /// </summary>
        public void EditCurrencies()
        {
            new CurrenciesView(new ObservableCollection<Currency>(Global.Currencies)).ShowDialog();
            Global.Main.OnPropertyChanged(nameof(Global.Main.UsingCurrencies));
        }

        /// <summary>
        /// Сохраняем каталог.
        /// </summary>
        public void SaveCatalog() => Global.Main.SaveCatalog();

        #region Categories

        /// <summary>
        /// Переустанавливаем родителей у элементов коллекции и распрямляем дерево.
        /// </summary>
        /// <returns></returns>
        //private ObservableCollection<Category> GetPreparedCategories()
        //{
        //    SetParents(CategoriesTree, null, null);
        //    return GetFlattenTree();
        //}

        /// <summary>
        /// Обходим дерево и устанавливаем родителей, во время редактирования и перетаскиваний всё может перемешаться.
        /// </summary>
        /// <param name="categoriesTree"></param>
        /// <param name="parentId"></param>
        private void SetParents(ObservableCollection<Category> categoriesTree, int? parentId, string parentGuid)
        {
            categoriesTree.ToList().ForEach(c =>
            {
                c.ParentId = parentId;
                c.ParentGuid = parentGuid;
                SetParents(c.Childs, c.Id, c.Guid);
            });
        }

        /// <summary>
        /// Распрямляем дерево.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Category> GetFlattenTree()
        {
            List<Category> flattenTree = new List<Category>();
            foreach (var cat in CategoriesTree)
            {
                var subCats = TreeWalker.Walk<Category>(cat, c => c.Childs).ToList();
                if (subCats.Count > 0)
                {
                    flattenTree.AddRange(subCats);
                }
            }
            ObservableCollection<Category> resColl = new ObservableCollection<Category>();
            flattenTree.ForEach(f => resColl.Add(f));
            return resColl;
        }

        public void ShowAllCategory() => CatalogFilter.SetMode(FilterMode.All);

        public void ShowWithoutCategory() => CatalogFilter.SetMode(FilterMode.WithoutCat);

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
            var roots = categories.Where(c => c.ParentGuid == null).OrderBy(c => c.Order).ToList();
            SetChilds(CategoriesTree, roots);
        }

        private void SetNomenclatureCache(Category ch) => Nomenclatures.Where(n => n.CategoryGuid == ch.Guid).ToList().ForEach(n => ch.Nomenclatures.Add(n));

        private void SetChilds(ObservableCollection<Category> targetCollection, List<Category> childs)
        {
            foreach (var ch in childs)
            {
                SetNomenclatureCache(ch);
                targetCollection.Add(ch);
                var childs_ = categories.Where(c => c.ParentGuid != null && c.ParentGuid == ch.Guid).OrderBy(c => c.Order).ToList();
                SetChilds(ch.Childs, childs_);
            }
        }

        /// <summary>
        /// Добавление категории.
        /// </summary>
        public void AddCategory()
        {
            Category cat = new Category(newCatName);
            CategoriesTree.Add(cat);
            Categories.Add(cat);
        }
            
        /// <summary>
        /// Редактирование заголовка категории.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="position"></param>
        public void EditCategory(Category category, Point position)
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
        public void DelCategory(Category category)
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
        /// Добавление номенклатуры в группу.
        /// </summary>
        /// <param name="nomenclature"></param>
        /// <returns></returns>
        public void AddNomenclatureToGroup(Nomenclature nomenclature)
        {
            if (SelectedNomenclatureGroup != null)
            {
                SelectedNomenclatureGroup.Nomenclatures.Add(nomenclature);
            }
            else
            {
                OnSendMessage("Не выбрана группа, в которую нужно добавить номенклатуру");
            }
        }

        /// <summary>
        /// Все номенклатуры.
        /// </summary>
        /// <returns></returns>
        internal ObservableCollection<Nomenclature> GetNomenclatures() => Nomenclatures;

        /// <summary>
        /// Открытие карточки номенклатуры.
        /// </summary>
        /// <param name="nomenclature"></param>
        public void OpenNomenclurueCard(Nomenclature nomenclature)
        {
            MvvmFactory.CreateWindow(new NomenclurueCard(nomenclature, this), new ViewModels.NomenclatureCardViewModel(), new Views.NomenclatureCard(), ViewMode.ShowDialog);
        }

        /// <summary>
        /// Добавление номенклатуры.
        /// </summary>
        public void AddNomenclature()
        {
            Nomenclature nomenclature = new Nomenclature();
            Nomenclatures.Add(nomenclature);
            OpenNomenclurueCard(nomenclature);
        }

        /// <summary>
        /// Удаление номенклатуры из каталога.
        /// </summary>
        /// <param name="nomenclature"></param>
        public void DeleteNomenclature(Nomenclature nomenclature) => CatalogFilter.Remove(nomenclature);

        /// <summary>
        /// Удаление номенклатур из каталога.
        /// </summary>
        /// <param name="nomenclature"></param>
        public void DeleteNomenclatures()
        {
            List<Nomenclature> noms = new List<Nomenclature>();
            foreach (var nom in selectedNomenclatures)
                noms.Add((Nomenclature)nom);
            noms.ForEach(n => CatalogFilter.Remove(n));
        }
            
        /// <summary>
        /// Клонирование номенклатуры.
        /// </summary>
        /// <param name="nomenclature"></param>
        public void CloneNomenclurue(Nomenclature nomenclature) => CatalogFilter.Clone(nomenclature);

        /// <summary>
        /// Удаление группы номенклатур.
        /// </summary>
        /// <param name="nomenclatureGroup"></param>
        public void DelNomGroup(NomenclatureGroup nomenclatureGroup) => NomenclatureGroups.Remove(nomenclatureGroup);

        /// <summary>
        /// Добавление группы номенклатур.
        /// </summary>
        public void AddNomenclatureGroup() => NomenclatureGroups.Add(new NomenclatureGroup() { Name = "Новая группа" });

        /// <summary>
        /// Удаление номенклатуры из группы.
        /// </summary>
        /// <param name="nomenclature"></param>
        public void DelNomFromNomenclatureGroup(Nomenclature nomenclature) => SelectedNomenclatureGroup.Nomenclatures.Remove(nomenclature);

        #endregion Nomenclature
    }
}
