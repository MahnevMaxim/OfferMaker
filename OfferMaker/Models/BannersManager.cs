using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using System.Windows.Media.Imaging;

namespace OfferMaker
{
    public class BannersManager : BaseModel
    {
        /// <summary>
        /// Пока не прочувствовал, чем банер отличается от простой картинки, возможно в будущем придётся во
        /// что-нибудь обернуть, пока выражаю просто через путь к картинке.
        /// </summary>
        //public ObservableCollection<string> Banners { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Banner> Banners { get; set; } = new ObservableCollection<Banner>();

        /// <summary>
        /// Рекламные материалы.
        /// </summary>
        public ObservableCollection<Advertising> Advertisings { get; set; } = new ObservableCollection<Advertising>();

        /// <summary>
        /// Рекламные материалы вверху.
        /// </summary>
        public ObservableCollection<IImage> AdvertisingsUp = new ObservableCollection<IImage>();

        /// <summary>
        /// Рекламные материалы внизу.
        /// </summary>
        public ObservableCollection<IImage> AdvertisingsDown = new ObservableCollection<IImage>();

        /// <summary>
        /// Выбранный баннер.
        /// </summary>
        public Banner SelectedBanner { get; set; }

        #region Singleton

        private BannersManager() { }

        private static readonly BannersManager instance = new BannersManager();

        public static BannersManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Срабатывает при двойном клике по баннеру. Выбирает баннер, сохраняет его и закрывает окно.
        /// </summary>
        /// <param name="path"></param>
        public void SelectBanner(Banner banner)
        {
            SelectedBanner = banner;
            Settings.SetDefaultBannerGuid(banner.Guid);
            Close();
        }

        /// <summary>
        /// Удаление баннера.
        /// </summary>
        /// <param name="banner"></param>
        async public void RemoveBanner(Banner banner)
        {
            CallResult cr = await Global.Main.DataRepository.BannerDelete(banner);
            if (cr.Success)
            {
                Banners.Remove(banner);
            }
            else
            {
                if (cr.Error.Code == 404)
                    Banners.Remove(banner);
                OnSendMessage(cr.Message);
            }
        }

        /// <summary>
        /// Добавление баннера.
        /// </summary>
        async public void AddBanner()
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
            {
                Banner banner = new Banner(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                Global.ImageManager.Add(banner, null);

                CallResult cr = await Global.Main.DataRepository.BannerCreate(banner);
                if (cr.Success)
                {
                    Banners.Add(banner);
                }
                else
                    OnSendMessage(cr.Message);
            }
        }

        /// <summary>
        /// Добавление ракламного материала.
        /// </summary>
        async public void AddAdvertising()
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
                await AddAdvertisingFromPath(path);
        }

        async public Task AddAdvertisingFromPath(string path)
        {
            Advertising advertising = new Advertising(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
            Global.ImageManager.Add(advertising, null);

            CallResult cr = await Global.Main.DataRepository.AdvertisingCreate(advertising);
            if (cr.Success)
            {
                Advertisings.Add(advertising);
            }
            else
                OnSendMessage(cr.Message);
        }

        async public void AddAdvertisingFromPdf()
        {
            string pdfPath = Helpers.GetFilePath("Pdf file (*.pdf) | *.pdf");
            if (pdfPath != null)
            {
                PdfGallery pg = new PdfGallery(pdfPath);
                pg.Run();
                MvvmFactory.CreateWindow(pg, new ViewModels.PdfGalleryViewModel(), new Views.PdfGallery(), ViewMode.ShowDialog);
                if (pg.ResultPages.Count > 0)
                {
                    foreach(var page in pg.ResultPages)
                    {
                        Advertising advertising = new Advertising(Guid.NewGuid().ToString(), Global.User.Id, null) { IsNew = true, Bitmap = (BitmapImage)page.PageImage };
                        Global.ImageManager.Add(advertising, null);

                        CallResult cr = await Global.Main.DataRepository.AdvertisingCreate(advertising);
                        if (cr.Success)
                        {
                            Advertisings.Add(advertising);
                        }
                        else
                            OnSendMessage(cr.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Удаление рекламного материала.
        /// </summary>
        /// <param name="advertising"></param>
        async public void RemoveAdvertising(Advertising advertising)
        {
            CallResult cr = await Global.Main.DataRepository.AdvertisingDelete(advertising.Id);
            if (cr.Success)
            {
                Advertisings.Remove(advertising);
            }
            else
            {
                if (cr.Error.Code == 404)
                    Advertisings.Remove(advertising);
                OnSendMessage(cr.Message);
            }
        }

        /// <summary>
        /// Удаление ракламного материала из конструктора.
        /// </summary>
        /// <param name="advertising"></param>
        public void RemoveAdvertisingDown(IImage advertising) => AdvertisingsDown.Remove(advertising);

        /// <summary>
        /// Удаление ракламного материала из конструктора.
        /// </summary>
        /// <param name="advertising"></param>
        public void RemoveAdvertisingUp(IImage advertising) => AdvertisingsUp.Remove(advertising);
    }
}
