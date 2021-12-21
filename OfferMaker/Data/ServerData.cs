using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;
using ApiLib;
using System.Collections.Specialized;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;

namespace OfferMaker
{
    class ServerData
    {
        Client client;
        System.Net.Http.HttpClient httpClient;
        string apiEndpoint = Global.apiEndpoint;

        public ServerData()
        {
            httpClient = new System.Net.Http.HttpClient();
            client = new Client(apiEndpoint, httpClient);
        }

        /// <summary>
        /// Пытаемся получить валюты с сервера.
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
                Log.Write(ex);
                return new CallResult<ObservableCollection<Currency>>() { Error = new Error("Ошибка при попытке получить валюты с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся получить номенклатуру с сервера.
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
                Log.Write(ex);
                return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error("Ошибка при попытке получить номенклатуру с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся получить категории с сервера.
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
                Log.Write(ex);
                return new CallResult<ObservableCollection<Category>>() { Error = new Error("Ошибка при попытке получить категории с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся получить пользователей с сервера.
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
                Log.Write(ex);
                return new CallResult<ObservableCollection<User>>() { Error = new Error("Ошибка при попытке получить пользователей с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся получить группы номенклатур с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<NomenclatureGroup>>> GetNomGroups()
        {
            try
            {
                var response = await client.NomenclatureGroupsAllAsync();
                ObservableCollection<NomenclatureGroup> res = Helpers.CloneObject<ObservableCollection<NomenclatureGroup>>(response);
                return new CallResult<ObservableCollection<NomenclatureGroup>>() { Data = res };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<NomenclatureGroup>>() { Error = new Error("Ошибка при попытке получить группы номенклатур с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся получить архив КП с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> GetOffers()
        {
            try
            {
                var response = await client.OffersAllAsync();
                ObservableCollection<Offer> res = Helpers.CloneObject<ObservableCollection<Offer>>(response);
                return new CallResult<ObservableCollection<Offer>>() { Data = res };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Offer>>() { Error = new Error("Ошибка при попытке получить архив КП с сервера.") };
            }
        }

        /// <summary>
        /// Пытаемся сохранить группы номенклатур на сервере.
        /// </summary>
        /// <param name="nomenclatureGroups"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveNomenclatureGroups(ObservableCollection<NomenclatureGroup> nomenclatureGroups)
        {
            try
            {
                IEnumerable<ApiLib.NomenclatureGroup> nomeGroups = Helpers.CloneObject<IEnumerable<ApiLib.NomenclatureGroup>>(nomenclatureGroups);
                await client.NomenclatureGroupsPUTAsync(nomeGroups);
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить группы номенклатур на сервере.") };
            }
        }

        internal Task<CallResult<StringCollection>> GetHints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Пытаемся сохранить КП на сервере.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveOffer(Offer offer)
        {
            try
            {
                ApiLib.Offer offerCopy = Helpers.CloneObject<ApiLib.Offer>(offer);
                var res = await client.OffersPOSTAsync(offerCopy);
                offer.Id = res.Id;
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить КП на сервере.") };
            }
        }

        /// <summary>
        /// Сохранение валют на сервере.
        /// </summary>
        /// <param name="currencies"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveCurrencies(ObservableCollection<Currency> currencies)
        {
            try
            {
                IEnumerable<ApiLib.Currency> currs = Helpers.CloneObject<IEnumerable<ApiLib.Currency>>(currencies);
                await client.CurrenciesPUTAsync(currs);
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить валюты на сервере.") };
            }
        }

        /// <summary>
        /// Пытаемся сохранить номенклатуры на сервере.
        /// </summary>
        /// <param name="nomenclatures"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveNomenclatures(ObservableCollection<Nomenclature> nomenclatures)
        {
            try
            {
                var newNoms = nomenclatures.Where(n => n.Id == 0 || n.GetIsEdit() == true).ToList();
                Global.ImageManager.UploadNewImages(newNoms);
                IEnumerable<ApiLib.Nomenclature> noms = Helpers.CloneObject<IEnumerable<ApiLib.Nomenclature>>(newNoms);
                await client.NomenclaturesPUTAsync(noms);
                newNoms.ToList().ForEach(n => n.SkipIsEdit());
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить номенклатуры на сервере.") };
            }
        }

        /// <summary>
        /// Удаление КП с сервера.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> DeleteOfferFromArchive(Offer offer)
        {
            try
            {
                if (offer.Id == 0) return new CallResult(); // нельзя удалить то, чего нет
                await client.OffersDELETEAsync((int)offer.Id);
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке удалить КП с сервера.") };
            }
        }

        /// <summary>
        /// Сохранение категорий на сервере.
        /// </summary>
        /// <param name="categoriesTree"></param>
        /// <returns></returns>
        async internal Task<CallResult> SaveCategories(ObservableCollection<Category> categoriesTree)
        {
            try
            {
                IEnumerable<ApiLib.Category> categoriesTreeCopy = Helpers.CloneObject<IEnumerable<ApiLib.Category>>(categoriesTree);
                await client.CategoriesPUTAsync(categoriesTreeCopy);
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить номенклатуры на сервере.") };
            }
        }
    }
}
