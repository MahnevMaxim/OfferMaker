using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace OfferMaker
{
    public class BannersManager : BaseModel
    {
        /// <summary>
        /// Пока не прочувствовал, чем банер отличается от простой картинки, возможно в будущем придётся во
        /// что-нибудь обернуть, пока выражаю просто через путь к картинке.
        /// </summary>
        public ObservableCollection<string> Banners { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Рекламные материалы.
        /// </summary>
        public ObservableCollection<string> Advertisings { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Рекламные материалы вверху.
        /// </summary>
        public ObservableCollection<string> AdvertisingsUp = new ObservableCollection<string>();

        /// <summary>
        /// Рекламные материалы внизу.
        /// </summary>
        public ObservableCollection<string> AdvertisingsDown = new ObservableCollection<string>();

        /// <summary>
        /// Выбранный баннер.
        /// </summary>
        public string SelectedBanner { get; set; }

        #region Singleton

        private BannersManager() { }

        private static readonly BannersManager instance = new BannersManager();

        public static BannersManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Срабатывает при двойном клике по баннеру. Выбирает баннер, сохраняет его и закрывает окно.
        /// </summary>
        /// <param name="path"></param>
        public void SelectBanner(string path)
        {
            SelectedBanner = path;
            Settings.SetDefaultBanner(path);
            Close();
        }
    }
}
