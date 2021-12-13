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

namespace OfferMaker
{
    class EntryPoint
    {
        Main main;
        DataRepository dataRepository;
        User user;
        ObservableCollection<User> managers = new ObservableCollection<User>();
        ObservableCollection<User> users;

        ObservableCollection<Category> categories;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups;
        ObservableCollection<Offer> offers;
        ObservableCollection<Currency> currencies;

        async internal void Run()
        {
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(L.AppDomain_CurrentDomain_UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(L.Application_ThreadException);

            //настройки нужны до авторизации
            main = new Main();
            Global.Main = main;
            main.Settings = Settings.GetInstance();
            main.Settings.SetSettings();

            SimpleViews.Hello form = new SimpleViews.Hello();
            var res = form.ShowDialog() ?? false;
            if (res)
            {
                user = SimpleViews.Hello.User;
                users = SimpleViews.Hello.Users;
            }
            else
            {
                return;
            }
            await Init();
            MvvmFactory.CreateWindow(main, new ViewModels.MainViewModel(), new Views.MainWindow(), ViewMode.Show);
        }

        async private Task Init()
        {
            //main = new Main();
            //Global.Main = main;
            //main.Settings = Settings.GetInstance();
            //main.Settings.SetSettings();
            main.DataRepository = DataRepository.GetInstance(); //инициализация хранилища
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
            var offersCr = await dataRepository.GetOffers();
            if (offersCr.Success)
                offers = offersCr.Data;
            else
                errorMessage += offersCr.Error.Message + "\n";

            //получаем хинты
            //var hintsCr = await DataRepository.GetHints();
            //if (hintsCr.Success)
            //{
            //    Hints = hintsCr.Data;
            //}
            //else
            //    errorMessage += hintsCr.Error.Message + "\n";

            if (!string.IsNullOrWhiteSpace(errorMessage))
                MessageBox.Show("", errorMessage);
        }

        private void InitModules()
        {
            //main
            main.Currencies = currencies;
            main.User = user;
            main.Users = users;
            users.ToList().ForEach(u => main.Managers.Add(u));

            //каталог
            SetNomGroups();
            main.Catalog = Catalog.GetInstance(nomenclatures, categories, nomenclatureGroups);
            main.Catalog.Run();

            //банеры
            main.BannersManager = BannersManager.GetInstance();

            //менеджер документов
            main.DocManager = DocManager.GetInstance();

            //конструктор
            main.Constructor = Constructor.GetInstance();

            //баннеры
            InitBanners();
            InitAdvertising();

            //архив
            SetOffers(offers);
            main.ArchiveFilter = new ArchiveFilter(offers, user);
            main.ArchiveOffers = main.ArchiveFilter.GetFilteredOffers();
            main.offers = offers;
        }

        /// <summary>
        /// Восстанавливаем необходимые данные.
        /// </summary>
        /// <param name="data"></param>
        private void SetOffers(ObservableCollection<Offer> offers)
        {
            ObservableCollection<Offer> restoredOffers = new ObservableCollection<Offer>();
            foreach (var offer in offers)
                restoredOffers.Add(RestoreOffer(offer));
            offers = restoredOffers;
        }

        /// <summary>
        /// Восстанавливаем данные и взаимосвязи класса Offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private Offer RestoreOffer(Offer offer)
        {
            try
            {
                offer.OfferCreator = users.Where(u => u.Id == offer.OfferCreatorId).FirstOrDefault();
                offer.Manager = users.Where(u => u.Id == offer.ManagerId).FirstOrDefault();
                offer.OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(n => n.SetOfferGroup(g)));
                offer.OfferGroups.ToList().ForEach(g => g.SetOffer(offer));
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
            return offer;
        }

        /// <summary>
        /// Инициализация рекламных материалов.
        /// </summary>
        private void InitAdvertising()
        {
            string advertisingsPath = AppDomain.CurrentDomain.BaseDirectory + "\\advertisings";
            var files = Directory.GetFiles(advertisingsPath);
            files.ToList().ForEach(f => main.BannersManager.Advertisings.Add(f));
        }

        /// <summary>
        /// Инициализация баннеров.
        /// </summary>
        private void InitBanners()
        {
            string bannersPath = AppDomain.CurrentDomain.BaseDirectory + "\\banners";
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
