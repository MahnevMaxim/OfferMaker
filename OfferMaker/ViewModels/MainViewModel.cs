using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        Main modelMain;
        bool isInfoBlocksOpen = false;
        bool isOffersFilterOpen = false;
        bool isOfferTemplatesFilterOpen = false;
        string appMode;

        private IDialogCoordinator dialogCoordinator;

        public override void InitializeViewModel()
        {
            modelMain = (Main)model;
            dialogCoordinator = ((Views.MainWindow)view).dialogCoordinator;
        }

        #region Fly menu

        public bool IsInfoBlocksOpen
        {
            get { return isInfoBlocksOpen; }
            set
            {
                isInfoBlocksOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsDiscountOpen
        {
            get { return modelMain.IsDiscountOpen; }
            set
            {
                modelMain.IsDiscountOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsOffersFilterOpen
        {
            get { return isOffersFilterOpen; }
            set
            {
                isOffersFilterOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsOfferTemplatesFilterOpen
        {
            get { return isOfferTemplatesFilterOpen; }
            set
            {
                isOfferTemplatesFilterOpen = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand FlyInfoBlocks { get => new RelayCommand(obj => IsInfoBlocksOpen = true); }

        public RelayCommand FlyDiscount { get => new RelayCommand(obj => IsDiscountOpen = true); }

        public RelayCommand FlyOffersFilter { get => new RelayCommand(obj => IsOffersFilterOpen = true); }

        public RelayCommand FlyOfferTemplatesFilter { get => new RelayCommand(obj => IsOfferTemplatesFilterOpen = true); }

        #endregion Fly menu

        #region Settings

        public Settings Settings { get => modelMain.Settings; }

        #endregion Settings

        #region Main

        public OfferStore ArchiveStore { get => modelMain.ArchiveStore; }

        public OfferStore TemplatesStore { get => modelMain.TemplatesStore; }

        public ObservableCollection<Offer> ArchiveOffers { get => modelMain.ArchiveStore.FilteredOffers; }

        public ObservableCollection<Offer> OfferTemplates { get => modelMain.TemplatesStore.FilteredOffers; }

        public ObservableCollection<User> Users { get => modelMain.Users; }

        public ObservableCollection<Currency> Currencies
        {
            get => modelMain.Currencies;
            set
            {
                modelMain.Currencies = value;
                OnPropertyChanged();
            }
        }

        public Offer SelectedOfferInArchive
        {
            get => modelMain.SelectedOfferInArchive;
            set => modelMain.SelectedOfferInArchive = value;
        }

        public Offer SelectedOfferTemplate
        {
            get => modelMain.SelectedOfferTemplate;
            set => modelMain.SelectedOfferTemplate = value;
        }

        public int CurrentMainSelectedTabIndex
        {
            get => modelMain.CurrentMainSelectedTabIndex;
            set => modelMain.CurrentMainSelectedTabIndex = value;
        }

        public ObservableCollection<string> UsingCurrencies { get => modelMain.ConstructorCurrencies; }

        public User User { get => modelMain.User; }

        public ObservableCollection<User> Managers { get => modelMain.Managers; }

        public bool IsBusy { get => modelMain.IsBusy; }

        public string ProcessStatus { get => modelMain.ProcessStatus; }

        #endregion Main

        #region Constructor

        public RelayCommand DeleteOfferGroupCommand
        {
            get => new RelayCommand(obj =>
            {
                DeleteOfferGroup((OfferGroup)obj);
            });
        }

        async void DeleteOfferGroup(OfferGroup group)
        {
            if(group.NomWrappers.Count>0)
            {
                var dialogSettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Удалить",
                    NegativeButtonText = "Отмена"
                };
                var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "", "Удалить группу?",
                    MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                if (dialogRes != MessageDialogResult.Affirmative)
                    return;
            }
            modelMain.DelOfferGroup(group);
        }

        #region Offer

        public Discount Discount { get => modelMain.Constructor.Offer.Discount; }

        public decimal TotalCostPriceSum { get => modelMain.Constructor.Offer.TotalCostPriceSum; }

        public decimal TotalSum { get => modelMain.Constructor.Offer.TotalSum; }

        public decimal AverageMarkup { get => modelMain.Constructor.Offer.AverageMarkup; }

        public decimal ProfitSum { get => modelMain.Constructor.Offer.ProfitSum; }

        public decimal TotalSumWithoutOptions { get => modelMain.Constructor.Offer.TotalSumWithoutOptions; }

        public decimal TotalSumOptions { get => modelMain.Constructor.Offer.TotalSumOptions; }

        public string AltId { get => modelMain.Constructor.Offer.AltId; }

        public string CreateDateString
        {
            get => modelMain.Constructor.Offer.CreateDateString;
            set
            {
                modelMain.Constructor.Offer.CreateDateString = value;
                OnPropertyChanged();
            }
        }

        public string CurrencyCode
        {
            get => modelMain.Constructor.Offer.Currency?.ToString();
            set
            {
                Currency curr = modelMain.Currencies.Where(c => c.CharCode == value).First();
                modelMain.Constructor.Offer.SetCurrencySilent(curr);
                OnPropertyChanged();
            }
        }

        public Currency Currency { get => modelMain.Constructor.Offer.Currency; }

        public User Manager
        {
            get => modelMain.Constructor.Offer.Manager;
            set
            {
                modelMain.Constructor.Offer.Manager = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OfferGroup> OfferGroups { get => modelMain.Constructor.Offer.OfferGroups; }

        public ObservableCollection<OfferGroup> OfferGroupsNotOptions { get => modelMain.Constructor.Offer.OfferGroupsNotOptions; }

        public ObservableCollection<OfferGroup> OfferGroupsOptions { get => modelMain.Constructor.Offer.OfferGroupsOptions; }

        public bool IsRequiredGroups { get => modelMain.Constructor.Offer.IsRequiredGroups; }

        public bool IsRequiredOptions { get => modelMain.Constructor.Offer.IsRequiredOptions; }

        public ObservableCollection<OfferInfoBlock> OfferInfoBlocks
        {
            get => modelMain.Constructor.Offer.OfferInfoBlocks;
            set
            {
                modelMain.Constructor.Offer.OfferInfoBlocks = value;
                OnPropertyChanged();
            }
        }

        public bool IsHiddenTextNds
        {
            get => modelMain.Constructor.Offer.IsHiddenTextNds;
            set
            {
                if (IsWithNds == 1 && value == false)
                {
                    OnPropertyChanged();
                }
                else
                {
                    modelMain.Constructor.Offer.IsHiddenTextNds = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsResultSummInRub
        {
            get => modelMain.Constructor.Offer.IsResultSummInRub;
            set
            {
                modelMain.Constructor.Offer.IsResultSummInRub = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowPriceDetails
        {
            get => modelMain.Constructor.Offer.IsShowPriceDetails;
            set
            {
                modelMain.Constructor.Offer.IsShowPriceDetails = value;
                OnPropertyChanged();
            }
        }

        public bool IsCreateByCostPrice
        {
            get => modelMain.Constructor.Offer.IsCreateByCostPrice;
            set
            {
                modelMain.Constructor.Offer.IsCreateByCostPrice = value;
                OnPropertyChanged();
            }
        }

        public bool IsHideNomsPrice
        {
            get => modelMain.Constructor.Offer.IsHideNomsPrice;
            set
            {
                modelMain.Constructor.Offer.IsHideNomsPrice = value;
                OnPropertyChanged();
            }
        }

        public int IsWithNds
        {
            get => modelMain.Constructor.Offer.IsWithNds ? 0 : 1;
            set
            {
                modelMain.Constructor.Offer.IsWithNds = value == 0 ? true : false;
                if (value == 1) IsHiddenTextNds = true;
                OnPropertyChanged();
            }
        }

        public DateTime CreateDate
        {
            get => modelMain.Constructor.Offer.CreateDate;
            set
            {
                modelMain.Constructor.Offer.CreateDate = value;
                OnPropertyChanged();
            }
        }

        public string BannerImagePath
        {
            get => modelMain.Constructor.Offer.BannerImagePath;
            set
            {
                modelMain.Constructor.Offer.BannerImagePath = value;
                OnPropertyChanged();
            }
        }

        public string OfferName
        {
            get => modelMain.Constructor.Offer.OfferName;
            set
            {
                modelMain.Constructor.Offer.OfferName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AdvertisingsUp { get => modelMain.Constructor.Offer.AdvertisingsUp; }

        public ObservableCollection<string> AdvertisingsDown { get => modelMain.Constructor.Offer.AdvertisingsDown; }

        public Customer Customer
        {
            get => modelMain.Constructor.Offer.Customer;
            set
            {
                modelMain.Constructor.Offer.Customer = value;
                OnPropertyChanged();
            }
        }

        public string OfferStatus { get => modelMain.Constructor.OfferStatus; }

        #endregion Offer

        public int PdfControlSelectedIndex
        {
            get => modelMain.Constructor.PdfControlSelectedIndex;
            set => modelMain.Constructor.PdfControlSelectedIndex = value;
        }

        public ObservableCollection<DebugTreeItem> DebugTree
        {
            get => modelMain.Constructor.DebugTree;
            set
            {
                modelMain.Constructor.DebugTree = value;
                OnPropertyChanged();
            }
        }

        public FixedDocument PdfDocument
        {
            get => modelMain.Constructor.PdfDocument;
            set
            {
                modelMain.Constructor.PdfDocument = value;
                OnPropertyChanged();
            }
        }

        public FixedDocument PdfDocumentShort
        {
            get => modelMain.Constructor.PdfDocumentShort;
            set
            {
                modelMain.Constructor.PdfDocumentShort = value;
                OnPropertyChanged();
            }
        }

        public string PhotoLogo
        {
            get => modelMain.Constructor.PhotoLogo;
            set
            {
                modelMain.Constructor.PhotoLogo = value;
                OnPropertyChanged();
            }
        }

        public string PhotoNumber
        {
            get => modelMain.Constructor.PhotoNumber;
            set
            {
                modelMain.Constructor.PhotoNumber = value;
                OnPropertyChanged();
            }
        }

        public string PhotoCustomer
        {
            get => modelMain.Constructor.PhotoCustomer;
            set
            {
                modelMain.Constructor.PhotoCustomer = value;
                OnPropertyChanged();
            }
        }

        public string PhotoAdress
        {
            get => modelMain.Constructor.PhotoAdress;
            set
            {
                modelMain.Constructor.PhotoAdress = value;
                OnPropertyChanged();
            }
        }

        public string PhotoNumberTeh { get => modelMain.Constructor.PhotoNumberTeh; }

        public string AppMode
        {
            get
            {
                if (appMode == null)
                    return appMode = Settings.GetInstance().AppMode.ToString();
                return appMode;
            }
        }

        public string Version { get => Settings.GetInstance().Version; }

        #endregion Constructor
    }
}
