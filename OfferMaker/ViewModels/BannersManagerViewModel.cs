using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class BannersManagerViewModel : BaseViewModel
    {
        BannersManager bannersManager;

        public override void InitializeViewModel()
        {
            bannersManager = (BannersManager)model;
        }

        public ObservableCollection<string> Banners { get => bannersManager.Banners; }

        public ObservableCollection<string> Advertisings { get => bannersManager.Advertisings; }
    }
}
