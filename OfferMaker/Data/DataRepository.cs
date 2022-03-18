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
    /// абстрагирование разработчика и логики приложения от источников данных,
    /// по сути это интерфейс.
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

        public static DataRepository GetInstance(AppMode appMode, string accessToken = null)
        {
            instance.Proxy = new Proxy(appMode, accessToken);
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

        async internal Task<CallResult> OfferCreate(Offer offer) => await Proxy.OfferCreate(offer);

        async internal Task<CallResult> OfferDelete(Offer offer) => await Proxy.OfferDelete(offer);

        #endregion Offers

        #region Offer templates

        async internal Task<CallResult> OfferTemplateCreate(Offer offer) => await Proxy.OfferTemplateCreate(offer);

        async internal Task<CallResult<ObservableCollection<Offer>>> OfferTemplatesGet() => await Proxy.OfferTemplatesGet();

        async internal Task<CallResult> OfferTemplateDelete(Offer offer) => await Proxy.OfferTemplateDelete(offer);

        async internal Task<CallResult> OfferTemplateEdit(Offer offer) => await Proxy.OfferTemplateEdit(offer);

        #endregion Offer templates

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

        #region Banners

        async internal Task<CallResult> BannerCreate(Banner banner) => await Proxy.BannerCreate(banner);

        async internal Task<CallResult<ObservableCollection<Banner>>> BannersGet() => await Proxy.BannersGet();

        async internal Task<CallResult> BannerDelete(Banner banner) => await Proxy.BannerDelete(banner);

        async internal Task<CallResult<ObservableCollection<Advertising>>> AdvertisingsGet() => await Proxy.AdvertisingsGet();

        async internal Task<CallResult> AdvertisingCreate(Advertising advertising) => await Proxy.AdvertisingCreate(advertising);

        async internal Task<CallResult> AdvertisingDelete(int id) => await Proxy.AdvertisingDelete(id);

        #endregion Banners

        #region ImageGuids

        async internal Task<CallResult<HashSet<string>>> ImageGuidsGet() => await Proxy.ImageGuidsGet();

        #endregion ImageGuids
    }
}
