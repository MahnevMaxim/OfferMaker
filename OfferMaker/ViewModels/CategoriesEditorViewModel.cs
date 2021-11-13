using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class CategoriesEditorViewModel : BaseViewModel
    {
        CategoriesEditor modelEditor;

        public override void InitializeViewModel()
        {
            modelEditor = (CategoriesEditor)model;
        }

        public ObservableCollection<Category> CategoriesTree
        {
            get { return modelEditor.CategoriesTree; }
            set
            {
                modelEditor.CategoriesTree = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCat
        {
            get { return modelEditor.SelectedCat; }
            set
            {
                modelEditor.SelectedCat = value;
                OnPropertyChanged();
            }
        }

        public string NewCatName
        {
            get { return modelEditor.NewCatName; }
            set
            {
                modelEditor.NewCatName = value;
                OnPropertyChanged();
            }
        }

        public string SelectedParentName
        {
            get { return modelEditor.SelectedParentName; }
            set
            {
                modelEditor.SelectedParentName = value;
                OnPropertyChanged();
            }
        }
    }
}
