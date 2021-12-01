using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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

        /// <summary>
        /// Ссылка на переключатель режима создания КП по нормальной/закупочной цене.
        /// </summary>
        public bool IsCreateByCostPrice { get => offer.IsCreateByCostPrice; }

        /// <summary>
        /// Ссылка на валюту КП.
        /// </summary>
        public Currency Currency { get => offer.Currency; }

        /// <summary>
        /// Спрятать текст НДС из КП если IsHiddenTextNds=true.
        /// </summary>
        public bool IsHiddenTextNds { get => offer.IsHiddenTextNds; }

        /// <summary>
        /// Спрятать цены номенклатур не в опциях.
        /// </summary>
        public bool IsHideNomsPrice { get => offer.IsHideNomsPrice; }

        private OfferGroup() { }

        public OfferGroup(Offer offer)
        {
            this.offer = offer;
            NomWrappers.CollectionChanged += NomWrappers_CollectionChanged;
        }

        public void AddNomenclaturesSilent(List<NomWrapper> list)
        {
            NomWrappers.CollectionChanged -= NomWrappers_CollectionChanged;
            list.ForEach(n=> nomWrappers.Add(n));
            NomWrappers.CollectionChanged += NomWrappers_CollectionChanged;
            NomWrappers_CollectionChanged(null, null);
        }

        public void NomWrappers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsContainsNoms));
            OnPropertyChanged(nameof(CostPriceSum));
            OnPropertyChanged(nameof(PriceSum));
            OnPropertyChanged(nameof(ProfitSum));
            OnPropertyChanged(nameof(CommmonMarkup));
            offer.OfferGroups_CollectionChanged(sender, e);
        }
    }
}
