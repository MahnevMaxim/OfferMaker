using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace OfferMaker
{
    /// <summary>
    /// Класс для получения данных из различных источников в зависимости
    /// от доступности либо в зависимости от настроек приложения.
    /// Удаление из кэша производим путём обновления объекта кэша.
    /// </summary>
    class Proxy
    {
        private ServerData ServerData;
        private LocalData LocalData;
        public AppMode defaultAppMode;

        AppMode AppMode
        {
            get
            {
                if (Global.Main == null) return defaultAppMode;
                return Global.Main.Settings.AppMode;
            }
        }

        public Proxy(string accessToken = null)
        {
            ServerData = new ServerData(accessToken);
            LocalData = new LocalData();
        }

        #region Nomenclatures

        /// <summary>
        /// Пытаемся получить валюты с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> NomenclaturesGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.NomenclaturesGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);

            CallResult<ObservableCollection<Nomenclature>> callResult = await ServerData.NomenclaturesGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.NomenclaturesPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);
        }

        /// <summary>
        /// Пытаемся сохранить номенклатуры на сервере или в кэше.
        /// </summary>
        /// <param name="nomenclatures"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveNomenclatures(ObservableCollection<Nomenclature> nomenclatures)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.SaveNomenclatures(nomenclatures);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(nomenclatures, LocalDataConfig.NomenclaturesPath);

            var callResult = await ServerData.SaveNomenclatures(nomenclatures);
            LocalData.UpdateCache(nomenclatures, LocalDataConfig.NomenclaturesPath);
            return callResult;
        }

        #endregion Nomenclatures

        #region Users

        /// <summary>
        /// Пытаемся получить пользователей с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<User>>> UsersGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.UsersGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);

            CallResult<ObservableCollection<User>> callResult = await ServerData.UsersGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.UsersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);
        }

        /// <summary>
        /// Обновляем пользователей.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        async internal Task<CallResult> UsersEdit(ObservableCollection<User> users) => await ServerData.UsersEdit(users);

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserDelete(User user) => await ServerData.UserDelete(user);

        /// <summary>
        /// Меняем пароль.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserChangePassword(User user) => await ServerData.UserChangePassword(user);

        /// <summary>
        /// Меняем пароль текущего аккаунта.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfChangePassword(User user, string oldPwd) => await ServerData.UserSelfChangePassword(user, oldPwd);

        /// <summary>
        /// Сохранение данных пользователя, работает только онлайн.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserEdit(User user) => await ServerData.UserEdit(user);

        /// <summary>
        /// Сохранение данных текущего пользователя, работает только онлайн.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfEdit(User user) => await ServerData.UserSelfEdit(user);

        /// <summary>
        /// Добавляем нового пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult<User>> UserCreate(User user) => await ServerData.UserCreate(user);

        #endregion Users

        #region Position

        /// <summary>
        /// Добавление новой должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult<Position>> PositionAdd(Position pos) => await ServerData.PositionAdd(pos);

        /// <summary>
        /// Получаем должности.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Position>>> PositionsGet() => await ServerData.PositionsGet();

        /// <summary>
        /// Удаление должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionDelete(Position pos) => await ServerData.PositionDelete(pos);

        /// <summary>
        /// Сохраняем изменения в должностях.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionsEdit(ObservableCollection<Position> positions) => await ServerData.PositionsEdit(positions);

        #endregion Position

        #region Currencies

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetCurrencies();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Currency>>(LocalDataConfig.CurrenciesPath);

            var callResult = await ServerData.GetCurrencies();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.CurrenciesPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Currency>>(LocalDataConfig.CurrenciesPath);
        }

        /// <summary>
        /// Сохраняем настройки валют на сервере и локально.
        /// </summary>
        /// <param name="currencies"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.SaveCurrencies(currencies);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(currencies, LocalDataConfig.CurrenciesPath);

            var callResult = await ServerData.SaveCurrencies(currencies);
            LocalData.UpdateCache(currencies, LocalDataConfig.CurrenciesPath);
            return callResult;
        }

        #endregion Currencies

        #region Nomenclature groups

        /// <summary>
        /// Пытаемся получить группы номенклатур с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<NomenclatureGroup>>> GetNomGroups()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetNomGroups();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<NomenclatureGroup>>(LocalDataConfig.NomenclatureGroupsPath);

            CallResult<ObservableCollection<NomenclatureGroup>> callResult = await ServerData.GetNomGroups();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.NomenclatureGroupsPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<NomenclatureGroup>>(LocalDataConfig.NomenclatureGroupsPath);
        }

        /// <summary>
        /// Сохраняем группы номенклатур на сервере и локально.
        /// </summary>
        /// <param name="currencies"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveNomenclatureGroups(ObservableCollection<NomenclatureGroup> nomenclatureGroups)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.SaveNomenclatureGroups(nomenclatureGroups);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(nomenclatureGroups, LocalDataConfig.NomenclatureGroupsPath);

            var callResult = await ServerData.SaveNomenclatureGroups(nomenclatureGroups);
            LocalData.UpdateCache(nomenclatureGroups, LocalDataConfig.NomenclatureGroupsPath);
            return callResult;
        }

        #endregion Nomenclature groups

        #region Offers

        /// <summary>
        /// Пытаемся получить КП с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.OffersGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);

            CallResult<ObservableCollection<Offer>> callResult = await ServerData.OffersGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.OffersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);
        }

        /// <summary>
        /// Пытаемся получить КП текущего пользователя с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersSelfGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.OffersSelfGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);

            CallResult<ObservableCollection<Offer>> callResult = await ServerData.OffersSelfGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.OffersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);
        }

        /// <summary>
        /// Пытаемся сохранить КП на сервере или в кэше.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferCreate(Offer offer, ObservableCollection<Offer> offers)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.OfferCreate(offer);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(offers, LocalDataConfig.OffersPath);

            var callResult = await ServerData.OfferCreate(offer);
            LocalData.UpdateCache(offers, LocalDataConfig.OffersPath);
            return callResult;
        }

        /// <summary>
        /// Удаляем КП из кэша и с сервера.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="offers"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferDelete(Offer offer, ObservableCollection<Offer> offers)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.OfferDelete(offer);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(offers, LocalDataConfig.OffersPath);

            var callResult = await ServerData.OfferDelete(offer);
            LocalData.UpdateCache(offers, LocalDataConfig.OffersPath);
            return callResult;
        }

        #endregion Offers

        #region Categories

        /// <summary>
        /// Сохраняем категории на сервере и локально.
        /// </summary>
        /// <param name="categoriesTree"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveCategories(ObservableCollection<Category> categoriesTree)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.SaveCategories(categoriesTree);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(categoriesTree, LocalDataConfig.CategoriesPath);

            var callResult = await ServerData.SaveCategories(categoriesTree);
            LocalData.UpdateCache(categoriesTree, LocalDataConfig.CategoriesPath);
            return callResult;
        }

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetCategories();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Category>>(LocalDataConfig.CategoriesPath);

            CallResult<ObservableCollection<Category>> callResult = await ServerData.GetCategories();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.CategoriesPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Category>>(LocalDataConfig.CategoriesPath);
        }

        #endregion Categories

        #region Hints

        /// <summary>
        /// Пытаемся получить хинты с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<List<Hint>>> HintsGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.HintsGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<List<Hint>>(LocalDataConfig.HintsPath);

            CallResult<List<Hint>> callResult = await ServerData.HintsGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.HintsPath);
                return callResult;
            }
            return await LocalData.GetCache<List<Hint>>(LocalDataConfig.HintsPath);
        }

        #endregion Hints
    }
}
