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
using System.Net.Http.Headers;

namespace OfferMaker
{
    class ServerData
    {
        Client client;
        HttpClient httpClient;
        string apiEndpoint = Global.apiEndpoint;

        static readonly string getCurrencyErrorMess = "Ошибка при попытке получить валюты с сервера.";
        static readonly string getNomenclaturesErrorMess = "Ошибка при попытке получить номенклатуру с сервера.";
        static readonly string getCategoriesErrorMess = "Ошибка при попытке получить категории с сервера.";
        static readonly string getUsersErrorMess = "Ошибка при попытке получить пользователей с сервера.";
        static readonly string getNomenclatureGroupErrorMess = "Ошибка при попытке получить группы номенклатур с сервера.";
        static readonly string getOffersErrorMess = "Ошибка при попытке получить архив КП с сервера.";
        static readonly string positionAddErrorMess = "Ошибка при попытке добавить должность.";
        static readonly string positionDeleteErrorMess = "Ошибка при попытке удалить должность.";
        static readonly string positionsGetErrorMess = "Ошибка при попытке получить должности.";
        static readonly string userAddErrorMess = "Ошибка при попытке добавить пользователя.";
        static readonly string userChangePasswordErrorMess = "Ошибка при попытке обновить пароль.";
        static readonly string userDeleteErrorMess = "Ошибка при попытке удалить пользователя.";

        public ServerData(string accessToken)
        {
            httpClient = new HttpClient();
            if (accessToken != null)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client = new Client(apiEndpoint, httpClient);
        }

        #region Users

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserDelete(User user)
        {
            try
            {
                var response = await client.UserDeleteAsync(user.Id);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Пользователь удалён" };
                }
                else
                {
                    return new CallResult() { Error = new Error(userDeleteErrorMess) };
                }
            }
            catch (ApiException ex)
            {
                Log.Write(ex);
                if (ex.StatusCode == 404)
                {
                    return new CallResult() { Error = new Error(ex.StatusCode, userDeleteErrorMess + " Пользователь не найден.") };
                }
                else
                {
                    return new CallResult() { Error = new Error(ex.StatusCode, userDeleteErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(userDeleteErrorMess) };
            }
        }

        /// <summary>
        /// Добавляем нового пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult<User>> UserCreate(User user)
        {
            try
            {
                Global.ImageManager.UploadImage(user);
                ApiLib.User userCopy = Helpers.CloneObject<ApiLib.User>(user);
                var response = await client.UserCreateAsync(userCopy);
                if (response.StatusCode == 201)
                {
                    User res = Helpers.CloneObject<User>(response.Result);
                    user.Id = res.Id;
                    return new CallResult<User>() { Data = user, SuccessMessage = "Пользователь добавлен" };
                }
                else
                {
                    return new CallResult<User>() { Error = new Error(userAddErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<User>() { Error = new Error(userAddErrorMess) };
            }
        }

        /// <summary>
        /// Пытаемся получить пользователей с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<User>>> UsersGet()
        {
            try
            {
                var response = await client.UsersGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<User> res = Helpers.CloneObject<ObservableCollection<User>>(response.Result);
                    return new CallResult<ObservableCollection<User>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<User>>() { Error = new Error(getUsersErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<User>>() { Error = new Error(getUsersErrorMess) };
            }
        }

        /// <summary>
        /// Меняем пароль.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult> UserChangePassword(User user, string oldPwd)
        {
            try
            {
                ApiLib.User userCopy = Helpers.CloneObject<ApiLib.User>(user);
                var response = await client.UserChangePasswordAsync(oldPwd, userCopy);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Пароль обновлён" };
                }
                else
                {
                    return new CallResult() { Error = new Error(userChangePasswordErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(userChangePasswordErrorMess, ex) };
            }
        }

        /// <summary>
        /// Обновляем пользователей.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        async internal Task<CallResult> UsersEdit(ObservableCollection<User> users)
        {
            try
            {
                users.ToList().ForEach(u => 
                { 
                    if(u.Image != null && u.Image.Guid!=null)
                        Global.ImageManager.UploadImage(u);
                });
                IEnumerable<ApiLib.User> usersCopy = Helpers.CloneObject<IEnumerable<ApiLib.User>>(users);
                usersCopy.ToList().ForEach(u =>
                {
                    if (u.Image != null && u.Image.Guid == null)
                    {
                        //обнуляем фото, если стоит фото по умолчанию
                        u.Image = null;
                    }
                });
                ApiResponse<ICollection<ApiLib.User>> result = await client.UsersEditAsync(usersCopy);
                string message = "";
                result.Result.ToList().ForEach(u =>
                {
                    message += "Пользователь " + u.FirstName + " " + u.LastName + " сохранен.\n";
                });
                return new CallResult() { SuccessMessage = message.Trim() };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить пользователей на сервере.") };
            }
        }

        /// <summary>
        /// Редактирование пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserEdit(User user)
        {
            try
            {
                Global.ImageManager.UploadImage(user);
                ApiLib.User us = Helpers.CloneObject<ApiLib.User>(user);
                await client.UserEditAsync(us.Id, us);
                return new CallResult() { SuccessMessage = "Настройки пользователя сохранены" };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить пользователя на сервере.") };
            }
        }

        #endregion Users

        #region Positions

        /// <summary>
        /// Получаем должности.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Position>>> PositionsGet()
        {
            try
            {
                var response = await client.PositionsGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Position> res = Helpers.CloneObject<ObservableCollection<Position>>(response.Result);
                    return new CallResult<ObservableCollection<Position>>() { Data = res, SuccessMessage = "Должности получены." };
                }
                else
                {
                    return new CallResult<ObservableCollection<Position>>() { Error = new Error(response.StatusCode, positionsGetErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Position>>() { Error = new Error(positionsGetErrorMess) };
            }
        }

        /// <summary>
        /// Сохранение изменений в должностях на сервере.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionsSave(ObservableCollection<Position> positions)
        {
            try
            {
                IEnumerable<ApiLib.Position> positionsCopy = Helpers.CloneObject<IEnumerable<ApiLib.Position>>(positions);
                ApiResponse<ICollection<ApiLib.Position>> result = await client.PositionsSaveAsync(positionsCopy);
                string message = "";
                result.Result.ToList().ForEach(p =>
                {
                    message += "Разрешения для должности " + p.PositionName + " сохранены.\n";
                });
                return new CallResult() { SuccessMessage = message.Trim() };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить должность на сервере.") };
            }
        }

        /// <summary>
        /// Удаление должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionDelete(Position pos)
        {
            try
            {
                var response = await client.PositionDeleteAsync(pos.Id);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Должность удалена." };
                }
                else
                {
                    return new CallResult() { Error = new Error(response.StatusCode, positionDeleteErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(positionDeleteErrorMess) };
            }
        }

        /// <summary>
        /// Добавление новой должности.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        async internal Task<CallResult<Position>> PositionAdd(Position pos)
        {
            try
            {
                ApiLib.Position positionCopy = Helpers.CloneObject<ApiLib.Position>(pos);
                var response = await client.PositionPostAsync(positionCopy);
                if (response.StatusCode == 201)
                {
                    Position res = Helpers.CloneObject<Position>(response.Result);
                    pos.Id = res.Id;
                    return new CallResult<Position>() { Data = pos, SuccessMessage = "Должность добавлена" };
                }
                else
                {
                    return new CallResult<Position>() { Error = new Error(positionAddErrorMess) };
                }
            }
            catch (ApiException aex)
            {
                Log.Write(aex);
                if (aex.StatusCode == 409)
                {
                    return new CallResult<Position>() { Error = new Error(positionAddErrorMess + " Должность с таким названием уже существует.") };
                }
                else
                {
                    return new CallResult<Position>() { Error = new Error(positionAddErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<Position>() { Error = new Error(positionAddErrorMess) };
            }
        }

        #endregion Positions

        #region Currencies

        /// <summary>
        /// Пытаемся получить валюты с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Currency>>> GetCurrencies()
        {
            try
            {
                var response = await client.CurrenciesAllAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Currency> res = Helpers.CloneObject<ObservableCollection<Currency>>(response.Result);
                    return new CallResult<ObservableCollection<Currency>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Currency>>() { Error = new Error(response.StatusCode, getCurrencyErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Currency>>() { Error = new Error(getCurrencyErrorMess) };
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

        #endregion Currencies

        #region Nomenclatures

        /// <summary>
        /// Пытаемся получить номенклатуру с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> GetNomenclatures()
        {
            try
            {
                var response = await client.NomenclaturesAllAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Nomenclature> res = Helpers.CloneObject<ObservableCollection<Nomenclature>>(response.Result);
                    return new CallResult<ObservableCollection<Nomenclature>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error(response.StatusCode, getNomenclaturesErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error(getNomenclaturesErrorMess) };
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
                newNoms.AddRange(Global.Catalog.CatalogFilter.GetDeletedNoms()); //также добавляем помеченные на удаление
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

        #endregion Nomenclatures

        #region Categories

        /// <summary>
        /// Пытаемся получить категории с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Category>>> GetCategories()
        {
            try
            {
                var response = await client.CategoriesAllAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Category> res = Helpers.CloneObject<ObservableCollection<Category>>(response.Result);
                    return new CallResult<ObservableCollection<Category>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Category>>() { Error = new Error(response.StatusCode, getCategoriesErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Category>>() { Error = new Error(getCategoriesErrorMess) };
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

        #endregion Categories

        #region Nomenclature groups

        /// <summary>
        /// Пытаемся получить группы номенклатур с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<NomenclatureGroup>>> GetNomGroups()
        {
            try
            {
                var response = await client.NomenclatureGroupsAllAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<NomenclatureGroup> res = Helpers.CloneObject<ObservableCollection<NomenclatureGroup>>(response.Result);
                    return new CallResult<ObservableCollection<NomenclatureGroup>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<NomenclatureGroup>>() { Error = new Error(getNomenclatureGroupErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<NomenclatureGroup>>() { Error = new Error(getNomenclatureGroupErrorMess) };
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

        #endregion Nomenclature groups

        #region Offers

        /// <summary>
        /// Пытаемся получить архив КП с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> GetOffers()
        {
            try
            {
                var response = await client.OffersAllAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Offer> res = Helpers.CloneObject<ObservableCollection<Offer>>(response.Result);
                    return new CallResult<ObservableCollection<Offer>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Offer>>() { Error = new Error(getOffersErrorMess) };
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult<ObservableCollection<Offer>>() { Error = new Error(getOffersErrorMess) };
            }
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
                offer.Id = res.Result.Id;
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error("Ошибка при попытке сохранить КП на сервере.") };
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

        #endregion Offers

        #region Hints

        internal Task<CallResult<StringCollection>> GetHints()
        {
            throw new NotImplementedException();
        }

        #endregion Hints
    }
}
