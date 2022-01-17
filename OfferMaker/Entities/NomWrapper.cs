using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace OfferMaker
{
    /// <summary>
    /// Обёртка для номенклатуры в КП, содержащая количество, настройки и прочую хуету.
    /// </summary>
    public class NomWrapper : BaseEntity
    {
        bool isRowDetailsVisibility;
        int amount = 1;
        OfferGroup offerGroup;
        Currency currency;
        Currency defaultCurrency;
        string currencyCharCode;
        string currencyCharCode_;
        bool isShowPrice = true;
        bool isIncludeIntoOffer = true;

        #region

        /// <summary>
        /// Название номенклатуры.
        /// </summary>
        public string Title { get => Nomenclature.Title; }

        /// <summary>
        /// Объект номенклатурной единицы, которую оборачмваем.
        /// </summary>
        public Nomenclature Nomenclature { get; set; }

        /// <summary>
        /// Включать/отключать позицию.
        /// Надо узнать, что делает чекбокс NBU слева. А то хуй поймёшь.
        /// </summary>
        public bool IsIncludeIntoOffer
        {
            get => isIncludeIntoOffer;
            set
            {
                isIncludeIntoOffer = value;
                offerGroup?.NomWrappers_CollectionChanged(null, null);
            }
        }

        /// <summary>
        /// Отображать стоимость отдельной позиции в КП или нет.
        /// При этом стоимость позиции независимо от того, показывается цена или нет, включается в общую стоимость, т.е. считается.
        /// </summary>
        public bool IsShowPrice 
        {
            get => isShowPrice;
            set
            {
                isShowPrice=value;
                OnPropertyChanged();
            }
        } 

        /// <summary>
        /// Состояние контрола деталей.
        /// </summary>
        public bool IsRowDetailsVisibility
        {
            get => isRowDetailsVisibility;
            set
            {
                isRowDetailsVisibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция описаний.
        /// </summary>
        public ObservableCollection<Description> Descriptions { get => Nomenclature.Descriptions; }

        /// <summary>
        /// Коллекция путей к фото номенклатуры.
        /// </summary>
        [JsonIgnore]
        public string Photo { get => Nomenclature.Image.LocalPhotoPath; }

        /// <summary>
        /// Спрятать цены номенклатур не в опциях.
        /// </summary>
        [JsonIgnore]
        public bool IsHideNomsPrice { get => offerGroup.IsHideNomsPrice; }

        #endregion

        #region Money

        /// <summary>
        /// Кол-во номенклатурных единиц.
        /// </summary>
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged();
                UpdateAmount();
            }
        }

        /// <summary>
        /// Себестоимость.
        /// </summary>
        [JsonIgnore]
        public decimal CostPrice
        {
            get
            {
                decimal costPrice = CurrencyCharCode == Nomenclature.CurrencyCharCode ? Nomenclature.CostPrice : Nomenclature.CostPrice * DefaultCurrency.Rate / Currency.Rate;
                return IsWithNds ? costPrice : costPrice / 1.2m;
            }
        }

        /// <summary>
        /// Себестоимость в валюте КП.
        /// </summary>
        [JsonIgnore]
        public decimal CostPriceInOfferCurrency
        {
            get
            {
                decimal costPrice = CurrencyCharCode == OfferCurrency.CharCode ? CostPrice : Nomenclature.CostPrice * DefaultCurrency.Rate / OfferCurrency.Rate;
                return IsWithNds ? costPrice : costPrice / 1.2m;
            }
        }

        /// <summary>
        /// Стоимость заданного кол-ва номенклатурных единиц.
        /// В текущей валюте номенклатуры.
        /// </summary>
        [JsonIgnore]
        public decimal Sum { get => offerGroup.IsCreateByCostPrice ? CostPrice * amount : Price * amount; }

        /// <summary>
        /// Стоимость заданного кол-ва номенклатурных единиц.
        /// В текущей валюте КП.
        /// </summary>
        [JsonIgnore]
        public decimal SumInOfferCurrency 
        {
            get 
            { 
                if(CurrencyCharCode==OfferCurrency.CharCode)
                    return offerGroup.IsCreateByCostPrice ? CostPrice * amount : Price * amount;
                else
                {
                    decimal costPriceTemp = Nomenclature.CostPrice * (DefaultCurrency.Rate / OfferCurrency.Rate);
                    costPriceTemp = IsWithNds ? costPriceTemp : costPriceTemp / 1.2m;
                    return offerGroup.IsCreateByCostPrice ? costPriceTemp * amount : costPriceTemp *Markup * amount;
                }
            }
        }

        /// <summary>
        /// Цена одной номенклатурной единицы.
        /// </summary>
        [JsonIgnore]
        public decimal Price { get => offerGroup == null ? 0 : offerGroup.IsCreateByCostPrice ? CostPrice : CostPrice * Markup; }

        /// <summary>
        /// Наценка.
        /// </summary>
        [JsonIgnore]
        public decimal Markup { get => offerGroup.IsCreateByCostPrice ? 1 : Nomenclature.Markup; }

        /// <summary>
        /// Себестоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        [JsonIgnore]
        public decimal CostSum { get => CostPrice * amount; }

        /// <summary>
        /// Себестоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        [JsonIgnore]
        public decimal CostSumInOfferCurrency { get => CostPriceInOfferCurrency * amount; }

        /// <summary>
        /// Суммарная прибыль с заданного кол-ва номенклатурных единиц.
        /// </summary>
        [JsonIgnore]
        public decimal ProfitSum { get => offerGroup.IsCreateByCostPrice ? 0 : Profit * amount; }

        // <summary>
        /// Сумма прибыли при продаже 1 единицы номенклатуры.
        /// </summary>
        public decimal Profit { get => Price - CostPrice; }

        /// <summary>
        /// Для прокидывания свойства IsWithNds от Offer к NomWrapper
        /// </summary>
        [JsonIgnore]
        public bool IsWithNds { get => offerGroup == null ? false : offerGroup.IsWithNds; }

        /// <summary>
        /// Текущая валюта номенклатуры.
        /// </summary>
        [JsonIgnore]
        public Currency Currency
        {
            get
            {
                if (currency == null)
                    currency = Global.GetCurrencyByCode(CurrencyCharCode); 
                return currency;
            }
            set
            {
                currency = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Базовая валюта номенклатуры.
        /// </summary>
        [JsonIgnore]
        public Currency DefaultCurrency
        {
            get
            {
                if (defaultCurrency == null)
                    defaultCurrency = Global.GetCurrencyByCode(Nomenclature.CurrencyCharCode); 
                return defaultCurrency;
            }
        }

        /// <summary>
        /// Символьный код валюты текущей.
        /// </summary>
        [JsonIgnore]
        public string CurrencyCharCode
        {
            get => currencyCharCode ?? Nomenclature.CurrencyCharCode;
            set
            {
                currencyCharCode = value;
                currency = Global.GetCurrencyByCode(currencyCharCode); 
                OnPropertyChanged();
                UpdateCurrency();
            }
        }

        /// <summary>
        /// Для сериализации без побочных эффектов.
        /// </summary>
        public string CurrencyCharCode_
        {
            get => currencyCharCode_;
            set => currencyCharCode_ = value;
        }

        /// <summary>
        /// Ссылка на валюту КП.
        /// </summary>
        [JsonIgnore]
        public Currency OfferCurrency { get => offerGroup.Currency; }

        #endregion Money

        #region Init

        /// <summary>
        /// Передаём в конструктор родительский класс, чтобы оповещать его об изменениях.
        /// </summary>
        /// <param name="offerGroup"></param>
        public NomWrapper(OfferGroup offerGroup, Nomenclature nomenclature)
        {
            this.offerGroup = offerGroup;
            Nomenclature = nomenclature;
        }

        /// <summary>
        /// Делаем конструктор приватным, чтобы работал только публичный,
        /// где мы передаём ссылку на родительский элемент.
        /// </summary>
        private NomWrapper() { }

        /// <summary>
        /// Для нормального функционирования после сериализации.
        /// </summary>
        /// <param name="offerGroup"></param>
        public void SetOfferGroup(OfferGroup offerGroup) => this.offerGroup = offerGroup;

        #endregion Init

        #region Methods

        /// <summary>
        /// Обновление валюты.
        /// </summary>
        public void UpdateCurrency()
        {
            OnPropertyChanged(nameof(Sum));
            OnPropertyChanged(nameof(CostSum));
            OnPropertyChanged(nameof(ProfitSum));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(CostPrice));
            OnPropertyChanged(nameof(Currency));
            offerGroup?.OnPropertyChanged("PriceSumWrapper");
            offerGroup?.OnPropertyChanged("CurrencyWrapper");
        }

        /// <summary>
        /// Обновления кол-ва номенклатуры.
        /// </summary>
        private void UpdateAmount()
        {
            OnPropertyChanged(nameof(Sum));
            OnPropertyChanged(nameof(CostSum));
            OnPropertyChanged(nameof(ProfitSum));
            offerGroup?.NomWrappers_CollectionChanged(null, null);
        }

        #endregion Methods
    }
}
