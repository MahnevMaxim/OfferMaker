using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using System.Windows;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        Main modelMain;

        public override void InitializeViewModel()
        {
            modelMain = (Main)model;
        }

        public ObservableCollection<Currency> Currencies
        {
            get { return modelMain.Currencies; }
            set
            {
                modelMain.Currencies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get { return modelMain.Nomenclatures; }
            set
            {
                modelMain.Nomenclatures = value;
                OnPropertyChanged();
            }
        }
    }
}
