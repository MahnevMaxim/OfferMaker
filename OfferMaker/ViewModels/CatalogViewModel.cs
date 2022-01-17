using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.ViewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        Catalog modelCatalog;
        private IDialogCoordinator dialogCoordinator;

        public override void InitializeViewModel()
        {
            modelCatalog = (Catalog)model;
            dialogCoordinator = ((Views.Catalog)view).dialogCoordinator;
            NomToCatDropHandler = new NomToCatDropHandler();
            NomToCatDragHandler = new NomToCatDragHandler();
            NomToNomListOfGroupDropHandler = new NomToNomListOfGroupDropHandler();
        }

        public RelayCommand NomenclatureDeleteCommand
        {
            get => new RelayCommand(obj =>
            {
                NomenclatureDelete((Nomenclature)obj);
            });
        }

        async void NomenclatureDelete(Nomenclature nomenclature)
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Удалить",
                NegativeButtonText = "Отмена"
            };
            var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "Удаление номенклатуры", "Вы точно уверены, что хотите удалить номенклатуру?",
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            if (dialogRes == MessageDialogResult.Affirmative)
                modelCatalog.DeleteNomenclature(nomenclature);
        }

        public RelayCommand DeleteNomenclaturesCommand
        {
            get => new RelayCommand(obj =>
            {
                DeleteNomenclatures();
            });
        }

        async void DeleteNomenclatures()
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Удалить",
                NegativeButtonText = "Отмена"
            };
            var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "Удаление номенклатур", "Вы точно уверены, что хотите удалить номенклатуры?",
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            if (dialogRes == MessageDialogResult.Affirmative)
                modelCatalog.DeleteNomenclatures();
        }

        public NomToCatDropHandler NomToCatDropHandler { get; set; }

        public NomToCatDragHandler NomToCatDragHandler { get; set; }

        public NomToNomListOfGroupDropHandler NomToNomListOfGroupDropHandler { get; set; }

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get => modelCatalog.FilteredNomenclatures;
            set
            {
                modelCatalog.FilteredNomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NomenclatureGroup> NomenclatureGroups
        {
            get => modelCatalog.NomenclatureGroups;
            set
            {
                modelCatalog.NomenclatureGroups = value;
                OnPropertyChanged();
            }
        }

        public NomenclatureGroup SelectedNomenclatureGroup
        {
            get => modelCatalog.SelectedNomenclatureGroup;
            set
            {
                modelCatalog.SelectedNomenclatureGroup = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> CategoriesTree
        {
            get => modelCatalog.CategoriesTree;
            set
            {
                modelCatalog.CategoriesTree = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCat
        {
            get => modelCatalog.CatalogFilter.SelectedCat;
            set
            {
                modelCatalog.CatalogFilter.SelectedCat = value;
                OnPropertyChanged();
            }
        }

        public CatalogFilter CatalogFilter
        {
            get => modelCatalog.CatalogFilter;
            set
            {
                modelCatalog.CatalogFilter = value;
                OnPropertyChanged();
            }
        }

        public string SearchStringInCatalog { set => modelCatalog.SearchStringInCatalog = value; }

        public IList SelectedNomenclatures { set =>  modelCatalog.SelectedNomenclatures = value; }
    }
}
