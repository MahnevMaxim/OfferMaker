using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public enum FilterMode { All, WithoutCat, Category }

    /// <summary>
    /// Номенклатуры в каталоге представлены в виде отфильтрованной коллекции nomenclatures - это вся номенклатура.
    /// CatalogFilter предназначен для отображения коллекции и согласованной работы отфильтрованной и не отфильтрованной коллекций.
    /// </summary>
    public class CatalogFilter : BaseEntity
    {
        private ObservableCollection<Nomenclature> Nomenclatures { get => catalog.Nomenclatures; }
        private ObservableCollection<Nomenclature> nomenclaturesWithoutCat=new ObservableCollection<Nomenclature>();
        string keyString;
        Category selectedCat;
        Catalog catalog;

        public int CategoryId { get; set; }

        public FilterMode FilterMode { get; set; }

        public string KeyString
        {
            get => keyString;
            set
            {
                keyString = value;
            }
        }

        public Category SelectedCat
        {
            get { return selectedCat; }
            set
            {
                selectedCat = value;
                SetMode(FilterMode.Category);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Nomenclature> FilteredNomenclatures 
        {
            get 
            {
                if(FilterMode==FilterMode.WithoutCat)
                    return new ObservableCollection<Nomenclature>(Nomenclatures.Where(n=>n.CategoryGuid==null).ToList());
                if (FilterMode == FilterMode.Category)
                    return selectedCat.Nomenclatures;
                return Nomenclatures;
            }  
        }

        public CatalogFilter(Catalog catalog)
        {
            this.catalog=catalog;
            Nomenclatures.ToList().ForEach(n => { if(n.CategoryGuid == null) nomenclaturesWithoutCat.Add(n); });
        }
        
        internal void Remove(Nomenclature nomenclature)
        {
            Nomenclatures.Remove(nomenclature);
            OnPropertyChanged(nameof(FilteredNomenclatures));
        }

        internal void Clone(Nomenclature nomenclature)
        {
            Nomenclature newNom = Helpers.CloneObject<Nomenclature>(nomenclature);
            newNom.Id = 0;
            int index = Nomenclatures.IndexOf(nomenclature);
            Nomenclatures.Insert(index+1,newNom);
        }

        internal void SetMode(FilterMode mode)
        {
            FilterMode = mode;
            if(mode!=FilterMode.Category)
            {
                selectedCat = null;
                OnPropertyChanged(nameof(SelectedCat));
            }
                
            OnPropertyChanged();
            OnPropertyChanged(nameof(FilteredNomenclatures));
        }

        internal void RemoveDropedNom(Nomenclature nomenclature)
        {
            if(FilterMode==FilterMode.Category)
                SelectedCat.Nomenclatures.Remove(nomenclature);
            OnPropertyChanged(nameof(FilteredNomenclatures));
        }
    }
}
