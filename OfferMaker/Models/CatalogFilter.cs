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
        private ObservableCollection<Nomenclature> nomenclaturesWithoutCat = new ObservableCollection<Nomenclature>();
        Category selectedCat;
        ICatalog catalog;
        List<Nomenclature> deletedNoms = new List<Nomenclature>();

        public int CategoryId { get; set; }

        public FilterMode FilterMode { get; set; }

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
                if (FilterMode == FilterMode.WithoutCat)
                    return new ObservableCollection<Nomenclature>(Nomenclatures.Where(n => n.CategoryGuid == null && !n.IsDelete).ToList());
                if (FilterMode == FilterMode.Category)
                    return selectedCat.Nomenclatures;
                return Nomenclatures;
            }
        }

        public CatalogFilter(ICatalog catalog)
        {
            this.catalog = catalog;
            Nomenclatures.ToList().ForEach(n => { if (n.CategoryGuid == null) nomenclaturesWithoutCat.Add(n); });
        }

        internal void Remove(Nomenclature nomenclature)
        {
            nomenclature.IsDelete=true;
            deletedNoms.Add(nomenclature);
            Nomenclatures.Remove(nomenclature);
            ((Catalog)catalog).RemoveNomFromCat(nomenclature);
        }

        public List<Nomenclature> GetDeletedNoms() => deletedNoms;

        internal void Clone(Nomenclature nomenclature)
        {
            Nomenclature newNom = Helpers.CloneObject<Nomenclature>(nomenclature);
            newNom.Id = 0;
            newNom.Guid = Guid.NewGuid().ToString();
            int index = Nomenclatures.IndexOf(nomenclature);
            Nomenclatures.Insert(index + 1, newNom);
            if (FilterMode == FilterMode.Category)
            {
                int indexInCat = SelectedCat.Nomenclatures.IndexOf(nomenclature);
                SelectedCat.Nomenclatures.Insert(indexInCat + 1, newNom);
            }
        }

        internal void SetMode(FilterMode mode)
        {
            FilterMode = mode;
            if (mode != FilterMode.Category)
            {
                selectedCat = null;
                OnPropertyChanged(nameof(SelectedCat));
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(FilteredNomenclatures));
        }

        /// <summary>
        /// Обработка события перетаскивания номенклатуры в категорию.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void RemoveDropedNom(Nomenclature nomenclature)
        {
            if (FilterMode == FilterMode.Category)
                SelectedCat.Nomenclatures.Remove(nomenclature);
            OnPropertyChanged(nameof(FilteredNomenclatures));
        }

        /// <summary>
        /// Удаление категории у номенклатуры.
        /// </summary>
        /// <param name="nomenclature"></param>
        internal void RemoveFromCategory(Nomenclature nomenclature)
        {
            Category cat = ((Catalog)catalog).GetFlattenTree().Where(c => c.Guid == nomenclature.CategoryGuid).FirstOrDefault();
            if(cat!=null)
            {
                cat.Nomenclatures.Remove(nomenclature);
                nomenclature.CategoryGuid = null;
            }
        }
    }
}
