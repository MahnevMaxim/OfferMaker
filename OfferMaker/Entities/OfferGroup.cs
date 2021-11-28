using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class OfferGroup : BaseEntity
    {
        Offer offer;
        ObservableCollection<NomWrapper> nomWrappers = new ObservableCollection<NomWrapper>();
        string groupTitle;
        bool isOption;

        public int Id { get; set; }

        /// <summary>
        /// Название группы.
        /// </summary>
        public string GroupTitle 
        {
            get => groupTitle;
            set
            {
                groupTitle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Это опция.
        /// </summary>
        public bool IsOption
        {
            get => isOption;
            set
            {
                isOption = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Колекция номенклатуры.
        /// </summary>
        public ObservableCollection<NomWrapper> NomWrappers 
        {
            get => nomWrappers;
            set => nomWrappers = value;
        }

        /// <summary>
        /// Содержит ли текущая группа номенклатуру.
        /// </summary>
        public bool IsContainsNoms { get => NomWrappers.Count == 0 ? false : true; }

        /// <summary>
        /// Суммарная себестоимость группы КП.
        /// </summary>
        public decimal CostPriceSum
        {
            get
            {
                decimal sum = 0;
                NomWrappers.ToList().ForEach(n => sum += n.CostSum);
                return sum;
            }
        }

        /// <summary>
        /// Суммарная цена группы КП.
        /// </summary>
        public decimal PriceSum
        {
            get
            {
                decimal sum = 0;
                NomWrappers.ToList().ForEach(n => sum += n.Sum);
                return sum;
            }
        }

        /// <summary>
        /// Суммарная прибыль с группы КП.
        /// </summary>
        public decimal ProfitSum { get => PriceSum - CostPriceSum; }

        /// <summary>
        /// Средняя наценка в группе КП.
        /// </summary>
        public decimal CommmonMarkup
        {
            get
            {
                if(CostPriceSum>0)
                    return PriceSum / CostPriceSum;
                return 0;
            }
        }

        private OfferGroup() { }

        public OfferGroup(Offer offer)
        {
            this.offer = offer;
            PropertyChanged += OfferGroup_PropertyChanged;
            NomWrappers.CollectionChanged += NomWrappers_CollectionChanged;
        }

        public void AddNomenclatures(List<NomWrapper> list)
        {
            NomWrappers.CollectionChanged -= NomWrappers_CollectionChanged;
            list.ForEach(n=> nomWrappers.Add(n));
            NomWrappers.CollectionChanged += NomWrappers_CollectionChanged;
            OnPropertyChanged(string.Empty);
        }

        private void NomWrappers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(string.Empty);

        private void OfferGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => offer?.OnPropertyChanged(string.Empty);
    }
}
