using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;
using ApiLib;

namespace OfferMaker
{
    class ServerData
    {
        Client client;
        System.Net.Http.HttpClient httpClient;
        string apiEndpoint = "https://localhost:44378/";

        public ServerData()
        {
            httpClient = new System.Net.Http.HttpClient();
            client = new Client(apiEndpoint, httpClient);
        }

        /// <summary>
        /// Пытаемся получить валюты с сервера
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            try
            {
                var response = await client.CurrenciesAllAsync();
                ObservableCollection<Currency> res = Helpers.CloneObject<ObservableCollection<Currency>>(response); 
                return new CallResult<ObservableCollection<Currency>>() { Data = res };
            }
            catch (Exception ex)
            {
                return new CallResult<ObservableCollection<Currency>>() { Error = new Error("Ошибка при попытке получить валюты с сервера", ex) };
            }
        }

        /// <summary>
        /// Пытаемся получить номенклатуру с сервера
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> GetNomenclatures()
        {
            try
            {
                var response = await client.NomenclaturesAllAsync();
                ObservableCollection<Nomenclature> res = Helpers.CloneObject<ObservableCollection<Nomenclature>>(response);
                return new CallResult<ObservableCollection<Nomenclature>>() { Data = res };
            }
            catch (Exception ex)
            {
                return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error("Ошибка при попытке получить номенклатуру с сервера", ex) };
            }
        }

        /// <summary>
        /// Пытаемся получить категории с сервера
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories()
        {
            try
            {
                var response = await client.CategoriesAllAsync();
                ObservableCollection<Category> res = Helpers.CloneObject<ObservableCollection<Category>>(response);
                return new CallResult<ObservableCollection<Category>>() { Data = res };
            }
            catch (Exception ex)
            {
                return new CallResult<ObservableCollection<Category>>() { Error = new Error("Ошибка при попытке получить категории с сервера", ex) };
            }
        }

        /// <summary>
        /// Пытаемся получить пользователей с сервера
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<User>>> GetUsers()
        {
            try
            {
                var response = await client.UsersAllAsync();
                ObservableCollection<User> res = Helpers.CloneObject<ObservableCollection<User>>(response);
                return new CallResult<ObservableCollection<User>>() { Data = res };
            }
            catch (Exception ex)
            {
                return new CallResult<ObservableCollection<User>>() { Error = new Error("Ошибка при попытке получить пользователей с сервера", ex) };
            }
        }
    }
}
