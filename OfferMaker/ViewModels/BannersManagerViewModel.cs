using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GongSolutions.Wpf.DragDrop;

namespace OfferMaker.ViewModels
{
    public class BannersManagerViewModel : BaseViewModel
    {
        BannersManager bannersManager;

        public override void InitializeViewModel()
        {
            bannersManager = (BannersManager)model;
        }

        public ObservableCollection<Banner> Banners { get => bannersManager.Banners; }

        public ObservableCollection<Advertising> Advertisings { get => bannersManager.Advertisings; }

        public ObservableCollection<IImage> AdvertisingsUp { get => bannersManager.AdvertisingsUp; }

        public ObservableCollection<IImage> AdvertisingsDown { get => bannersManager.AdvertisingsDown; }
    }
}
