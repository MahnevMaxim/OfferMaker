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

        public override void InitializeViewModel()
        {
            modelMain = (Main)model;
        }

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

        public ObservableCollection<Currency> UsingCurrencies { get => modelMain.UsingCurrencies; }

        public User User { get => modelMain.User; }

        public ObservableCollection<User> Managers { get => modelMain.Managers; }

        #endregion Main

        #region Constructor

        public ObservableCollection<DebugTreeItem> DebugTree
        {
            get => modelMain.Constructor.DebugTree; 
            set
            {
                modelMain.Constructor.DebugTree = value;
                OnPropertyChanged();
            }
        }

        public Currency Currency
        {
            get => modelMain.Constructor.Offer.Currency; 
            set
            {
                modelMain.Constructor.Offer.Currency = value;
                OnPropertyChanged();
            }
        }

        public User Manager
        {
            get => modelMain.Constructor.Offer.Manager; 
            set
            {
                modelMain.Constructor.Offer.Manager = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<OfferGroup> OfferGroups
        {
            get => modelMain.Constructor.Offer.OfferGroups; 
            set
            {
                modelMain.Constructor.Offer.OfferGroups = value;
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

        #endregion Constructor
    }
}
