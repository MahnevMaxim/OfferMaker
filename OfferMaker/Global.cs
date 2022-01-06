using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    class Global
    {
        /// <summary>
        /// Если true - Тогда подключение идёт удалённому серверу.
        /// </summary>
        //static bool IsRealMode = true;
        static bool IsRealMode;

        public static string apiEndpoint { get => IsRealMode ? "https://kip.mybot.su/" : "https://localhost:44313/"; }

        public static Main Main;

        public static List<Currency> Currencies { get => Main.Currencies.ToList(); }

        internal static Currency GetCurrencyByCode(string currencyCharCode) => Currencies.Where(c => c.CharCode == currencyCharCode).FirstOrDefault();

        public static Catalog Catalog { get => Main.Catalog; }

        public static Settings Settings { get => Main.Settings; }

        public static User User { get => Main.User; }

        public static ObservableCollection<User> Users { get => Main.Users; }

        public static ImageManager ImageManager { get => Main.ImageManager; }

        public static Offer Offer { get => Main.Constructor.Offer; }

        public static Constructor Constructor { get => Main.Constructor; }

        public static string NoProfilePicturePath { get => AppDomain.CurrentDomain.BaseDirectory + "Images\\no-profile-picture.png"; }

        public static Image NoProfileImage = new Image() { LocalPhotoPath = NoProfilePicturePath, IsNew = false };

    }
}
