using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class CategoriesEditor : BaseModel
    {
        Category selectedCat;
        string newCatName;
        string selectedParentName;

        public ObservableCollection<Category> CategoriesTree { get; set; }

        public Category SelectedCat
        {
            get { return selectedCat; }
            set
            {
                selectedCat = value;
                SelectedParentName = selectedCat?.Title;
                OnPropertyChanged();
            }
        }

        public string NewCatName
        {
            get { return newCatName; }
            set
            {
                newCatName = value;
                OnPropertyChanged();
            }
        }

        public string SelectedParentName
        {
            get { return selectedParentName; }
            set
            {
                selectedParentName = value;
                OnPropertyChanged();
            }
        }

        public CategoriesEditor(ObservableCollection<Category> categoriesTree)
        {
            CategoriesTree = categoriesTree;
        }

        public void ResetParent() => SelectedCat = null;

        public void AddCategory()
        {
            if (SelectedCat?.Title == "Все") return;
            if (SelectedCat == null)
            {
                CategoriesTree.Add(new Category(NewCatName));
            }
            else
            {
                SelectedCat.Childs.Add(new Category(NewCatName));
            }
        }
    }
}
