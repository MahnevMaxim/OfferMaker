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

        ObservableCollection<User> managers = new ObservableCollection<User>();
        ObservableCollection<Currency> currencies;

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

        public Constructor Constructor { get; set; } = Constructor.GetInstance();

        public DocManager DocManager { get; set; }

        #endregion Properties

        #region Initialization Main

        async internal override void Run()
        {
            Global.Main = this;
            DocManager = DocManager.GetInstance();
            MvvmFactory.RegisterModel(this, Catalog);
            MvvmFactory.RegisterModel(this, Constructor);
            Settings.SetSettings();
            
            //Фейковая авторизация.
            CallResult cr = await Hello.SetUsers(DataRepository);
            if (!cr.Success)
            {
                OnSendMessage("Ошибка при получении пользователей");
                await Task.Delay(7000);
                Close();
                return;
            }
            User = Hello.User;
            Managers = Hello.Managers;

            await InitData();
            Catalog.Run();
        }

        /// <summary>
        /// Инициализация данных
        /// </summary>
        /// <returns></returns>
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

            Constructor.Offer.OfferCreator = User;

            InitImages();
        }

        /// <summary>
        /// Инициализация изображений.
        /// </summary>
        private void InitImages()
        {
            Constructor.PhotoLogo = AppDomain.CurrentDomain.BaseDirectory + "common_images\\logo.png";
        }

        /// <summary>
        /// Установка ссылок на объекты номенклатур в группы номенклатур.
        /// </summary>
        private void SetNomGroups()
        {
            for (int i = 0; i < Catalog.NomenclatureGroups.Count; i++)
            {
                for (int j = 0; j < Catalog.NomenclatureGroups[i].Nomenclatures.Count; j++)
                {
                    var nom = Catalog.NomenclatureGroups[i].Nomenclatures[j];
                    if (nom.Id != 0)
                    {
                        Catalog.NomenclatureGroups[i].Nomenclatures.Remove(nom);
                        var existNom = Catalog.Nomenclatures.Where(n => n.Id == nom.Id).FirstOrDefault();
                        if (existNom != null) Catalog.NomenclatureGroups[i].Nomenclatures.Add(existNom);
                    }
                }
            }
        }

        #endregion Initialization Main

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

        public void SkipOffer() => Constructor.SkipOffer();

        #endregion Constructor

        #region DocManager

        public void SaveToPdfWithBanner() => DocManager.SaveToPdfWithBanner();

        #endregion DocManager

        #region Settings

        public void OpenSettings() => MvvmFactory.CreateWindow(Settings, new ViewModels.SettingsViewModel(), new Views.Settings(), ViewMode.ShowDialog);

        #endregion Settings

        #endregion Commands
    }
}
