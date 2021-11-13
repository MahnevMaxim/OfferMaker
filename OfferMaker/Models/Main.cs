using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
            }
        }

        #endregion Propetries

        #endregion MVVVM 

        #region Properties

        DataRepository DataRepository { get; set; } = DataRepository.GetInstance();

        public Catalog Catalog { get; set; } = Catalog.GetInstance();

        public Settings Settings { get; set; } = Settings.GetInstance();

        #endregion Properties

        async internal override void Run()
        {
            Global.Main = this;
            MvvmFactory.RegisterModel(this, Catalog);
            Settings.SetSettings();
            Currencies = await DataRepository.GetCurrencies();
            Catalog.Nomenclatures = await DataRepository.GetNomenclatures();
            Catalog.Categories = await DataRepository.GetCategories();
            Catalog.Run();
        }

        #region Commands

        public void EditCategories() => Catalog.EditCategories();

        public void OpenSettings() => MvvmFactory.CreateWindow(Settings, new ViewModels.SettingsViewModel(), new Views.Settings(), ViewMode.ShowDialog);

        public void EditCurrencies() => new CurrenciesView(Currencies).ShowDialog();

        async public void SaveCatalog() => await DataRepository.SaveCurrencies(Currencies);

        #endregion Commands
    }
}
