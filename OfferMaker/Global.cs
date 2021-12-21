using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    class Global
    {
        static bool IsRealMode;
        public static string apiEndpoint { get => IsRealMode ? "https://kip.mybot.su/" : "https://localhost:44333/"; }

        public static Main Main;

        public static List<Currency> Currencies { get => Main.Currencies.ToList(); }

        internal static Currency GetCurrencyByCode(string currencyCharCode) => Currencies.Where(c => c.CharCode == currencyCharCode).FirstOrDefault();

        public static Catalog Catalog { get => Main.Catalog; }

        public static Settings Settings { get => Main.Settings; }

        public static User User { get => Main.User; }

        public static ImageManager ImageManager { get => Main.ImageManager; }
    }
}
