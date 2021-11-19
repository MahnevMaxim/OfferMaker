using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

namespace OfferMaker
{
    public class Main : BaseModel
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<User> users;
        ObservableCollection<Currency> currencies;

        #endregion Fields

        #region Propetries

        public ObservableCollection<User> Users
        {
            get => users; 
            set
            {
                users = value;
                OnPropertyChanged();
            }
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

        public ObservableCollection<Currency> UsingCurrencies
        {
            get
            {
                if (currencies == null) return null;
                return new ObservableCollection<Currency>(currencies.Where(c => c.IsEnabled || c.CharCode == "RUB"));
            }
        }

        #endregion Propetries

        #endregion MVVVM 

        #region Properties

        DataRepository DataRepository { get; set; } = DataRepository.GetInstance();

        public Catalog Catalog { get; set; } = Catalog.GetInstance();

        public Settings Settings { get; set; } = Settings.GetInstance();

        public Constructor Constructor { get; set; } = new Constructor();

        #endregion Properties

        async internal override void Run()
        {
            Global.Main = this;
            MvvmFactory.RegisterModel(this, Catalog);
            MvvmFactory.RegisterModel(this, Constructor);
            Settings.SetSettings();

            //получаем валюты
            var currenciesCr = await DataRepository.GetCurrencies();
            if (currenciesCr.Success)
                Currencies = currenciesCr.Data;
            else
                OnSendMessage(currenciesCr.Error.Message);

            //получаем категории
            var categoriesCr = await DataRepository.GetCategories();
            if (categoriesCr.Success)
                Catalog.Categories = categoriesCr.Data;
            else
                OnSendMessage(categoriesCr.Error.Message);

            //получаем номенклатуры
            var nomenclaturesCr = await DataRepository.GetNomenclatures();
            if (nomenclaturesCr.Success)
                Catalog.Nomenclatures = nomenclaturesCr.Data;
            else
                OnSendMessage(nomenclaturesCr.Error.Message);

            //получаем группы номенклатур
            var nomGroupsCr = await DataRepository.GetNomGroups();
            if (nomGroupsCr.Success)
                Catalog.NomenclatureGroups = nomGroupsCr.Data;
            else
                OnSendMessage(nomGroupsCr.Error.Message);

            Catalog.Run();
        }

        #region Commands

        #region Catalog

        public void EditCategories() => Catalog.EditCategories();

        public void OpenNomenclurueCard(Nomenclature nomenclature) => Catalog.OpenNomenclurueCard(nomenclature);

        public void DeleteNomenclurue(Nomenclature nomenclature) => Catalog.DeleteNomenclurue(nomenclature);

        public void DelNomGroup(NomenclatureGroup nomenclatureGroup) => Catalog.DelNomGroup(nomenclatureGroup);

        public void AddNomenclatureGroup() => Catalog.AddNomenclatureGroup();

        public void AddNomenclatureToGroup(Nomenclature nomenclature)
        {
            CallResult cr = Catalog.AddNomenclatureToGroup(nomenclature);
            if (!cr.Success) OnSendMessage(cr.Error.Message);
        }

        public void EditCurrencies()
        {
            new CurrenciesView(Currencies).ShowDialog();
            OnPropertyChanged(nameof(UsingCurrencies));
        }

        async public void SaveCatalog()
        {
            CallResult crSaveCurrencies = await DataRepository.SaveCurrencies(Currencies);
            CallResult crSaveNomenclatureGroups = await DataRepository.SaveNomenclatureGroups(Catalog.NomenclatureGroups);
        }

        #endregion Catalog

        #region Constructor

        public void AddOfferGroup() => Constructor.AddOfferGroup();

        public void DelOfferGroup(OfferGroup offerGroup) => Constructor.DelOfferGroup(offerGroup);

        public void AddNomenclatureToOfferGroup(OfferGroup offerGroup) => Constructor.AddNomenclatureToOfferGroup(offerGroup);

        #endregion Constructor

        public void OpenSettings() => MvvmFactory.CreateWindow(Settings, new ViewModels.SettingsViewModel(), new Views.Settings(), ViewMode.ShowDialog);

        #endregion Commands
    }
}
