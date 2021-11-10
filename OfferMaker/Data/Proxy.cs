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
                LocalData.UpdateCurrencies(callResult.Data);
                return callResult.Data;
            }
            var callResultLocal = await LocalData.GetCurrencies(); 
            if (callResultLocal.Success)
                return callResultLocal.Data;
            return null;
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
                LocalData.UpdateNomenclatures(callResult.Data);
                return callResult.Data;
            }
  
            var callResultLocal = await LocalData.GetNomenclatures();
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
                LocalData.UpdateCategories(callResult.Data);
                return callResult.Data;
            }

            var callResultLocal = await LocalData.GetCategories();
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
                LocalData.UpdateUsers(callResult.Data);
                return callResult.Data;
            }

            var callResultLocal = await LocalData.GetUsers();
            if (callResultLocal.Success)
                return callResultLocal.Data;
            return null;
        }
    }
}
