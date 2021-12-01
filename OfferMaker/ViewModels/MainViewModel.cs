using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace OfferMaker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        Main modelMain;
        private bool isInfoBlocksOpen = false;

        public override void InitializeViewModel()
        {
            modelMain = (Main)model;
        }

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

        public RelayCommand FlyInfoBlocks { get => new RelayCommand(obj => IsInfoBlocksOpen = true); }

        public RelayCommand FlyDiscount { get => new RelayCommand(obj => IsDiscountOpen = true); }

        #region Catalog

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get => modelMain.Catalog.Nomenclatures;
            set
            {
                modelMain.Catalog.Nomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NomenclatureGroup> NomenclatureGroups
        {
            get => modelMain.Catalog.NomenclatureGroups;
            set
            {
                modelMain.Catalog.NomenclatureGroups = value;
                OnPropertyChanged();
            }
        }

        public NomenclatureGroup SelectedNomenclatureGroup
        {
            get => modelMain.Catalog.SelectedNomenclatureGroup;
            set
            {
                modelMain.Catalog.SelectedNomenclatureGroup = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> CategoriesTree
        {
            get => modelMain.Catalog.CategoriesTree;
            set
            {
                modelMain.Catalog.CategoriesTree = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCat
        {
            get => modelMain.Catalog.SelectedCat;
            set
            {
                modelMain.Catalog.SelectedCat = value;
                OnPropertyChanged();
            }
        }

        #endregion Catalog

        #region Settings

        public Settings Settings { get => modelMain.Settings; }

        #endregion Settings

        #region Main

        public ObservableCollection<Currency> Currencies
        {
            get => modelMain.Currencies;
            set
            {
                modelMain.Currencies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UsingCurrencies { get => modelMain.UsingCurrencies; }

        public User User { get => modelMain.User; }

        public ObservableCollection<User> Managers { get => modelMain.Managers; }

        #endregion Main

        #region Constructor

        #region Offer

        public Discount Discount { get => modelMain.Constructor.Offer.Discount;}

        public decimal TotalCostPriceSum { get => modelMain.Constructor.Offer.TotalCostPriceSum; }

        public decimal TotalSum { get => modelMain.Constructor.Offer.TotalSum; }

        public decimal AverageMarkup { get => modelMain.Constructor.Offer.AverageMarkup; }

        public decimal ProfitSum { get => modelMain.Constructor.Offer.ProfitSum; }

        public decimal TotalSumWithoutOptions { get => modelMain.Constructor.Offer.TotalSumWithoutOptions; }

        public decimal TotalSumOptions { get => modelMain.Constructor.Offer.TotalSumOptions; }

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
            get
            {
                if (modelMain.Constructor.Offer.Manager == null) return null;
                if (modelMain.Constructor.Offer.Manager.Id == User.Id) return null;
                return modelMain.Constructor.Offer.Manager;
            }
                
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
                modelMain.Constructor.Offer.IsHiddenTextNds = value;
                OnPropertyChanged();
            }
        }

        public bool ResultSummInRub
        {
            get => modelMain.Constructor.Offer.ResultSummInRub;
            set
            {
                modelMain.Constructor.Offer.ResultSummInRub = value;
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

        public string Banner
        {
            get => modelMain.Constructor.Offer.Banner;
            set
            {
                modelMain.Constructor.Offer.Banner = value;
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

        #endregion Constructor
    }
}
