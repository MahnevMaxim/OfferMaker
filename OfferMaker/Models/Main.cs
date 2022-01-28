using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.Windows;

namespace OfferMaker
{
    public class Main : BaseModel
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<User> managers = new ObservableCollection<User>();
        ObservableCollection<Currency> currencies;
        bool isDiscountOpen;
        int currentMainSelectedTabIndex;
        ObservableCollection<User> users;
        OfferStore archiveStore;
        OfferStore templatesStore;
        //public ObservableCollection<Offer> offerTemplates = new ObservableCollection<Offer>();

        #endregion Fields

        #region Propetries

        public User User { get; set; }

        public ObservableCollection<User> Managers
        {
            get => managers;
            set
            {
                managers = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Currency> Currencies
        {
            get
            {
                return currencies;
            }
            set
            {
                currencies = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsingCurrencies));
            }
        }

        public ObservableCollection<string> ConstructorCurrencies
        {
            get
            {
                if (Constructor.Currencies == null) return null;
                var list = Constructor.Currencies.Where(c => c.IsEnabled || c.CharCode == "RUB").Select(c => c.CharCode).ToList();
                return new ObservableCollection<string>(list);
            }
        }

        public ObservableCollection<string> UsingCurrencies
        {
            get
            {
                if (currencies == null) return null;
                var list = currencies.Where(c => c.IsEnabled || c.CharCode == "RUB").Select(c => c.CharCode).ToList();
                return new ObservableCollection<string>(list);
            }
        }

        public bool IsDiscountOpen
        {
            get => isDiscountOpen;
            set
            {
                isDiscountOpen = value;
                OnPropertyChanged();
            }
        }

        public Offer SelectedOfferInArchive { get; set; }

        public Offer SelectedOfferTemplate { get; set; }

        public int CurrentMainSelectedTabIndex
        {
            get => currentMainSelectedTabIndex;
            set
            {
                currentMainSelectedTabIndex = value;
                ShowOffers();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public OfferStore ArchiveStore
        {
            get => archiveStore;
            set
            {
                archiveStore = value;
                OnPropertyChanged();
            }
        }

        public OfferStore TemplatesStore
        {
            get => templatesStore;
            set
            {
                templatesStore = value;
                OnPropertyChanged();
            }
        }

        //public StringCollection Hints { get; set; }

        #endregion Propetries

        #endregion MVVVM 

        #region Properties

        #region Modules

        public DataRepository DataRepository { get; set; }

        public Catalog Catalog { get; set; }

        public Constructor Constructor { get; set; }

        public BannersManager BannersManager { get; set; }

        public Settings Settings { get; set; }

        public DocManager DocManager { get; set; }

        public ImageManager ImageManager { get; set; }

        public AdminPanel AdminPanel { get; set; }

        #endregion Modules

        #endregion Properties

        #region Fields

        public static List<Hint> hints;

        #endregion Fields

        #region Initialization Main

        async internal override void Run() => MvvmFactory.RegisterModel(this, Constructor);

        #endregion Initialization Main

        #region Commands

        #region Catalog

        public void OpenCatalog() => MvvmFactory.CreateWindow(Catalog, new ViewModels.CatalogViewModel(), new Views.Catalog(), ViewMode.ShowDialog);

        /// <summary>
        /// Сохраняем каталог.
        /// </summary>
        async public void SaveCatalog()
        {
            CallResult crSaveCurrencies = await DataRepository.SaveCurrencies(Currencies);
            CallResult crSaveNomenclatureGroups = await DataRepository.SaveNomenclatureGroups(Catalog.NomenclatureGroups);
            CallResult crSaveNomenclatures = await DataRepository.SaveNomenclatures(Catalog.Nomenclatures);
            ObservableCollection<Category> cats = GetPreparedCategories();
            CallResult crSaveCategories = await DataRepository.SaveCategories(cats);
        }

        /// <summary>
        /// Переустанавливаем родителей у элементов коллекции и распрямляем дерево.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Category> GetPreparedCategories()
        {
            SetParents(Catalog.CategoriesTree, null, null);
            return GetFlattenTree();
        }

        /// <summary>
        /// Обходим дерево и устанавливаем родителей, во время редактирования и перетаскиваний всё может перемешаться.
        /// </summary>
        /// <param name="categoriesTree"></param>
        /// <param name="parentId"></param>
        private void SetParents(ObservableCollection<Category> categoriesTree, int? parentId, string parentGuid)
        {
            int order = 0;
            categoriesTree.ToList().ForEach(c =>
            {
                c.ParentId = parentId;
                c.ParentGuid = parentGuid;
                c.Order = order++;
                SetParents(c.Childs, c.Id, c.Guid);
            });
        }

        /// <summary>
        /// Распрямляем дерево.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Category> GetFlattenTree()
        {
            List<Category> flattenTree = new List<Category>();
            foreach (var cat in Catalog.CategoriesTree)
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

        public void EditCategory(Category category, Point position) => Catalog.EditCategory(category, position);

        public void DelCategory(Category category) => Catalog.DelCategory(category);

        #endregion Catalog

        #region Constructor

        #region OfferGroup

        public void AddOfferGroup() => Constructor.AddOfferGroup();

        public void DelOfferGroup(OfferGroup offerGroup) => Constructor.DelOfferGroup(offerGroup);

        public void AddNomenclatureToOfferGroup(OfferGroup offerGroup) => Constructor.AddNomenclatureToOfferGroup(offerGroup);

        public void UpOfferGroup(OfferGroup offerGroup) => Constructor.UpOfferGroup(offerGroup);

        public void DownOfferGroup(OfferGroup offerGroup) => Constructor.DownOfferGroup(offerGroup);

        #endregion OfferGroup

        #region NomWrapper

        public void DeleteDescriptionFromNomWrapper(Description description, NomWrapper nomWrapper) => Constructor.DeleteDescriptionFromNomWrapper(description, nomWrapper);

        public void DescriptionMoveUp(Description description, NomWrapper nomWrapper) => Constructor.DescriptionMoveUp(description, nomWrapper);

        public void DescriptionMoveDown(Description description, NomWrapper nomWrapper) => Constructor.DescriptionMoveDown(description, nomWrapper);

        public void OpenDescriptions(NomWrapper nomWrapper) => Constructor.OpenDescriptions(nomWrapper);

        public void CloseRowDetails(NomWrapper nomWrapper) => Constructor.CloseRowDetails(nomWrapper);

        public void OpenCardNomWrapper(NomWrapper nomWrapper) => Constructor.OpenCardNomWrapper(nomWrapper);

        public void AddDescriptionToNomWrapper(NomWrapper nomWrapper) => Constructor.AddDescriptionToNomWrapper(nomWrapper);

        public void AddCommentToNomWrapper(NomWrapper nomWrapper) => Constructor.AddCommentToNomWrapper(nomWrapper);

        public void MoveUpNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => Constructor.MoveUpNomWrapper(nomWrapper, offerGroup);

        public void MoveDownNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => Constructor.MoveDownNomWrapper(nomWrapper, offerGroup);

        public void DeleteNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => Constructor.DeleteNomWrapper(nomWrapper, offerGroup);

        #endregion NomWrapper

        #region Discount

        public void SetDiscount()
        {
            CallResult cr = Constructor.SetDiscount();
            if (!cr.Success)
                OnSendMessage(cr.Error.Message);
            else
                IsDiscountOpen = false;
        }

        public void CancelDiscount()
        {
            Constructor.CancelDiscount();
            IsDiscountOpen = false;
        }

        #endregion Discount

        #region Etc

        public void SkipOffer() => Constructor.SkipOffer();

        public void EditCustomer() => Constructor.EditCustomer();

        public void OpenBanners() => Constructor.OpenBanners();

        public void AddInformBlock()
        {
            var cr = Constructor.AddInformBlock();
            if (!cr.Success) OnSendMessage(cr.Error.Message);
        }

        public void RemoveInformBlock(OfferInfoBlock offerInfoBlock) => Constructor.RemoveInformBlock(offerInfoBlock);

        public void SendMess(string mess) => OnSendMessage(mess);

        #endregion Etc

        #endregion Constructor

        #region Archive

        public void LoadOfferFromArchive(Offer offer)
        {
            CurrentMainSelectedTabIndex = 0;
            Constructor.LoadOfferFromArchive(offer);
        }

        public void LoadSelectedOfferFromArchive()
        {
            if (SelectedOfferInArchive != null)
            {
                CurrentMainSelectedTabIndex = 0;
                Constructor.LoadOfferFromArchive(SelectedOfferInArchive);
            }
            else
                OnSendMessage("Выберите КП");
        }

        private void ShowOffers()
        {
            if (CurrentMainSelectedTabIndex == 1)
                ArchiveStore.ApplyOfferFilter();
            else if (CurrentMainSelectedTabIndex == 2)
                TemplatesStore.ApplyOfferFilter();
        }

        public void ApplyArchiveFilter() => ArchiveStore.ApplyOfferFilter();

        public void FindOfferInArchive() => ArchiveStore.ApplyOfferFilter();

        public void ResetArchiveFilter()
        {
            ArchiveStore.ResetFilter();
            ArchiveStore.ApplyOfferFilter();
            OnPropertyChanged(nameof(ArchiveStore));
        }

        async public void DeleteOfferFromArchive(Offer offer)
        {
            var cr = await DataRepository.OfferDelete(offer);
            if (cr.Success)
            {
                if (!(offer.Id != 0 && Settings.AppMode == AppMode.Offline))
                    ArchiveStore.RemoveOffer(offer);
                ArchiveStore.ApplyOfferFilter();
            }
            else
                OnSendMessage(cr.Error.Message);
        }

        #endregion Archive

        #region Offer templates

        public void LoadOfferTemplate(Offer offer)
        {
            CurrentMainSelectedTabIndex = 0;
            Constructor.LoadOfferTemplate(offer);
        }

        public void FindOfferTemplate() => TemplatesStore.ApplyOfferFilter();

        public void LoadSelectedOfferTemplate()
        {
            if (SelectedOfferTemplate != null)
            {
                CurrentMainSelectedTabIndex = 0;
                Constructor.LoadOfferTemplate(SelectedOfferTemplate);
            }
            else
                OnSendMessage("Выберите шаблон");
        }

        #endregion Offer templates

        #region DocManager

        public void SaveToPdfWithBanner() => DocManager.SaveToPdfWithBanner();

        public void SaveToPdfWithoutBanner() => DocManager.SaveToPdfWithoutBanner();

        public void OfferTemplateCreate() => DocManager.OfferTemplateCreate();

        public void SaveOfferToFile() => DocManager.SaveOfferToFile();

        public void OpenOfferFromFile() => DocManager.OpenOfferFromFile();

        async public void OfferCreate() => DocManager.OfferCreate();

        #endregion DocManager

        #region Settings

        public void OpenSettings() => MvvmFactory.CreateWindow(Settings, new ViewModels.SettingsViewModel(), new Views.Settings(false), ViewMode.ShowDialog);

        public void Quit()
        {
            Settings.SkipUserSettings();
            Close();
        }

        #endregion Settings

        #region AdminPanel

        public void OpenAdminPanel() => MvvmFactory.CreateWindow(AdminPanel, new ViewModels.AdminPanelViewModel(), new Views.AdminPanel(), ViewMode.ShowDialog);

        #endregion AdminPanel

        #endregion Commands
    }
}
