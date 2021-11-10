using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Main : BaseModel
    {
        #region MVVVM 

        #region Fields

        ObservableCollection<Currency> currencies;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<User> users;
        ObservableCollection<Category> categories;

        #endregion Fields

        #region Propetries

        public ObservableCollection<Currency> Currencies
        {
            get { return currencies; }
            set
            {
                currencies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Nomenclature> Nomenclatures
        {
            get { return nomenclatures; }
            set
            {
                nomenclatures = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }

        #endregion Propetries

        #endregion MVVVM 

        DataRepository DataRepository { get; set; }

        async internal override void Run()
        {
            DataRepository = new DataRepository();
            Currencies = await DataRepository.GetCurrencies();
            Nomenclatures = await DataRepository.GetNomenclatures();
            Categories = await DataRepository.GetCategories();
        }

        public void EditCategories()
        {
            CategoriesEditor editor = new CategoriesEditor();
            MvvmFactory.CreateWindow(editor, new ViewModels.CategoriesEditorViewModel(), new Views.CategoriesEditor());
        }
    }
}
