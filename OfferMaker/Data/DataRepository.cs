using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using System.Collections.Specialized;

namespace OfferMaker
{
    /// <summary>
    /// Класс для хранения, управления и синхронизации данных,
    /// абстрагирование разработчика и логики приложения от источников данных
    /// </summary>
    public class DataRepository
    {
        /// <summary>
        /// private не трогать, обращаться только через DataRepository,
        /// иначе смысла в DataRepository классе нет, 
        /// а смысл DataRepository - упрощение хранения, управления и синхронизации данных,
        /// абстрагирование разработчика и логики приложения от источников данных
        /// </summary>
        private Proxy Proxy = new Proxy();

        #region Singleton

        private DataRepository() { }

        private static readonly DataRepository instance = new DataRepository();

        public static DataRepository GetInstance() => instance;

        internal static DataRepository GetInstance(AppMode appMode)
        {
            instance.Proxy.defaultAppMode = appMode;
            return instance;
        }

        #endregion Singleton

        async public Task<CallResult<ObservableCollection<Currency>>> GetCurrencies() => await Proxy.GetCurrencies();

        async internal Task<CallResult<ObservableCollection<NomenclatureGroup>>> GetNomGroups() => await Proxy.GetNomGroups();

        

        async internal Task<CallResult<ObservableCollection<Nomenclature>>> GetNomenclatures() => await Proxy.GetNomenclatures();

        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories() => await Proxy.GetCategories();

        async internal Task<CallResult<ObservableCollection<User>>> GetUsers() => await Proxy.GetUsers();

        async internal Task<CallResult<ObservableCollection<Offer>>> GetOffers() => await Proxy.GetOffers();

        async internal Task<CallResult<StringCollection>> GetHints() => await Proxy.GetHints();

        async internal Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies) => await Proxy.SaveCurrencies(currencies);

        async internal Task<CallResult> SaveCategories(ObservableCollection<Category> categoriesTree) => await Proxy.SaveCategories(categoriesTree);

        async internal Task<CallResult> SaveNomenclatureGroups(ObservableCollection<NomenclatureGroup> nomenclatureGroups) => await Proxy.SaveNomenclatureGroups(nomenclatureGroups);

        async internal Task<CallResult> SaveOffer(Offer offer, ObservableCollection<Offer> offers) => await Proxy.SaveOffer(offer, offers);

        async internal Task<CallResult> SaveNomenclatures(ObservableCollection<Nomenclature> nomenclatures) => await Proxy.SaveNomenclatures(nomenclatures);

        async internal Task<CallResult> DeleteOfferFromArchive(Offer offer, ObservableCollection<Offer> offers) => await Proxy.DeleteOfferFromArchive(offer, offers);

        public void SyncData()
        {

        }

        public void UpdateData()
        {

        }

        
    }
}
