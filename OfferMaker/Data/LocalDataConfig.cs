using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OfferMaker
{
    class LocalDataConfig
    {
        //папки
        public static string ServerCacheDataDir = "data_cache";
        public static string LocalDataDir = "local_data";
        public static readonly string ImageCacheDir = "cache";

        //имена файлов
        static string CurrenciesPath = "currencies.json";
        static string NomenclaturesPath = "nomenclatures.json";
        static string UsersPath = "users.json";
        static string CategoriesPath = "categories.json";
        static string NomenclatureGroupsPath = "nomenclature_groups.json";
        static string OffersPath = "offers.json";
        static string OffersHistoryPath = "offers_history.json";
        static string HintsPath = "hints.json";
        static string OfferTemplatesPath = "offer_templates.json";
        static string BannersPath = "banners.json";
        static string AdvertisingsPath = "advertisings.json";
        static string PositionsPath = "positions.json";
        static string ImageGuidsPath = "image_guids.json";

        //локальные пути локальных данных, созданных или изменённых локально, относительно директории приложения
        public static string LocalCurrenciesPath = Path.Combine(LocalDataDir, CurrenciesPath);
        public static string LocalNomenclaturesPath = Path.Combine(LocalDataDir, NomenclaturesPath);
        public static string LocalUsersPath = Path.Combine(LocalDataDir, UsersPath);
        public static string LocalCategoriesPath = Path.Combine(LocalDataDir, CategoriesPath);
        public static string LocalNomenclatureGroupsPath = Path.Combine(LocalDataDir, NomenclatureGroupsPath);
        public static string LocalOffersPath = Path.Combine(LocalDataDir, OffersPath);
        public static string LocalOffersHistoryPath = Path.Combine(LocalDataDir, OffersHistoryPath);
        public static string LocalHintsPath = Path.Combine(LocalDataDir, HintsPath);
        public static string LocalOfferTemplatesPath = Path.Combine(LocalDataDir, OfferTemplatesPath);
        public static string LocalBannersPath = Path.Combine(LocalDataDir, BannersPath);
        public static string LocalAdvertisingsPath = Path.Combine(LocalDataDir, AdvertisingsPath);
        public static string LocalPositionsPath = Path.Combine(LocalDataDir, PositionsPath);
        public static string LocalImageGuidsPath = Path.Combine(LocalDataDir, ImageGuidsPath);

        //локальные пути кэшированных данных с сервера относительно директории приложения
        public static string ServerCacheCurrenciesPath = Path.Combine(ServerCacheDataDir, CurrenciesPath);
        public static string ServerCacheNomenclaturesPath = Path.Combine(ServerCacheDataDir, NomenclaturesPath);
        public static string ServerCacheUsersPath = Path.Combine(ServerCacheDataDir, UsersPath);
        public static string ServerCacheCategoriesPath = Path.Combine(ServerCacheDataDir, CategoriesPath);
        public static string ServerCacheNomenclatureGroupsPath = Path.Combine(ServerCacheDataDir, NomenclatureGroupsPath);
        public static string ServerCacheOffersPath = Path.Combine(ServerCacheDataDir, OffersPath);
        public static string ServerCacheOffersHistoryPath = Path.Combine(ServerCacheDataDir, OffersHistoryPath);
        public static string ServerCacheHintsPath = Path.Combine(ServerCacheDataDir, HintsPath);
        public static string ServerCacheOfferTemplatesPath = Path.Combine(ServerCacheDataDir, OfferTemplatesPath);
        public static string ServerCacheBannersPath = Path.Combine(ServerCacheDataDir, BannersPath);
        public static string ServerCacheAdvertisingsPath = Path.Combine(ServerCacheDataDir, AdvertisingsPath);
        public static string ServerCachePositionsPath = Path.Combine(ServerCacheDataDir, PositionsPath);
        public static string ServerCacheImageGuidsPath = Path.Combine(ServerCacheDataDir, ImageGuidsPath);
    }
}
