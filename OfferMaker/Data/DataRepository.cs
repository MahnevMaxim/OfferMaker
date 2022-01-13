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
        private Proxy Proxy;

        #region Singleton

        private DataRepository() { }

        private static readonly DataRepository instance = new DataRepository();

        public static DataRepository GetInstance(string accessToken)
        {
            instance.Proxy = new Proxy(accessToken);
            return instance;
        }
            
        internal static DataRepository GetInstance(AppMode appMode)
        {
            instance.Proxy = new Proxy();
            instance.Proxy.defaultAppMode = appMode;
            return instance;
        }

        #endregion Singleton

        #region Currencies

        async public Task<CallResult<ObservableCollection<Currency>>> GetCurrencies() => await Proxy.GetCurrencies();

        async internal Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies) => await Proxy.SaveCurrencies(currencies);

        #endregion Currencies

        #region Offers

        async internal Task<CallResult<ObservableCollection<Offer>>> OffersGet() => await Proxy.OffersGet();

        async internal Task<CallResult<ObservableCollection<Offer>>> OffersSelfGet() => await Proxy.OffersSelfGet();

        async internal Task<CallResult> OfferCreate(Offer offer, ObservableCollection<Offer> offers) => await Proxy.OfferCreate(offer, offers);

        async internal Task<CallResult> OfferDelete(Offer offer, ObservableCollection<Offer> offers) => await Proxy.OfferDelete(offer, offers);

        #endregion Offers

        #region Categories

        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories() => await Proxy.GetCategories();

        async internal Task<CallResult> SaveCategories(ObservableCollection<Category> categoriesTree) => await Proxy.SaveCategories(categoriesTree);

        #endregion Categories

        #region Nomenclatures

        async internal Task<CallResult<ObservableCollection<Nomenclature>>> NomenclaturesGet() => await Proxy.NomenclaturesGet();

        async internal Task<CallResult> SaveNomenclatures(ObservableCollection<Nomenclature> nomenclatures) => await Proxy.SaveNomenclatures(nomenclatures);

        #endregion Nomenclatures

        #region Nomenclature groups

        async internal Task<CallResult> SaveNomenclatureGroups(ObservableCollection<NomenclatureGroup> nomenclatureGroups) => await Proxy.SaveNomenclatureGroups(nomenclatureGroups);

        async internal Task<CallResult<ObservableCollection<NomenclatureGroup>>> GetNomGroups() => await Proxy.GetNomGroups();

        #endregion Nomenclature groups

        #region Hints

        async internal Task<CallResult<List<Hint>>> HintsGet() => await Proxy.HintsGet();

        #endregion Hints

        #region Positions 

        async internal Task<CallResult<Position>> PositionAdd(Position pos) => await Proxy.PositionAdd(pos);

        async internal Task<CallResult> PositionDelete(Position pos) => await Proxy.PositionDelete(pos);

        async internal Task<CallResult<ObservableCollection<Position>>> PositionsGet() => await Proxy.PositionsGet();

        async internal Task<CallResult> PositionsEdit(ObservableCollection<Position> positions) => await Proxy.PositionsEdit(positions);

        #endregion Positions 

        #region Users

        async internal Task<CallResult<User>> UserCreate(User user) => await Proxy.UserCreate(user);

        async internal Task<CallResult<ObservableCollection<User>>> UsersGet() => await Proxy.UsersGet();

        async internal Task<CallResult> UserEdit(User user) => await Proxy.UserEdit(user);

        async internal Task<CallResult> UserSelfEdit(User user) => await Proxy.UserSelfEdit(user);

        async internal Task<CallResult> UsersEdit(ObservableCollection<User> users) => await Proxy.UsersEdit(users);

        async internal Task<CallResult> UserChangePassword(User user) => await Proxy.UserChangePassword(user);

        async internal Task<CallResult> UserSelfChangePassword(User user, string oldPwd) => await Proxy.UserSelfChangePassword(user, oldPwd);

        async internal Task<CallResult> UserDelete(User user) => await Proxy.UserDelete(user);

        #endregion Users
    }
}
