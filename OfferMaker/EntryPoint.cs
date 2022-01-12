using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Shared;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using ApiLib;
using System.Text.Json;
using Newtonsoft.Json;

namespace OfferMaker
{
    class EntryPoint
    {
        readonly string apiEndpoint = Global.apiEndpoint;

        User user;
        Main main;
        DataRepository dataRepository;
        ObservableCollection<User> users;
        ObservableCollection<Position> positions;
        ObservableCollection<Category> categories;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups;
        ObservableCollection<Offer> offers;
        ObservableCollection<Currency> currencies;
        List<Hint> hints;

        async internal void Run()
        {
            //настройка для логирования необработанных исключений
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(Log.AppDomain_CurrentDomain_UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Log.Application_ThreadException);

            //настройки нужны до авторизации
            main = new Main();
            Global.Main = main;
            main.Settings = Settings.GetInstance();
            main.Settings.SetSettings();

            //Если есть токен, то  пытаемся войти по нему.
            //Если авторизация удастся, то токен обновится и обновятся права.
            if (Settings.GetIsRememberMe())
            {
                CallResult<User> userCr = await Auth(Settings.GetToken());
                if (userCr.Success)
                {
                    user = userCr.Data;
                    Settings.SetToken(user.Account.Token);
                }
            }

            //Если авторизация по токену не удалась, то открываем окно авторизации.
            if (user == null)
            {
                SimpleViews.Hello form = new SimpleViews.Hello();
                if (Settings.GetIsRememberMe())
                    form.loginTextBox.Text = Settings.GetLogin();

                var res = form.ShowDialog() ?? false;
                if (res)
                {
                    Settings.SetIsRememberMe(SimpleViews.Hello.IsRememberMe);
                    Settings.SetToken(SimpleViews.Hello.AccessToken);
                    Settings.SetLogin(SimpleViews.Hello.Login.Trim());
                    user = SimpleViews.Hello.User;
                }
                else
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            await Init();
            MvvmFactory.CreateWindow(main, new ViewModels.MainViewModel(), new Views.MainWindow(), ViewMode.Show);
        }

        /// <summary>
        /// Авторизация по токену.
        /// </summary>
        /// <returns></returns>
        async private Task<CallResult<User>> Auth(string token)
        {
            try
            {
                var res = await new Client(apiEndpoint, new System.Net.Http.HttpClient()).AccountUpdateTokenAsync(token);
                JsonElement authRes = (JsonElement)res.Result;
                string accessToken = authRes.GetProperty("access_token").GetString();
                var userString = authRes.GetProperty("user").ToString();
                User user = JsonConvert.DeserializeObject<User>(userString);
                return new CallResult<User>() { Data=user};
            }
            catch (ApiException ex)
            {
                return new CallResult<User>() { Error = new Error(ex.Response.ToString()) };
            }
            catch (Exception ex)
            {
                return new CallResult<User>() { Error = new Error(ex) };
            }
        }

        /// <summary>
        /// Инициализация модели.
        /// </summary>
        /// <returns></returns>
        async private Task Init()
        {
            main.DataRepository = DataRepository.GetInstance(Settings.GetToken()); //инициализация хранилища
            dataRepository = main.DataRepository;
            await ReciveData(); //инициализация данных
            InitModules(); //инициализация модулей на основе данных
        }

        #region Build Main

        /// <summary>
        /// Получение данных с сервера / из кэша.
        /// </summary>
        /// <returns></returns>
        async private Task ReciveData()
        {
            string errorMessage = "";

            //получаем должности
            var positionsCr = await dataRepository.PositionsGet();
            if (positionsCr.Success)
                positions = positionsCr.Data;
            else
                errorMessage += positionsCr.Error.Message + "\n";

            //получаем пользователей
            if (user.Position.Permissions.Contains(Shared.Permissions.CanAll)
                || user.Position.Permissions.Contains(Shared.Permissions.CanControlUsers)
                || user.Position.Permissions.Contains(Shared.Permissions.CanUseManager)
                || user.Position.Permissions.Contains(Shared.Permissions.CanSeeAllOffers))
            {
                var usersCr = await dataRepository.UsersGet();
                if (usersCr.Success)
                    users = usersCr.Data;
                else
                    errorMessage += usersCr.Error.Message + "\n";
            }


            //получаем валюты
            var currenciesCr = await dataRepository.GetCurrencies();
            if (currenciesCr.Success)
                currencies = currenciesCr.Data;
            else
                errorMessage += currenciesCr.Error.Message + "\n";

            //получаем категории
            var categoriesCr = await dataRepository.GetCategories();
            if (categoriesCr.Success)
                categories = categoriesCr.Data;
            else
                errorMessage += categoriesCr.Error.Message + "\n";

            //получаем номенклатуры
            var nomenclaturesCr = await dataRepository.GetNomenclatures();
            if (nomenclaturesCr.Success)
                nomenclatures = nomenclaturesCr.Data;
            else
                errorMessage += nomenclaturesCr.Error.Message + "\n";

            //получаем группы номенклатур
            var nomGroupsCr = await dataRepository.GetNomGroups();
            if (nomGroupsCr.Success)
                nomenclatureGroups = nomGroupsCr.Data;
            else
                errorMessage += nomGroupsCr.Error.Message + "\n";

            //получаем архив КП
            if (user.Position.Permissions.Contains(Shared.Permissions.CanAll)
                || user.Position.Permissions.Contains(Shared.Permissions.CanSeeAllOffers))
            {
                var offersCr = await dataRepository.OffersGet();
                if (offersCr.Success)
                    offers = offersCr.Data;
                else
                    errorMessage += offersCr.Error.Message + "\n";
            }
            else
            {
                var offersCr = await dataRepository.OffersSelfGet();
                if (offersCr.Success)
                    offers = offersCr.Data;
                else
                    errorMessage += offersCr.Error.Message + "\n";
            }

            //получаем хинты
            var hintsCr = await dataRepository.HintsGet();
            if (hintsCr.Success)
            {
                hints = hintsCr.Data;
            }
            else
                errorMessage += hintsCr.Error.Message + "\n";

            if (!string.IsNullOrWhiteSpace(errorMessage))
                MessageBox.Show("", errorMessage);
        }

        /// <summary>
        /// Инициализация основных моделей приложения.
        /// </summary>
        private void InitModules()
        {
            //main
            main.Currencies = currencies;
            main.User = users == null ? user : users.Where(u=>u.Id==user.Id).FirstOrDefault();
            main.Users = users == null ? new ObservableCollection<User>() { user } : users;
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

            //каталог
            SetNomGroups();
            main.Catalog = Catalog.GetInstance(nomenclatures, categories, nomenclatureGroups);
            main.Catalog.Run();

            //банеры
            main.BannersManager = BannersManager.GetInstance();

            //конструктор
            main.Constructor = Constructor.GetInstance();

            //менеджер документов
            main.DocManager = DocManager.GetInstance();

            //баннеры
            InitBanners();
            InitAdvertising();

            //архив
            SetOffers(offers);
            main.ArchiveFilter = new ArchiveFilter(offers, main.User);
            main.ArchiveOffers = main.ArchiveFilter.GetFilteredOffers();
            main.offers = offers;

            //менеджер картинок
            main.ImageManager = ImageManager.GetInstance();
        }

        /// <summary>
        /// Восстанавливаем необходимые данные.
        /// </summary>
        /// <param name="data"></param>
        private void SetOffers(ObservableCollection<Offer> offers)
        {
            ObservableCollection<Offer> restoredOffers = new ObservableCollection<Offer>();
            foreach (var offer in offers)
                restoredOffers.Add(Utils.RestoreOffer(offer, users));
            offers = restoredOffers;
        }

        /// <summary>
        /// Инициализация рекламных материалов.
        /// </summary>
        private void InitAdvertising()
        {
            string advertisingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\advertisings");
            var files = Directory.GetFiles(advertisingsPath);
            files.ToList().ForEach(f => main.BannersManager.Advertisings.Add(f));
        }

        /// <summary>
        /// Инициализация баннеров.
        /// </summary>
        private void InitBanners()
        {
            string bannersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\banners");
            var files = Directory.GetFiles(bannersPath);
            files.ToList().ForEach(f => main.BannersManager.Banners.Add(f));
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

        #endregion Build Main
    }
}
