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
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace OfferMaker
{
    class ServerStore
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
        static readonly string positionsEditErrorMess = "Ошибка при попытке изменить разрешения.";
        static readonly string userAddErrorMess = "Ошибка при попытке добавить пользователя.";
        static readonly string userChangePasswordErrorMess = "Ошибка при попытке обновить пароль.";
        static readonly string userDeleteErrorMess = "Ошибка при попытке удалить пользователя.";
        static readonly string getHintsErrorMess = "Ошибка при попытке получить хинты.";
        static readonly string offerCreateErrorMess = "Ошибка при попытке сохранить КП на сервере.";
        static readonly string offerDeleteErrorMess = "Ошибка при попытке удалить КП с сервера.";
        static readonly string usersEditErrorMess = "Ошибка при попытке сохранить пользователей на сервере.";
        static readonly string userEditErrorMess = "Ошибка при попытке сохранить пользователя на сервере.";
        static readonly string offersSelfGetErrorMess = "Ошибка при попытке получить свои КП с сервера.";
        static readonly string offerTemplatesGetErrorMess = "Ошибка при попытке получить шаблоны с сервера.";
        static readonly string bannerCreateErrorMess = "Ошибка при попытке загрузить баннер.";
        static readonly string bannersGetErrorMess = "Ошибка при попытке получить баннеры с сервера.";
        static readonly string bannerDeleteErrorMess = "Ошибка при попытке удалить баннер с сервера.";
        static readonly string advertisingsGetErrorMess = "Ошибка при попытке получить рекламные изображения с сервера.";
        static readonly string advertisingCreateErrorMess = "Ошибка при попытке создать рекламное изображение с сервера.";
        static readonly string advertisingDeleteErrorMess = "Ошибка при попытке удалить рекламное изображение с сервера.";
        static readonly string imageGuidsGetErrorMess = "Ошибка при попытке получить список изображений с сервера.";

        public ServerStore(string accessToken)
        {
            httpClient = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 0, 10)
            };
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
                    return GetApiError(userDeleteErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(userDeleteErrorMess, ex);
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
                if (user.Image != null)
                    Global.ImageManager.UploadImage(user);
                ApiLib.User userCopy = Helpers.CloneObject<ApiLib.User>(user);
                if (userCopy.Image?.Guid == null)
                    userCopy.Image = null;
                var response = await client.UserCreateAsync(userCopy);
                if (response.StatusCode == 201)
                {
                    User res = Helpers.CloneObject<User>(response.Result);
                    user.Id = res.Id;
                    return new CallResult<User>() { Data = user, SuccessMessage = "Пользователь добавлен" };
                }
                else
                {
                    return GetApiError<User>(userAddErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<User>(userAddErrorMess, ex);
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
                    return GetApiError<ObservableCollection<User>>(getUsersErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<User>>(getUsersErrorMess, ex);
            }
        }

        /// <summary>
        /// Меняем пароль.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult> UserChangePassword(User user)
        {
            try
            {
                ApiLib.User userCopy = Helpers.CloneObject<ApiLib.User>(user);
                var response = await client.UserChangePasswordAsync(user.Pwd, userCopy);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Пароль обновлён" };
                }
                else
                {
                    return GetApiError(userChangePasswordErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(userChangePasswordErrorMess, ex);
            }
        }

        /// <summary>
        /// Меняем пароль текущего аккаунта.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfChangePassword(User user, string oldPwd)
        {
            try
            {
                ApiLib.User userCopy = Helpers.CloneObject<ApiLib.User>(user);
                var response = await client.UserSelfChangePasswordAsync(oldPwd, user.Pwd, userCopy);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Пароль обновлён" };
                }
                else
                {
                    return GetApiError(userChangePasswordErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(userChangePasswordErrorMess, ex);
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
                    if (u.Image != null && u.Image.Guid != null)
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
                return GetApiError(usersEditErrorMess, ex);
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
                if (user.Image != null)
                    Global.ImageManager.UploadImage(user);
                ApiLib.User us = Helpers.CloneObject<ApiLib.User>(user);
                if (us.Image.Guid == null)
                    us.Image = null;
                await client.UserEditAsync(us.Id, us);
                return new CallResult() { SuccessMessage = "Настройки пользователя сохранены" };
            }
            catch (Exception ex)
            {
                return GetApiError(userEditErrorMess, ex);
            }
        }

        /// <summary>
        /// Редактирование текущего пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        async internal Task<CallResult> UserSelfEdit(User user)
        {
            try
            {
                if (user.Image != null)
                    Global.ImageManager.UploadImage(user);
                ApiLib.User user_ = Helpers.CloneObject<ApiLib.User>(user);
                await client.UserSelfEditAsync(user.Id, user_);
                return new CallResult() { SuccessMessage = "Настройки пользователя сохранены" };
            }
            catch (Exception ex)
            {
                return GetApiError(userEditErrorMess, ex);
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
                    return GetApiError<ObservableCollection<Position>>(positionsGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Position>>(positionsGetErrorMess, ex);
            }
        }

        /// <summary>
        /// Сохранение изменений в должностях на сервере.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        async internal Task<CallResult> PositionsEdit(ObservableCollection<Position> positions)
        {
            try
            {
                IEnumerable<ApiLib.Position> positionsCopy = Helpers.CloneObject<IEnumerable<ApiLib.Position>>(positions);
                ApiResponse<ICollection<ApiLib.Position>> result = await client.PositionsEditAsync(positionsCopy);
                string message = "";
                result.Result.ToList().ForEach(p =>
                {
                    message += "Разрешения для должности " + p.PositionName + " сохранены.\n";
                });
                return new CallResult() { SuccessMessage = message.Trim() };
            }
            catch (Exception ex)
            {
                return GetApiError(positionsEditErrorMess, ex);
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
                    return GetApiError(positionDeleteErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(positionDeleteErrorMess, ex);
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
                    return GetApiError<Position>(positionAddErrorMess, response.StatusCode);
                }
            }
            catch (ApiException ex)
            {
                Log.Write(ex);
                if (ex.StatusCode == 409)
                {
                    return new CallResult<Position>() { Error = new Error(positionAddErrorMess + " Должность с таким названием уже существует.") };
                }
                return GetApiError<Position>(positionAddErrorMess, ex);
            }
            catch (Exception ex)
            {
                return GetApiError<Position>(positionAddErrorMess, ex);
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
                var response = await client.CurrenciesGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Currency> res = Helpers.CloneObject<ObservableCollection<Currency>>(response.Result);
                    return new CallResult<ObservableCollection<Currency>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Currency>>() { Error = new Error(getCurrencyErrorMess, response.StatusCode) };
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
                await client.CurrenciesEditAsync(currs);
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
        async internal Task<CallResult<ObservableCollection<Nomenclature>>> NomenclaturesGet()
        {
            try
            {
                var response = await client.NomenclaturesGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Nomenclature> res = Helpers.CloneObject<ObservableCollection<Nomenclature>>(response.Result);
                    return new CallResult<ObservableCollection<Nomenclature>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Nomenclature>>() { Error = new Error(getNomenclaturesErrorMess, response.StatusCode) };
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
                var response = await client.NomenclaturesSaveAsync(noms);
                foreach (var nom in response.Result)
                {
                    var newNom = newNoms.Where(n => n.Guid == nom.Guid).FirstOrDefault();
                    if (newNom != null)
                        newNom.Id = nom.Id;
                }
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
                var response = await client.CategoriesGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Category> res = Helpers.CloneObject<ObservableCollection<Category>>(response.Result);
                    return new CallResult<ObservableCollection<Category>>() { Data = res };
                }
                else
                {
                    return new CallResult<ObservableCollection<Category>>() { Error = new Error(getCategoriesErrorMess, response.StatusCode) };
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
                await client.CategoriesSaveAsync(categoriesTreeCopy);
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
                var response = await client.NomenclatureGroupsGetAsync();
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
                await client.NomenclatureGroupsSaveAsync(nomeGroups);
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
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersGet()
        {
            try
            {
                var response = await client.OffersGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Offer> res = Helpers.CloneObject<ObservableCollection<Offer>>(response.Result);
                    return new CallResult<ObservableCollection<Offer>>() { Data = res };
                }
                else
                {
                    return GetApiError<ObservableCollection<Offer>>(getOffersErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Offer>>(getOffersErrorMess, ex);
            }
        }

        /// <summary>
        /// Пытаемся получить архив КП текущего пользователя с сервера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OffersSelfGet()
        {
            try
            {
                var response = await client.OffersSelfGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Offer> res = Helpers.CloneObject<ObservableCollection<Offer>>(response.Result);
                    return new CallResult<ObservableCollection<Offer>>() { Data = res };
                }
                else
                {
                    return GetApiError<ObservableCollection<Offer>>(offersSelfGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Offer>>(offersSelfGetErrorMess, ex);
            }
        }

        /// <summary>
        /// Пытаемся сохранить КП на сервере.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferCreate(Offer offer)
        {
            try
            {
                Global.ImageManager.UploadNewImages(offer);
                ApiLib.Offer offerCopy = Helpers.CloneObject<ApiLib.Offer>(offer);
                var res = await client.OfferPostAsync(offerCopy);
                offer.Id = res.Result.Id;
                return new CallResult() { SuccessMessage = "КП сохранено в архив" };
            }
            catch (Exception ex)
            {
                return GetApiError(offerCreateErrorMess, ex);
            }
        }

        /// <summary>
        /// Удаление КП с сервера.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferDelete(Offer offer)
        {
            try
            {
                if (offer.Id == 0) return new CallResult(); // нельзя удалить то, чего нет
                await client.OfferDeleteAsync(offer.Id);
                return new CallResult() { SuccessMessage = "КП Id " + offer.Id + " удалено." };
            }
            catch (Exception ex)
            {
                return GetApiError(offerDeleteErrorMess, ex);
            }
        }

        #endregion Offers

        #region Offer templates

        /// <summary>
        /// ПОлучение всех шаблонов.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Offer>>> OfferTemplatesGet()
        {
            try
            {
                var response = await client.OfferTemplatesGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Offer> res = Helpers.CloneObject<ObservableCollection<Offer>>(response.Result);
                    return new CallResult<ObservableCollection<Offer>>() { Data = res };
                }
                else
                {
                    return GetApiError<ObservableCollection<Offer>>(offerTemplatesGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Offer>>(offerTemplatesGetErrorMess, ex);
            }
        }

        /// <summary>
        /// Создание шаблона.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        async internal Task<CallResult> OfferTemplateCreate(Offer offer)
        {
            try
            {
                ApiLib.OfferTemplate offerCopy = Helpers.CloneObject<ApiLib.OfferTemplate>(offer);
                var res = await client.OfferTemplatePostAsync(offerCopy);
                offer.Id = res.Result.Id;
                return new CallResult() { SuccessMessage = "Шаблон сохранён." };
            }
            catch (Exception ex)
            {
                return GetApiError(offerCreateErrorMess, ex);
            }
        }

        async internal Task<CallResult> OfferTemplateDelete(Offer offer)
        {
            try
            {
                ApiLib.OfferTemplate offerCopy = Helpers.CloneObject<ApiLib.OfferTemplate>(offer);
                if (offerCopy.Id == 0) return new CallResult(); // нельзя удалить то, чего нет
                await client.OfferTemplateDeleteAsync(offerCopy.Id);
                return new CallResult() { SuccessMessage = "Шаблон Id " + offerCopy.Id + " удалён." };
            }
            catch (Exception ex)
            {
                return GetApiError(offerDeleteErrorMess, ex);
            }
        }

        async internal Task<CallResult> OfferTemplateEdit(Offer offer)
        {
            try
            {
                ApiLib.OfferTemplate offerCopy = Helpers.CloneObject<ApiLib.OfferTemplate>(offer);
                var cr = await client.OfferTemplateEditAsync(offerCopy.Id, offerCopy);
                return new CallResult() { SuccessMessage = "Шаблон Id " + offerCopy.Id + " изменён." };
            }
            catch (Exception ex)
            {
                return GetApiError(offerDeleteErrorMess, ex);
            }
        }

        #endregion Offer templates

        #region Hints

        /// <summary>
        /// Получение всех хинтов.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<List<Hint>>> HintsGet()
        {
            try
            {
                var response = await client.HintsGetAsync();
                if (response.StatusCode == 200)
                {
                    List<Hint> res = Helpers.CloneObject<List<Hint>>(response.Result);
                    return new CallResult<List<Hint>>() { Data = res };
                }
                else
                {
                    return GetApiError<List<Hint>>(getHintsErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<List<Hint>>(getHintsErrorMess, ex);
            }
        }

        #endregion Hints

        #region Banners

        /// <summary>
        /// Создание баннера.
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        async internal Task<CallResult> BannerCreate(Banner banner)
        {
            try
            {
                CallResult cr = await Global.ImageManager.UploadBanner(banner);
                if (cr.Success)
                {
                    ApiLib.Banner bannerCopy = Helpers.CloneObject<ApiLib.Banner>(banner);
                    var res = await client.BannerPostAsync(bannerCopy);
                    return new CallResult() { SuccessMessage = "Баннер загружен" };
                }
                else
                {
                    return new CallResult() { Error = new Error(bannerCreateErrorMess) };
                }
            }
            catch (Exception ex)
            {
                return GetApiError(bannerCreateErrorMess, ex);
            }
        }

        /// <summary>
        /// Получение одиночного баннера.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Banner>>> BannersGet()
        {
            try
            {
                var response = await client.BannersGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Banner> res = Helpers.CloneObject<ObservableCollection<Banner>>(response.Result);
                    return new CallResult<ObservableCollection<Banner>>() { Data = res };
                }
                else
                {
                    return GetApiError<ObservableCollection<Banner>>(bannersGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Banner>>(bannersGetErrorMess, ex);
            }
        }

        /// <summary>
        /// Удаление баннера.
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        async internal Task<CallResult> BannerDelete(Banner banner)
        {
            try
            {
                var response = await client.BannerDeleteAsync(banner.Id);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Баннер удалён." };
                }
                else
                {
                    return GetApiError(bannerDeleteErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(bannerDeleteErrorMess, ex);
            }
        }

        /// <summary>
        /// Получение рекламы.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<ObservableCollection<Advertising>>> AdvertisingsGet()
        {
            try
            {
                var response = await client.AdvertisingsGetAsync();
                if (response.StatusCode == 200)
                {
                    ObservableCollection<Advertising> res = Helpers.CloneObject<ObservableCollection<Advertising>>(response.Result);
                    return new CallResult<ObservableCollection<Advertising>>() { Data = res };
                }
                else
                {
                    return GetApiError<ObservableCollection<Advertising>>(advertisingsGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<ObservableCollection<Advertising>>(advertisingsGetErrorMess, ex);
            }
        }

        /// <summary>
        /// СОздание рекламы.
        /// </summary>
        /// <param name="advertising"></param>
        /// <returns></returns>
        async internal Task<CallResult> AdvertisingCreate(Advertising advertising)
        {
            try
            {
                CallResult cr = await Global.ImageManager.UploadBanner(advertising);
                if (cr.Success)
                {
                    ApiLib.Advertising advertisingCopy = Helpers.CloneObject<ApiLib.Advertising>(advertising);
                    var res = await client.AdvertisingPostAsync(advertisingCopy);
                    return new CallResult() { SuccessMessage = "Рекламное изображение загружено" };
                }
                else
                {
                    return new CallResult() { Error = new Error(advertisingCreateErrorMess) };
                }
            }
            catch (Exception ex)
            {
                return GetApiError(advertisingCreateErrorMess, ex);
            }
        }

        /// <summary>
        /// Удаление рекламы.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async internal Task<CallResult> AdvertisingDelete(int id)
        {
            try
            {
                var response = await client.AdvertisingDeleteAsync(id);
                if (response.StatusCode == 204)
                {
                    return new CallResult() { SuccessMessage = "Рекламное изображение удалено." };
                }
                else
                {
                    return GetApiError(advertisingDeleteErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError(advertisingDeleteErrorMess, ex);
            }
        }

        #endregion Banners

        #region ImageGuids

        async internal Task<CallResult<HashSet<string>>> ImageGuidsGet()
        {
            try
            {
                var response = await client.ImageGuidsGetAsync();
                if (response.StatusCode == 200)
                {
                    List<string> list = new List<string>();
                    foreach(var imageGuid in response.Result)
                    {
                        list.Add(imageGuid.Guid);
                    }
                    HashSet<string> set = new HashSet<string>(list);
                    return new CallResult<HashSet<string>>() { Data = set };
                }
                else
                {
                    return GetApiError<HashSet<string>>(imageGuidsGetErrorMess, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return GetApiError<HashSet<string>>(imageGuidsGetErrorMess, ex);
            }
        }

        #endregion ImageGuids

        #region Errors

        private CallResult<T> GetApiError<T>(string errorMess, int statusCode) => new CallResult<T>() { Error = GetApiError(errorMess, statusCode).Error };

        private CallResult GetApiError(string errorMess, int statusCode) => new CallResult() { Error = new Error(errorMess + "Код ошибки " + statusCode, statusCode) };

        private CallResult<T> GetApiError<T>(string errorMess, Exception ex) => new CallResult<T>() { Error = GetApiError(errorMess, ex).Error };

        private CallResult GetApiError(string errorMess, Exception ex)
        {
            Log.Write(ex);
            if (ex is ApiException)
            {
                ApiException apiEx = ex as ApiException;
                if (apiEx.StatusCode == 403)
                {
                    return new CallResult() { Error = new Error(errorMess + " Нет прав.", apiEx.StatusCode) };
                }
                else if (apiEx.StatusCode == 404)
                {
                    return new CallResult() { Error = new Error(errorMess + " Объект не найден.", apiEx.StatusCode) };
                }
                else if (apiEx.StatusCode == 503)
                {
                    return new CallResult() { Error = new Error(errorMess + " Сервер недоступен.", apiEx.StatusCode) };
                }
                else
                {
                    string errorDetails = "";
                    if (apiEx.Response != null)
                        errorDetails = TryGetErrorDetails(apiEx.Response);
                    string errorMessage = errorMess + "Код ошибки " + apiEx.StatusCode;
                    if (!string.IsNullOrWhiteSpace(errorDetails))
                        errorMessage += "\n" + errorDetails;
                    return new CallResult() { Error = new Error(errorMessage, apiEx.StatusCode) };
                }
            }
            else
            {
                return new CallResult() { Error = new Error(errorMess) };
            }
        }

        private string TryGetErrorDetails(string response)
        {
            string errorDetails = "";
            try
            {
                JObject jo = JObject.Parse(response);
                errorDetails = jo["errors"].ToString();
            }
            catch (Exception ex)
            {
                errorDetails = response;
                Log.Write(ex);
            }
            return errorDetails;
        }

        #endregion
    }
}
