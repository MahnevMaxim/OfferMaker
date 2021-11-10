using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace OfferMaker
{
    class LocalData
    {
        #region Update cache

        /// <summary>
        /// Обновление кэша валют
        /// </summary>
        /// <param name="currencies"></param>
        internal void UpdateCurrencies(ObservableCollection<Currency> currencies) => Helpers.SaveObject(LocalDataConfig.CurrenciesPath, currencies);

        /// <summary>
        /// Обновление кэша номенклатуры
        /// </summary>
        /// <param name="nomenclatures"></param>
        internal void UpdateNomenclatures(ObservableCollection<Nomenclature> nomenclatures) => Helpers.SaveObject(LocalDataConfig.NomenclaturesPath, nomenclatures);

        /// <summary>
        /// Обновление кэша категорий
        /// </summary>
        /// <param name="categories"></param>
        internal void UpdateCategories(ObservableCollection<Category> categories) => Helpers.SaveObject(LocalDataConfig.CategoriesPath, categories);

        /// <summary>
        /// Обновление кэша пользователей
        /// </summary>
        /// <param name="users"></param>
        internal void UpdateUsers(ObservableCollection<User> users) => Helpers.SaveObject(LocalDataConfig.UsersPath, users);

        #endregion Update cache

        #region Get data from cache

        /// <summary>
        /// Пытаемся получить вылюты из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            ObservableCollection<Currency> res = Helpers.InitObject<ObservableCollection<Currency>>(LocalDataConfig.CurrenciesPath);
            if (res != null)
                return new CallResult<ObservableCollection<Currency>>() { Data = res };
            return new CallResult<ObservableCollection<Currency>>() { Error = new Error("Ошибка при попытке получить валюты из кэша") };
        }

        /// <summary>
        /// Пытаемся получить номенклатуру из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> GetNomenclatures()
        {
            ObservableCollection<Nomenclature> res = Helpers.InitObject<ObservableCollection<Nomenclature>>(LocalDataConfig.NomenclaturesPath);
            if (res != null)
                return new CallResult<ObservableCollection<Nomenclature>>() { Data = res };
            return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error("Ошибка при попытке получить номенклатуру из кэша") };
        }

        /// <summary>
        /// Пытаемся получить категории из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories()
        {
            ObservableCollection<Category> res = Helpers.InitObject<ObservableCollection<Category>>(LocalDataConfig.CategoriesPath);
            if (res != null)
                return new CallResult<ObservableCollection<Category>>() { Data = res };
            return new CallResult<ObservableCollection<Category>>() { Error = new Error("Ошибка при попытке получить категории из кэша") };
        }

        /// <summary>
        /// Пытаемся получить пользователей из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<User>>> GetUsers()
        {
            ObservableCollection<User> res = Helpers.InitObject<ObservableCollection<User>>(LocalDataConfig.UsersPath);
            if (res != null)
                return new CallResult<ObservableCollection<User>>() { Data = res };
            return new CallResult<ObservableCollection<User>>() { Error = new Error("Ошибка при попытке получить пользователей из кэша") };
        }

        #endregion Get data from cache
    }
}
