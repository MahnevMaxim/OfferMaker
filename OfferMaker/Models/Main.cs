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
            await InitData();
            Catalog.Run();
        }

        async private Task InitData()
        {
            string errorMessage = "";

            //получаем валюты
            var currenciesCr = await DataRepository.GetCurrencies();
            if (currenciesCr.Success)
                Currencies = currenciesCr.Data;
            else
                errorMessage += currenciesCr.Error.Message + "\n";

            //получаем категории
            var categoriesCr = await DataRepository.GetCategories();
            if (categoriesCr.Success)
                Catalog.Categories = categoriesCr.Data;
            else
                errorMessage += categoriesCr.Error.Message + "\n";

            //получаем номенклатуры
            var nomenclaturesCr = await DataRepository.GetNomenclatures();
            if (nomenclaturesCr.Success)
                Catalog.Nomenclatures = nomenclaturesCr.Data;
            else
                errorMessage += nomenclaturesCr.Error.Message + "\n";

            //получаем группы номенклатур
            var nomGroupsCr = await DataRepository.GetNomGroups();
            if (nomGroupsCr.Success)
            {
                Catalog.NomenclatureGroups = nomGroupsCr.Data;
                SetNomGroups();
            }

            else
                errorMessage += nomGroupsCr.Error.Message + "\n";

            if (!string.IsNullOrWhiteSpace(errorMessage))
                OnSendMessage(errorMessage);
        }

        private void SetNomGroups()
        {
            for (int i = 0; i < Catalog.NomenclatureGroups.Count; i++)
            {
                for(int j=0;j < Catalog.NomenclatureGroups[i].Nomenclatures.Count;j++)
                {
                    var nom = Catalog.NomenclatureGroups[i].Nomenclatures[j];
                    if(nom.Id!=0)
                    {
                        Catalog.NomenclatureGroups[i].Nomenclatures.Remove(nom);
                        var existNom = Catalog.Nomenclatures.Where(n => n.Id == nom.Id).FirstOrDefault();
                        if (existNom != null) Catalog.NomenclatureGroups[i].Nomenclatures.Add(existNom);
                    }
                }
            }
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

        public void DeleteNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => Constructor.DeleteNomWrapper(nomWrapper, offerGroup);
        public void DeleteDescriptionFromNomWrapper(Description description, NomWrapper nomWrapper) => Constructor.DeleteDescriptionFromNomWrapper(description, nomWrapper);

        #endregion Constructor

        public void OpenSettings() => MvvmFactory.CreateWindow(Settings, new ViewModels.SettingsViewModel(), new Views.Settings(), ViewMode.ShowDialog);

        #endregion Commands
    }
}
