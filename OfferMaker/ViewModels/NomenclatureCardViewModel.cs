using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class NomenclatureCardViewModel : BaseViewModel
    {
        NomenclurueCard nomenclurueCard;

        public override void InitializeViewModel()
        {
            nomenclurueCard = (NomenclurueCard)model;
        }

        #region Properties

        public Nomenclature Nomenclature
        {
            get { return nomenclurueCard.Nomenclature; }
            set
            {
                nomenclurueCard.Nomenclature = value;
                OnPropertyChanged();
            }
        }

        public Image SelectedImage
        {
            get { return nomenclurueCard.SelectedImage; }
            set
            {
                nomenclurueCard.SelectedImage = value;
                Nomenclature.Image = value;
                OnPropertyChanged();
            }
        }

        public List<string> Currencies { get => nomenclurueCard.Currencies; }

        public string CurrencyCharCode
        {
            get => nomenclurueCard.CurrencyCharCode;
            set
            {
                nomenclurueCard.CurrencyCharCode = value;
                OnPropertyChanged();
            }
        }

        public string CategoryTitle { get => nomenclurueCard.CategoryTitle; }

        #endregion Properties
    }
}
