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
    /// от доступности либо в зависимости от настроек приложения
    /// </summary>
    class Proxy
    {
        private ServerData ServerData;
        private LocalData LocalData;
        AppMode AppMode { get => Global.Main.Settings.AppMode; }

        public Proxy()
        {
            ServerData = new ServerData();
            LocalData = new LocalData();
        }

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            if(AppMode==AppMode.Online)
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
        /// Пытаемся получить валюты с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> GetNomenclatures()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetNomenclatures();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);

            CallResult<ObservableCollection<Nomenclature>> callResult = await ServerData.GetNomenclatures();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.NomenclaturesPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);
        }

        /// <summary>
        /// Пытаемся получить КП с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> GetOffers()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetOffers();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);

            CallResult<ObservableCollection<Offer>> callResult = await ServerData.GetOffers();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.OffersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<Offer>>(LocalDataConfig.OffersPath);
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

        /// <summary>
        /// Пытаемся получить пользователей с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<User>>> GetUsers()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetUsers();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);

            CallResult<ObservableCollection<User>> callResult = await ServerData.GetUsers();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.UsersPath);
                return callResult;
            }
            return await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);
        }

        /// <summary>
        /// Пытаемся получить хинты с сервера или из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<StringCollection>> GetHints()
        {
            if (AppMode == AppMode.Online)
                return await ServerData.GetHints();
            if (AppMode == AppMode.Offline)
                return await LocalData.GetCache<StringCollection>(LocalDataConfig.HintsPath);

            CallResult<StringCollection> callResult = await ServerData.GetHints();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.HintsPath);
                return callResult;
            }
            return await LocalData.GetCache<StringCollection>(LocalDataConfig.HintsPath);
        }

        /// <summary>
        /// Пытаемся сохранить КП на сервере или в кэше.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveOffer(Offer offer)
        {
            if (AppMode == AppMode.Online)
                return await ServerData.SaveOffer(offer);
            if (AppMode == AppMode.Offline)
                return LocalData.UpdateCache(offer, LocalDataConfig.OffersPath);

            var callResult = await ServerData.SaveOffer(offer);
            LocalData.UpdateCache(offer, LocalDataConfig.OffersPath);
            return callResult;
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

        /// <summary>
        /// Сохраняем настройки валют на сервере и локально
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

        /// <summary>
        /// Сохраняем группы номенклатур на сервере и локально
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
    }
}
