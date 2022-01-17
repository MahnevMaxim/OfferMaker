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
        public static string DataCacheDir = "data_cache";
        public static string CurrenciesPath { get => Path.Combine(DataCacheDir, "currencies.json"); }
        public static string NomenclaturesPath { get => Path.Combine(DataCacheDir, "nomenclatures.json"); } 
        public static string UsersPath { get => Path.Combine(DataCacheDir, "users.json"); } 
        public static string CategoriesPath { get => Path.Combine(DataCacheDir, "categories.json"); } 
        public static string NomenclatureGroupsPath { get => Path.Combine(DataCacheDir, "nomenclature_groups.json"); } 
        public static string OffersPath { get => Path.Combine(DataCacheDir, "offers.json"); } 
        public static string HintsPath { get => Path.Combine(DataCacheDir, "hints.json"); }
        public static string OfferTemplates { get => Path.Combine(DataCacheDir, "offer_templates.json"); }
    }
}
