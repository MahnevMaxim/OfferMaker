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
        private ServerStore ServerStore;
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
            ServerStore = new ServerStore(accessToken);
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
                return await ServerStore.NomenclaturesGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);

            CallResult<ObservableCollection<Nomenclature>> callResult = await ServerStore.NomenclaturesGet();
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
                return await ServerStore.SaveNomenclatures(nomenclatures);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(nomenclatures, LocalDataConfig.NomenclaturesPath);

            var callResult = await ServerStore.SaveNomenclatures(nomenclatures);
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
                return await ServerStore.UsersGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);

            CallResult<ObservableCollection<User>> callResult = await ServerStore.UsersGet();
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
        async internal Task<CallResult> UsersEdit(ObservableCollection<User> users) => await ServerStore.UsersEdit(users);

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserDelete(User user) => await ServerStore.UserDelete(user);

        /// <summary>
        /// Меняем пароль.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserChangePassword(User user) => await ServerStore.UserChangePassword(user);

        /// <summary>
        /// Меняем пароль текущего аккаунта.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfChangePassword(User user, string oldPwd) => await ServerStore.UserSelfChangePassword(user, oldPwd);

        /// <summary>
        /// Сохранение данных пользователя, работает только онлайн.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserEdit(User user) => await ServerStore.UserEdit(user);

        /// <summary>
        /// Сохранение данных текущего пользователя, работает только онлайн.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfEdit(User user) => await ServerStore.UserSelfEdit(user);

        /// <summary>
        /// Добавляем нового пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult<User>> UserCreate(User user) => await ServerStore.UserCreate(user);

        #endregion Users

        #region Position

        /// <summary>
        /// Добавление новой должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult<Position>> PositionAdd(Position pos) => await ServerStore.PositionAdd(pos);

        /// <summary>
        /// Получаем должности.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Position>>> PositionsGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.PositionsGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Position>>(LocalDataConfig.PositionsPath);

            CallResult<ObservableCollection<Position>> callResult = await ServerStore.PositionsGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.PositionsPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Position>>(LocalDataConfig.PositionsPath);
        }

        /// <summary>
        /// Удаление должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionDelete(Position pos) => await ServerStore.PositionDelete(pos);

        /// <summary>
        /// Сохраняем изменения в должностях.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionsEdit(ObservableCollection<Position> positions) => await ServerStore.PositionsEdit(positions);

        #endregion Position

        #region Currencies

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.GetCurrencies();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Currency>>(LocalDataConfig.CurrenciesPath);

            var callResult = await ServerStore.GetCurrencies();
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
                return await ServerStore.SaveCurrencies(currencies);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(currencies, LocalDataConfig.CurrenciesPath);

            var callResult = await ServerStore.SaveCurrencies(currencies);
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
                return await ServerStore.GetNomGroups();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<NomenclatureGroup>>(LocalDataConfig.NomenclatureGroupsPath);

            CallResult<ObservableCollection<NomenclatureGroup>> callResult = await ServerStore.GetNomGroups();
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
                return await ServerStore.SaveNomenclatureGroups(nomenclatureGroups);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(nomenclatureGroups, LocalDataConfig.NomenclatureGroupsPath);

            var callResult = await ServerStore.SaveNomenclatureGroups(nomenclatureGroups);
            LocalData.UpdateCache(nomenclatureGroups, LocalDataConfig.NomenclatureGroupsPath);
            return callResult;
        }

        #endregion Nomenclature groups

        #region Offer templates

        async internal Task<CallResult> OfferTemplateCreate(Offer offer, ObservableCollection<Offer> offers)
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OfferTemplateCreate(offer);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(offers, LocalDataConfig.OfferTemplatesPath);

            var callResult = await ServerStore.OfferTemplateCreate(offer);
            LocalData.UpdateCache(offers, LocalDataConfig.OfferTemplatesPath);
            return callResult;
        }

        async internal Task<CallResult<ObservableCollection<Offer>>> OfferTemplatesGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OfferTemplatesGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OfferTemplatesPath);

            CallResult<ObservableCollection<Offer>> callResult = await ServerStore.OfferTemplatesGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.OfferTemplatesPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OfferTemplatesPath);
        }

        #endregion Offer templates

        #region Offers

        /// <summary>
        /// Пытаемся получить КП с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OffersGet();
            if (AppMode == AppMode.Offline)
            {
                var cacheResult = await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);
                var localResult = await LocalData.GetLocalData<List<Offer>>(LocalDataConfig.OffersPath);
                return MergeCallResults(cacheResult, localResult);
            }

            //в режиме auto получаем 2 колекции, если что-то локально помечено к удалению, то также помечаем к удалению данные сервера
            var serverResult = await ServerStore.OffersGet();
            if (serverResult.Success)
                LocalData.UpdateCache(serverResult.Data, LocalDataConfig.OffersPath);
            var localResult_ = await LocalData.GetLocalData<List<Offer>>(LocalDataConfig.OffersPath);
            return MergeCallResults(serverResult, localResult_);
        }

        /// <summary>
        /// Пытаемся получить КП текущего пользователя с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersSelfGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OffersSelfGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);

            CallResult<ObservableCollection<Offer>> callResult = await ServerStore.OffersSelfGet();
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
        async internal Task<CallResult> OfferCreate(Offer offer)
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OfferCreate(offer);
            if (AppMode == AppMode.Offline)
                return await LocalData.Post<Offer>(offer, LocalDataConfig.OffersPath);

            var remoteResult = await ServerStore.OfferCreate(offer);
            string message = remoteResult.Message;
            var localResult = await LocalData.Post<Offer>(offer, LocalDataConfig.OffersPath);
            message += "\n" + localResult.Message;
            if (remoteResult.Success && localResult.Success)
                return new CallResult() { SuccessMessage = message };
            else
                return new CallResult() { Error = new Error(message) };
        }

        /// <summary>
        /// Удаляем КП из кэша и с сервера.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="offers"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferDelete(Offer offer)
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.OfferDelete(offer);
            if (AppMode == AppMode.Offline)
                return await LocalData.Delete<Offer>(offer, LocalDataConfig.OffersPath, true);
              
            var serverResult = await ServerStore.OfferDelete(offer);
            var localResult = await LocalData.Delete<Offer>(offer, LocalDataConfig.OffersPath, !serverResult.Success);
            return MergeCallResults(serverResult, localResult);
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
                return await ServerStore.SaveCategories(categoriesTree);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(categoriesTree, LocalDataConfig.CategoriesPath);

            var callResult = await ServerStore.SaveCategories(categoriesTree);
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
                return await ServerStore.GetCategories();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Category>>(LocalDataConfig.CategoriesPath);

            CallResult<ObservableCollection<Category>> callResult = await ServerStore.GetCategories();
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
                return await ServerStore.HintsGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<List<Hint>>(LocalDataConfig.HintsPath);

            CallResult<List<Hint>> callResult = await ServerStore.HintsGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.HintsPath);
                return callResult;
            }
            return await LocalData.GetCache<List<Hint>>(LocalDataConfig.HintsPath);
        }

        #endregion Hints

        #region Banners

        async internal Task<CallResult> BannerCreate(Banner banner) => await ServerStore.BannerCreate(banner);

        async internal Task<CallResult<ObservableCollection<Banner>>> BannersGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.BannersGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Banner>>(LocalDataConfig.BannersPath);

            CallResult<ObservableCollection<Banner>> callResult = await ServerStore.BannersGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.BannersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Banner>>(LocalDataConfig.BannersPath);
        }

        async internal Task<CallResult> BannerDelete(Banner banner) => await ServerStore.BannerDelete(banner);

        async internal Task<CallResult<ObservableCollection<Advertising>>> AdvertisingsGet()
        {
            if (AppMode == AppMode.Online)
                return await ServerStore.AdvertisingsGet();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Advertising>>(LocalDataConfig.AdvertisingsPath);

            CallResult<ObservableCollection<Advertising>> callResult = await ServerStore.AdvertisingsGet();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.AdvertisingsPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Advertising>>(LocalDataConfig.AdvertisingsPath);
        }

        async internal Task<CallResult> AdvertisingCreate(Advertising advertising) => await ServerStore.AdvertisingCreate(advertising);

        async internal Task<CallResult> AdvertisingDelete(int id) => await ServerStore.AdvertisingDelete(id);

        #endregion Banners

        /// <summary>
        /// Слияние
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private CallResult<ObservableCollection<T>> MergeCallResults<T>(CallResult<ObservableCollection<T>> serverData, CallResult<List<T>> localData) where T : IEntity
        {
            ObservableCollection<T> result = new ObservableCollection<T>();
            string message = serverData.Message;
            message += "\n" + localData.Message;
            if (serverData.Success)
                result = serverData.Data;
            if (localData.Success)
            {
                localData.Data.Where(o => o.Id == 0).ToList().ForEach(o => result.Add(o));
                foreach (var entity in localData.Data.Where(o => o.Id != 0))
                {
                    var entity_ = result.FirstOrDefault(o => o.Id == entity.Id);
                    if (entity_ != null && entity.IsDelete)
                        entity_.IsDelete = true;
                }
            }

            if (localData.Success && serverData.Success)
                return new CallResult<ObservableCollection<T>>() { SuccessMessage = message, Data = result };
            else
                return new CallResult<ObservableCollection<T>>() { Error = new Error(message), Data = result };
        }

        private CallResult MergeCallResults(CallResult serverData, CallResult localData) 
        {
            string message = serverData.Message;
            message += "\n" + localData.Message;

            if (localData.Success && serverData.Success)
                return new CallResult() { SuccessMessage = message };
            else
                return new CallResult() { Error = new Error(message) };
        }
    }
}
