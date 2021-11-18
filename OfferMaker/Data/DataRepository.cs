using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

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

        #endregion Singleton

        async public Task<ObservableCollection<Currency>> GetCurrencies() => await Proxy.GetCurrencies();

        async public Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies) => await Proxy.SaveCurrencies(currencies);

        async public Task<CallResult> SaveNomenclatureGroups(ObservableCollection<NomenclatureGroup> nomenclatureGroups) => await Proxy.SaveNomenclatureGroups(nomenclatureGroups);

        internal async Task<ObservableCollection<Nomenclature>> GetNomenclatures() => await Proxy.GetNomenclatures();

        internal async Task<ObservableCollection<Category>> GetCategories() => await Proxy.GetCategories();

        internal async Task<ObservableCollection<User>> GetUsers() => await Proxy.GetUsers();

        public void SyncData()
        {

        }

        public void UpdateData()
        {

        }
    }
}
