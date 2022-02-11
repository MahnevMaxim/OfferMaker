using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace OfferMaker.ViewModels
{
    public class PdfGalleryViewModel : BaseViewModel
    {
        PdfGallery pdfGalleryModel;

        public override void InitializeViewModel()
        {
            pdfGalleryModel = (PdfGallery)model;
        }

        public ObservableCollection<PdfPageWrapper> PdfPages { get => pdfGalleryModel.PdfPages; }

        public bool IsBusy { get => pdfGalleryModel.IsBusy; }
    }
}
