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

        #region Catalog

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get { return modelMain.Catalog.Nomenclatures; }
            set
            {
                modelMain.Catalog.Nomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> CategoriesTree
        {
            get { return modelMain.Catalog.CategoriesTree; }
            set
            {
                modelMain.Catalog.CategoriesTree = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCat
        {
            get { return modelMain.Catalog.SelectedCat; }
            set
            {
                modelMain.Catalog.SelectedCat = value;
                OnPropertyChanged();
            }
        }

        #endregion Catalog

        #region Settings

        public Settings Settings { get => modelMain.Settings; }

        #endregion Settings

        #region Main

        public ObservableCollection<Currency> Currencies
        {
            get { return modelMain.Currencies; }
            set
            {
                modelMain.Currencies = value;
                OnPropertyChanged();
            }
        }

        #endregion Main
    }
}
