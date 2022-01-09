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
        ArchiveFilter archiveFilter;
        ObservableCollection<Offer> archiveOffers = new ObservableCollection<Offer>();

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

        internal List<string> GetHints()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Currency> Currencies
        {
            get => currencies;
            set
            {
                currencies = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsingCurrencies));
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

        /// <summary>
        /// Все КП получаем строго через ArchiveFilter.
        /// </summary>
        public ObservableCollection<Offer> ArchiveOffers
        {
            get => archiveOffers;
            set
            {
                archiveOffers = value;
                OnPropertyChanged();
            }
        }

        public Offer SelectedOfferInArchive { get; set; }

        public int CurrentMainSelectedTabIndex
        {
            get => currentMainSelectedTabIndex;
            set
            {
                currentMainSelectedTabIndex = value;
                ShowArchive();
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

        public ArchiveFilter ArchiveFilter
        {
            get => archiveFilter;
            set
            {
                archiveFilter = value;
                OnPropertyChanged();
            }
        }

        public StringCollection Hints { get; set; }

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

        public ObservableCollection<Offer> offers;

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
            int order=0;
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
            foreach(var cat in Catalog.CategoriesTree)
            {
                var subCats = TreeWalker.Walk<Category>(cat, c => c.Childs).ToList();
                if(subCats.Count>0)
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
                CurrentMainSelectedTabIndex=0;
                Constructor.LoadOfferFromArchive(SelectedOfferInArchive);
            }
            else
                OnSendMessage("Выберите КП");
        }

        private void ShowArchive()
        {
            if(CurrentMainSelectedTabIndex==1)
            {
                ArchiveFilter.SetArchiveMode(ArchiveMode.ShowOffers);
            }
            else if(CurrentMainSelectedTabIndex==2)
            {
                ArchiveFilter.SetArchiveMode(ArchiveMode.ShowTemplate);
            }
            ApplyArchiveFilter();
        }

        public void ApplyArchiveFilter() => ArchiveOffers = ArchiveFilter.GetFilteredOffers();

        public void FindOfferInArchive() => ApplyArchiveFilter();

        public void ResetArchiveFilter()
        {
            ArchiveFilter = new ArchiveFilter(offers, User);
            ArchiveOffers = ArchiveFilter.GetFilteredOffers();
        }
          
        async public void DeleteOfferFromArchive(Offer offer)
        {
            offers.Remove(offer);
            ArchiveOffers = ArchiveFilter.GetFilteredOffers();
            var cr = await DataRepository.DeleteOfferFromArchive(offer, offers);
            if (!cr.Success)
                OnSendMessage(cr.Error.Message);
        }

        #endregion Archive

        #region DocManager

        public void SaveToPdfWithBanner() => DocManager.SaveToPdfWithBanner();

        public void SaveToPdfWithoutBanner() => DocManager.SaveToPdfWithoutBanner();

        public void SaveTemplateToArchive() => DocManager.SaveTemplateToArchive();

        public void SaveOfferToFile() => DocManager.SaveOfferToFile();

        public void OpenOfferFromFile() => DocManager.OpenOfferFromFile();

        async public void SaveOffer() => DocManager.SaveOffer();
        
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
