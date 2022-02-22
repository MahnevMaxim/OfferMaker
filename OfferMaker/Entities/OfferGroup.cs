using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class OfferGroup : BaseEntity
    {
        Offer offer;
        ObservableCollection<NomWrapper> nomWrappers = new ObservableCollection<NomWrapper>();
        string groupTitle;
        bool isOption;
        bool isEnabled = true;

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
                offer?.UpdateIsOption();
            }
        }

        /// <summary>
        /// Вкл/выкл.
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
                offer?.UpdateIsEnabled();
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
        /// Спрятать текст НДС из КП если IsHiddenTextNds=true.
        /// </summary>
        [JsonIgnore]
        public bool IsHiddenTextNds { get => offer.IsHiddenTextNds; }

        /// <summary>
        /// Спрятать цены номенклатур не в опциях.
        /// </summary>
        [JsonIgnore]
        public bool IsHideNomsPrice { get => offer.IsHideNomsPrice; }

        #region money

        /// <summary>
        /// Суммарная себестоимость группы КП.
        /// </summary>
        [JsonIgnore]
        public decimal CostPriceSum
        {
            get
            {
                decimal sum = 0;
                NomWrappers.ToList().ForEach(n => { if (n.IsIncludeIntoOffer) sum += n.CostSumInOfferCurrency; });
                return sum;
            }
        }

        /// <summary>
        /// Суммарная цена группы КП.
        /// </summary>
        [JsonIgnore]
        public decimal PriceSum
        {
            get
            {
                decimal sum = 0;
                NomWrappers.ToList().ForEach(n => { if (n.IsIncludeIntoOffer) sum += n.SumInOfferCurrency; });
                OnPropertyChanged(nameof(PriceSumWrapper));
                OnPropertyChanged(nameof(CurrencyWrapper));
                return sum;
            }
        }

        /// <summary>
        /// Суммарная цена группы КП. Обёртка для отображения суммы группы, предназначена для того, чтобы не ломать логику
        /// подсчёта суммы.
        /// </summary>
        [JsonIgnore]
        public decimal PriceSumWrapper
        {
            get
            {
                decimal sum = 0;
                if (NomWrappers.Count > 0)
                {
                    string firstCurrencyCode = NomWrappers[0].CurrencyCharCode;
                    bool isOneCurrency = NomWrappers.All(n => n.Currency.CharCode == firstCurrencyCode);
                    if (isOneCurrency && firstCurrencyCode!=Currency.CharCode)
                    {
                        //подсчёт суммы, если вся группа в одной валюте, отличной от валюты КП
                        decimal tempSum = 0;
                        NomWrappers.ToList().ForEach(n => { if (n.IsIncludeIntoOffer) tempSum += n.SumInOfferCurrency; });
                        sum = tempSum *(Currency.Rate/ NomWrappers[0].Currency.Rate);
                    }
                    else
                    {
                        NomWrappers.ToList().ForEach(n => { if (n.IsIncludeIntoOffer) sum += n.SumInOfferCurrency; });
                    }
                }
                return sum;
            }
        }

        /// <summary>
        /// Суммарная прибыль с группы КП.
        /// </summary>
        [JsonIgnore]
        public decimal ProfitSum { get => PriceSum - CostPriceSum; }

        /// <summary>
        /// Средняя наценка в группе КП.
        /// </summary>
        [JsonIgnore]
        public decimal CommmonMarkup
        {
            get
            {
                if (CostPriceSum > 0)
                    return PriceSum / CostPriceSum;
                return 0;
            }
        }

        /// <summary>
        /// Ссылка на переключатель режима создания КП по нормальной/закупочной цене.
        /// </summary>
        [JsonIgnore]
        public bool IsCreateByCostPrice { get => offer.IsCreateByCostPrice; }

        /// <summary>
        /// Ссылка на валюту КП.
        /// </summary>
        [JsonIgnore]
        public Currency Currency { get => offer.Currency; }

        /// <summary>
        /// Обёртка для отображения символа валюты в зависимости от того, в какой валюте номенклатуры, 
        /// если все в одной валюте, отличной от валюты КП, то изменяем валюту.
        /// </summary>
        [JsonIgnore]
        public Currency CurrencyWrapper 
        { 
            get
            {
                if (NomWrappers.Count > 0)
                {
                    string firstCurrencyCode = NomWrappers[0].CurrencyCharCode;
                    bool isOneCurrency = NomWrappers.All(n => n.Currency.CharCode == firstCurrencyCode);
                    if (isOneCurrency && firstCurrencyCode != Currency.CharCode)
                    {
                        return Global.GetConstructorCurrencyByCode(firstCurrencyCode);
                    }
                }
                return offer.Currency;
            } 
        }

        /// <summary>
        /// Для прокидывания свойства IsWithNds от Offer к NomWrapper
        /// </summary>
        [JsonIgnore]
        public bool IsWithNds { get => offer.IsWithNds; }

        #endregion money

        #region Init

        private OfferGroup() { }

        public OfferGroup(Offer offer)
        {
            this.offer = offer;
            NomWrappers.CollectionChanged += NomWrappers_CollectionChanged;
        }

        public void SetOffer(Offer offer) => this.offer = offer;

        #endregion Init

        #region Methods

        public void AddNomenclaturesSilent(List<NomWrapper> list)
        {
            NomWrappers.CollectionChanged -= NomWrappers_CollectionChanged;
            list.ForEach(n => nomWrappers.Add(n));
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

        #endregion Methods
    }
}
