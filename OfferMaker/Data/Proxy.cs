using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;

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

        public Proxy()
        {
            ServerData = new ServerData();
            LocalData = new LocalData();
        }

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<ObservableCollection<Currency>> GetCurrencies()
        {
            var callResult = await ServerData.GetCurrencies();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.CurrenciesPath);
                return callResult.Data;
            }

            var callResultLocal = await LocalData.GetCache<ObservableCollection<Currency>>(LocalDataConfig.CurrenciesPath);
            if (callResultLocal.Success)
                return callResultLocal.Data;
            return null;
        }

        /// <summary>
        /// Сохраняем настройки валют на сервере и локально
        /// </summary>
        /// <param name="currencies"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies)
        {
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
            var callResult = await ServerData.SaveNomenclatureGroups(nomenclatureGroups);
            LocalData.UpdateCache(nomenclatureGroups, LocalDataConfig.NomenclatureGroupsPath);
            return callResult;
        }

        /// <summary>
        /// Пытаемся получить валюты с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<ObservableCollection<Nomenclature>> GetNomenclatures()
        {
            CallResult<ObservableCollection<Nomenclature>> callResult = await ServerData.GetNomenclatures();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.NomenclaturesPath);
                return callResult.Data;
            }
  
            var callResultLocal = await LocalData.GetCache<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);
            if (callResultLocal.Success) 
                return callResultLocal.Data;
            return null;
        }

        /// <summary>
        /// Пытаемся получить категории с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<ObservableCollection<Category>> GetCategories()
        {
            CallResult<ObservableCollection<Category>> callResult = await ServerData.GetCategories();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.CategoriesPath);
                return callResult.Data;
            }

            var callResultLocal = await LocalData.GetCache<ObservableCollection<Category>>(LocalDataConfig.CategoriesPath);
            if (callResultLocal.Success)
                return callResultLocal.Data;
            return null;
        }

        /// <summary>
        /// Пытаемся получить пользователей с сервера или из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<ObservableCollection<User>> GetUsers()
        {
            CallResult<ObservableCollection<User>> callResult = await ServerData.GetUsers();
            if (callResult.Success)
            {
                LocalData.UpdateCache(callResult.Data, LocalDataConfig.UsersPath);
                return callResult.Data;
            }

            var callResultLocal = await LocalData.GetCache<ObservableCollection<User>>(LocalDataConfig.UsersPath);
            if (callResultLocal.Success)
                return callResultLocal.Data;
            return null;
        }
    }
}
