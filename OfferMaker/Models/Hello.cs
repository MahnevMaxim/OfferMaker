using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using ApiLib;
using System.Text.Json;
using Newtonsoft.Json;
using System.Windows;

namespace OfferMaker
{
    public class Hello : BaseModel
    {
        readonly string apiEndpoint = Global.apiEndpoint;
        Client client;
        System.Net.Http.HttpClient httpClient;
        bool isRun;
        public bool isAuthorizationOk;

        User User { get; set; }

        Main main;
        DataRepository dataRepository;
        ObservableCollection<User> users;
        ObservableCollection<Position> positions;
        ObservableCollection<Category> categories;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups;
        ObservableCollection<Offer> offers;
        ObservableCollection<Offer> offerTemplates;
        ObservableCollection<Currency> currencies;
        List<Hint> hints;
        ObservableCollection<Banner> banners;
        ObservableCollection<Advertising> advertisings;

        #region MVVM

        bool isBusy;
        string helloStatus;
        string appModeString;

        public string LogoPath { get; set; }

        public string Login { get; set; }

        public bool IsRememberMe { get; set; }

        public string Pwd { get; set; }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public string HelloStatus
        {
            get => helloStatus;
            set
            {
                helloStatus = value;
                OnPropertyChanged();
            }
        }

        public string AppModeString 
        {
            get => appModeString;
            set
            {
                appModeString = value;
                OnPropertyChanged();
            }
        }

        #endregion MVVM

        async internal override void Run()
        {
            if (isRun) return;
            isRun = true;
            InitMain();
            InitHello();
            await TryAuthAutomaticaly();
            if (User != null)
                await Init();
        }

        async private Task Init()
        {
            IsBusy = true;
            await InitData();
            IsBusy = false;
            isAuthorizationOk = true;
            Close();
        }

        public Main GetMain() => main;

        private void SetHelloStatus(string message) => HelloStatus = message;

        #region Первичная инициализация

        private void InitMain()
        {
            //настройки нужны до авторизации
            main = new Main();
            Global.Main = main;
            main.Settings = Settings.GetInstance();
            main.Settings.SetSettings();
            AppModeString = main.Settings.AppMode.ToString();
        }

        private void InitHello()
        {
            LogoPath = AppDomain.CurrentDomain.BaseDirectory + "images\\logo.png";
            IsRememberMe = Settings.GetIsRememberMe();
            Login = Settings.GetLogin();
            //инициализация хранилища
            dataRepository = DataRepository.GetInstance(Settings.GetInstance().AppMode);
            //инициализация API-клиента
            httpClient = new System.Net.Http.HttpClient();
            client = new Client(apiEndpoint, httpClient);
        }

        #endregion Первичная инициализация

        #region Комманды

        public void LoginCommand() => HandAuth();

        public void OpenSettingsCommand()
        {
            MvvmFactory.CreateWindow(Global.Settings, new ViewModels.SettingsViewModel(), new Views.Settings(true), ViewMode.ShowDialog);
            AppModeString = main.Settings.AppMode.ToString();
        }
           
        #endregion Комманды

        #region Авторизация

        async private void HandAuth()
        {
            IsBusy = true;

            var mode = Settings.GetInstance().AppMode;
            //авторизация и получение пользователя
            SetHelloStatus("авторизация и получение пользователя...");
            CallResult cr = await Auth();
            if (!cr.Success)
            {
                if (mode == AppMode.Auto || mode == AppMode.Offline)
                {
                    bool isAuthOk = await AuthLocal();
                    IsBusy = false;
                    if (!isAuthOk) return;
                }
                else
                {
                    IsBusy = false;
                    OnSendMessage(cr.Error.Message);
                    return;
                }
            }

            // Вроде бы к этому моменту User по-любому не должен быть null, но хуй его знает.
            IsBusy = false;
            if (User == null)
            {
                OnSendMessage("Пользователь не найден");
                Application.Current.Shutdown();
                return;
            }

            Settings.SetIsRememberMe(IsRememberMe);
            Settings.SetLogin(Login.Trim());
            Settings.SavePasswordForOffline(Pwd);

            await Init();
        }

        /// <summary>
        /// Локальная авторизация.
        /// </summary>
        /// <returns></returns>
        async private Task<bool> AuthLocal()
        {
            bool isOk = CheckUser();
            if (!isOk)
            {

                OnSendMessage("Неправильный логин или пароль");
                return false;
            }
            User = await GetLocalUser(Login.Trim());
            if (User == null)
            {
                IsBusy = false;
                OnSendMessage("Пользователь не найден. Это означает, что у программы нет сохранённых данных пользователя." +
                    " Пользователи сохраняются в режиме Auto во время загрузки данных. Настроить режим можно нажав на кнопку с шестерёнкой");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Авторизация по логину и паролю.
        /// </summary>
        /// <returns></returns>
        async private Task<CallResult> Auth()
        {
            try
            {
                var res = await client.AccountGetTokenAsync(Login, Pwd);
                JsonElement authRes = (JsonElement)res.Result;
                var userAsString = authRes.GetProperty("user").ToString();
                User = JsonConvert.DeserializeObject<User>(userAsString);
                Settings.SetToken(authRes.GetProperty("access_token").GetString());
                return new CallResult();
            }
            catch (ApiException ex)
            {
                return new CallResult() { Error = new Error(ex.Response.ToString()) };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(ex.Message) };
            }
        }

        /// <summary>
        /// Авторизация по токену.
        /// </summary>
        /// <returns></returns>
        async private Task<CallResult> Auth(string token)
        {
            try
            {
                var res = await client.AccountUpdateTokenAsync(token);
                JsonElement authRes = (JsonElement)res.Result;
                var userString = authRes.GetProperty("user").ToString();
                User = JsonConvert.DeserializeObject<User>(userString);
                Settings.SetToken(authRes.GetProperty("access_token").GetString());
                return new CallResult();
            }
            catch (ApiException ex)
            {
                return new CallResult() { Error = new Error(ex.Response.ToString()) };
            }
            catch (Exception ex)
            {
                return new CallResult() { Error = new Error(ex) };
            }
        }

        private bool CheckUser()
        {
            if (Login == AppSettings.Default.Login)
            {
                if (Settings.GenerateHash(Pwd) == AppSettings.Default.Pwd)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получение пользователя из коллекции кэшированных пользователей.
        /// По идее они должны быть.
        /// </summary>
        /// <returns></returns>
        async private Task<User> GetLocalUser(string login)
        {
            var usersCr = await dataRepository.UsersGet();
            if (usersCr.Success)
                return usersCr.Data.Where(u => u.Email == login).FirstOrDefault();
            return null;
        }

        async private Task TryAuthAutomaticaly()
        {
            IsBusy = true;

            try
            {
                SetHelloStatus("авторизация...");
                //Если есть токен, то  пытаемся войти по нему.
                //Если авторизация удастся, то токен обновится и обновятся права.
                if (Settings.GetIsRememberMe() && (Settings.GetInstance().AppMode == AppMode.Auto || Settings.GetInstance().AppMode == AppMode.Online))
                {
                    CallResult userCr = await Auth(Settings.GetToken());
                    if (!userCr.Success)
                    {
                        OnSendMessage(userCr.Message);
                        Log.Write(userCr.Message);
                    }
                }
                else if (Settings.GetIsRememberMe() && Settings.GetInstance().AppMode == AppMode.Offline)
                {

                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

            IsBusy = false;
        }

        #endregion  Авторизация

        #region Получение данных и создание модели

        /// <summary>
        /// Инициализация модели.
        /// </summary>
        /// <returns></returns>
        async private Task InitData()
        {
            main.DataRepository = DataRepository.GetInstance(Settings.GetInstance().AppMode, Settings.GetToken()); //инициализация хранилища
            await ReciveData(); //инициализация данных
            InitModules(); //инициализация модулей на основе данных
            await UploadImages();
        }

        /// <summary>
        /// Получение данных с сервера / из кэша.
        /// </summary>
        /// <returns></returns>
        async private Task ReciveData()
        {
            string errorMessage = "";

            //получаем должности
            SetHelloStatus("получаем должности...");
            var positionsCr = await dataRepository.PositionsGet();
            if (positionsCr.Success)
                positions = positionsCr.Data;
            else
                errorMessage += positionsCr.Error.Message + "\n";

            //получаем пользователей
            SetHelloStatus("получаем пользователей...");
            if (User.Position.Permissions.Contains(Shared.Permissions.CanAll)
                || User.Position.Permissions.Contains(Shared.Permissions.CanControlUsers)
                || User.Position.Permissions.Contains(Shared.Permissions.CanUseManager)
                || User.Position.Permissions.Contains(Shared.Permissions.CanSeeAllOffers))
            {
                var usersCr = await dataRepository.UsersGet();
                if (usersCr.Success)
                    users = usersCr.Data;
                else
                    errorMessage += usersCr.Error.Message + "\n";
            }

            //получаем валюты
            SetHelloStatus("получаем валюты...");
            var currenciesCr = await dataRepository.GetCurrencies();
            if (currenciesCr.Success)
                currencies = currenciesCr.Data;
            else
                errorMessage += currenciesCr.Error.Message + "\n";

            //получаем категории
            SetHelloStatus("получаем категории...");
            var categoriesCr = await dataRepository.GetCategories();
            if (categoriesCr.Success)
                categories = categoriesCr.Data;
            else
                errorMessage += categoriesCr.Error.Message + "\n";

            //получаем номенклатуры
            SetHelloStatus("получаем номенклатуры...");
            var nomenclaturesCr = await dataRepository.NomenclaturesGet();
            if (nomenclaturesCr.Success)
                nomenclatures = nomenclaturesCr.Data;
            else
                errorMessage += nomenclaturesCr.Error.Message + "\n";

            //получаем группы номенклатур
            SetHelloStatus("получаем группы номенклатур...");
            var nomGroupsCr = await dataRepository.GetNomGroups();
            if (nomGroupsCr.Success)
                nomenclatureGroups = nomGroupsCr.Data;
            else
                errorMessage += nomGroupsCr.Error.Message + "\n";

            //получаем архив КП
            SetHelloStatus("получаем архив КП...");
            if (User.Position.Permissions.Contains(Shared.Permissions.CanAll)
                || User.Position.Permissions.Contains(Shared.Permissions.CanSeeAllOffers))
            {
                var offersCr = await dataRepository.OffersGet();
                if (!offersCr.Success)
                    errorMessage += offersCr.Error.Message + "\n";
                offers = offersCr.Data;
            }
            else
            {
                var offersCr = await dataRepository.OffersSelfGet();
                if (offersCr.Success)
                    offers = offersCr.Data;
                else
                    errorMessage += offersCr.Error.Message + "\n";
            }

            //получаем шаблоны
            SetHelloStatus("получаем шаблоны...");
            var offerTemplatesCr = await dataRepository.OfferTemplatesGet();
            if (offerTemplatesCr.Success)
                offerTemplates = offerTemplatesCr.Data;
            else
                errorMessage += offerTemplatesCr.Error.Message + "\n";

            //получаем хинты
            SetHelloStatus("получаем хинты...");
            var hintsCr = await dataRepository.HintsGet();
            if (hintsCr.Success)
                hints = hintsCr.Data;
            else
                errorMessage += hintsCr.Error.Message + "\n";

            //получаем баннеры
            SetHelloStatus("получаем баннеры...");
            var bannersCr = await dataRepository.BannersGet();
            if (bannersCr.Success)
                banners = bannersCr.Data;
            else
                errorMessage += bannersCr.Error.Message + "\n";

            //получаем рекламу
            SetHelloStatus("получаем рекламу...");
            var advertisingsCr = await dataRepository.AdvertisingsGet();
            if (advertisingsCr.Success)
                advertisings = advertisingsCr.Data;
            else
                errorMessage += advertisingsCr.Error.Message + "\n";

            if (!string.IsNullOrWhiteSpace(errorMessage))
                OnSendMessage(errorMessage);
        }

        /// <summary>
        /// Инициализация основных моделей приложения.
        /// </summary>
        private void InitModules()
        {
            //main
            SetHelloStatus("инициализация main...");
            main.Currencies = currencies;
            main.User = users == null ? User : users.Where(u => u.Id == User.Id).FirstOrDefault();
            main.Users = users == null ? new ObservableCollection<User>() { User } : users;
            main.Users.ToList().ForEach(u =>
            {
                if (u.Image == null)
                {
                    u.Image = Global.NoProfileImage;
                }
                main.Managers.Add(u);
                if (u.Position != null)
                    u.Position = positions.Where(p => p.Id == u.Position.Id).FirstOrDefault();
            });
            Main.hints = hints;
            positions.ToList().ForEach(p => p.SetWrapperPermission());
            main.AdminPanel = AdminPanel.GetInstance(users, main.User, positions);

            //менеджер картинок
            SetHelloStatus("инициализация менеджера картинок...");
            main.ImageManager = ImageManager.GetInstance();

            //каталог
            SetHelloStatus("инициализация каталога...");
            SetNomGroups();
            nomenclatures.ToList().ForEach(n => n.SkipIsEdit());
            main.Catalog = Catalog.GetInstance(nomenclatures, categories, nomenclatureGroups);
            main.Catalog.Run();

            //банеры
            SetHelloStatus("инициализация банеров...");
            main.BannersManager = BannersManager.GetInstance();
            main.BannersManager.Banners = banners;
            main.BannersManager.Advertisings = advertisings;

            //конструктор
            SetHelloStatus("инициализация конструктора...");
            main.Constructor = Constructor.GetInstance();

            //менеджер документов
            main.DocManager = DocManager.GetInstance();

            //архив
            SetHelloStatus("инициализация архива...");
            SetOffers(offers, true);
            main.ArchiveStore = new OfferStore(offers, main.User);
            main.ArchiveStore.ApplyOfferFilter();

            //шаблоны
            SetHelloStatus("инициализация шаблонов...");
            SetOffers(offerTemplates, false);
            main.TemplatesStore = new OfferStore(offerTemplates, main.User);
            main.TemplatesStore.ApplyOfferFilter();
        }

        async private Task UploadImages()
        {
            List<string> guids = new List<string>();
            main.BannersManager.Banners.ToList().ForEach(b=>guids.Add(b.Guid));
            main.BannersManager.Advertisings.ToList().ForEach(a => guids.Add(a.Guid));
            main.Users.ToList().ForEach(u => guids.Add(u.Image.Guid));
            List<string> guids_ = ImageManager.GetInstance().GetExceptImages(guids);
            foreach (var guid in guids_)
            {
                SetHelloStatus(guid);
                await ImageManager.GetInstance().DownloadImage(guid);
            }
        }

        /// <summary>
        /// Восстанавливаем необходимые данные.
        /// </summary>
        /// <param name="data"></param>
        private void SetOffers(ObservableCollection<Offer> offers, bool isArchive)
        {
            ObservableCollection<Offer> restoredOffers = new ObservableCollection<Offer>();
            foreach (var offer in offers)
                restoredOffers.Add(Utils.RestoreOffer(offer, main.Users, isArchive));
            offers = restoredOffers;
        }

        /// <summary>
        /// Установка ссылок на объекты номенклатур в группы номенклатур.
        /// </summary>
        private void SetNomGroups()
        {
            for (int i = 0; i < nomenclatureGroups.Count; i++)
            {
                for (int j = 0; j < nomenclatureGroups[i].Nomenclatures.Count; j++)
                {
                    var nom = nomenclatureGroups[i].Nomenclatures[j];
                    if (nom.Id != 0)
                    {
                        nomenclatureGroups[i].Nomenclatures.Remove(nom);
                        var existNom = nomenclatures.Where(n => n.Id == nom.Id).FirstOrDefault();
                        if (existNom != null) nomenclatureGroups[i].Nomenclatures.Add(existNom);
                    }
                }
            }
        }

        #endregion Получение данных и создание модели
    }
}
