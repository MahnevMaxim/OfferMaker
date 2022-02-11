using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using PDFtoImage;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;

namespace OfferMaker
{
    public class PdfGallery : BaseModel
    {
        private string pdfPath;
        bool isBusy;

        /// <summary>
        /// Выбранные для добавления документы.
        /// </summary>
        public List<PdfPageWrapper> ResultPages { get; set; } = new List<PdfPageWrapper>();

        /// <summary>
        /// Коллекция обёрток скриншотоф документов.
        /// </summary>
        public ObservableCollection<PdfPageWrapper> PdfPages { get; set; } = new ObservableCollection<PdfPageWrapper>();

        /// <summary>
        /// Это отображаемая страница.
        /// </summary>
        public PdfPageWrapper SelectedPage { get; set; }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PdfPageWrapper> PdfPagesTemp { get; set; } = new ObservableCollection<PdfPageWrapper>();

        public PdfGallery(string pdfPath)
        {
            this.pdfPath = pdfPath;
        }

        async internal override void Run()
        {
            if (PdfPages.Count > 0) return;
            IsBusy = true;
            try
            {
                using var inputStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
                List<System.Drawing.Image> listImages = new List<System.Drawing.Image>();
                await Task.Run(() => listImages = Conversion.ToImages(inputStream).ToList());
                Thread thread = new Thread(new ParameterizedThreadStart(CreateImages));
                thread.Start(listImages);
            }
            catch(Exception ex)
            {
                Log.Write(ex);
                IsBusy = false;
            }
        }

        private void CreateImages(object? listImages_)
        {
            List<System.Drawing.Image> listImages = (List<System.Drawing.Image>)listImages_;
            foreach (var image in listImages)
            {
                var image_ = ToImageSource(image, ImageFormat.Png);
                App.Current.Dispatcher.Invoke(() => PdfPages.Add(new PdfPageWrapper() { PageImage = image_ }));
            }
            IsBusy = false;
        }

        public static ImageSource ToImageSource(System.Drawing.Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, imageFormat);
                stream.Seek(0, SeekOrigin.Begin);
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }

        public void AddAdvertisings()
        {
            ResultPages = PdfPages.Where(p => p.IsChecked).ToList();
            Close();
        }
    }
}
