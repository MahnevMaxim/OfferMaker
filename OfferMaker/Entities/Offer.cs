using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Offer : BaseEntity
    {
        /// <summary>
        /// Счётчик добавлений групп в текущем КП,
        /// используется только для выводв названия групп.
        /// </summary>
        public int addGroupsCounter;
        /// <summary>
        /// Обсервер.
        /// </summary>
        Constructor constructor;

        ObservableCollection<OfferGroup> offerGroups = new ObservableCollection<OfferGroup>();
        ObservableCollection<OfferInfoBlock> offerInfoBlocks = new ObservableCollection<OfferInfoBlock>();
        ObservableCollection<string> advertisingsUp;
        ObservableCollection<string> advertisingsDown;
        DateTime createDate = DateTime.Now;
        User manager;
        User offerCreator;
        Currency currency;
        Customer customer = new Customer();
        Discount discount;
        string createDateString;
        string banner;
        string offerName;
        bool isHiddenTextNds;
        bool isWithNds = true;
        bool resultSummInRub;
        bool isShowPriceDetails;
        bool isCreateByCostPrice;
        bool isHideNomsPrice;


        public int Id { get; set; }

        /// <summary>
        /// Группы номенклатур для контрола конструктора.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroups
        {
            get => offerGroups;
            set
            {
                offerGroups = value;
                OnPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Коллекция информблоков.
        /// </summary>
        public ObservableCollection<OfferInfoBlock> OfferInfoBlocks
        {
            get => offerInfoBlocks;
            set
            {
                offerInfoBlocks = value;
                if (offerInfoBlocks != null) offerInfoBlocks.CollectionChanged += OfferInfoBlocks_CollectionChanged;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Рекламмные материалы, идущие после титульника.
        /// </summary>
        public ObservableCollection<string> AdvertisingsUp
        {
            get => advertisingsUp;
            set
            {
                advertisingsUp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Рекламмные материалы, идущие внизу.
        /// </summary>
        public ObservableCollection<string> AdvertisingsDown
        {
            get => advertisingsDown;
            set
            {
                advertisingsDown = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата создания КП.
        /// </summary>
        public DateTime CreateDate
        {
            get => createDate;
            set
            {
                createDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Валюта, в которую надо пересчитать КП.
        /// </summary>
        public Currency Currency
        {
            get => currency;
            set
            {
                currency = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Данные клиента.
        /// </summary>
        public Customer Customer
        {
            get => customer;
            set
            {
                customer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Создатель КП.
        /// </summary>
        public User OfferCreator
        {
            get => offerCreator;
            set
            {
                offerCreator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Назначенный менеджер КП.
        /// </summary>
        public User Manager
        {
            get => manager ?? offerCreator;
            set
            {
                manager = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Большая картинка с телефоном справа от менеджера.
        /// </summary>
        public string PhotoNumberTeh { get => constructor.PhotoNumberTeh; }

        /// <summary>
        /// Баннер.
        /// </summary>
        public string Banner
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(banner))
                    return banner;
                return Environment.CurrentDirectory + @"\Images\no-image.jpg";
            }
            set
            {
                banner = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отображение даты в удобочитаемом формате.
        /// </summary>
        public string CreateDateString
        {
            get => CreateDate.ToLongDateString();
            set
            {
                createDateString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Название КП.
        /// </summary>
        public string OfferName
        {
            get => offerName;
            set
            {
                offerName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Полная цена КП.
        /// </summary>
        public decimal TotalSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => totalSum += o.PriceSum);
                return totalSum;
            }
        }

        /// <summary>
        /// Полная цена КП.
        /// </summary>
        public decimal TotalSumWithoutOptions
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().Where(o=>!o.IsOption).ToList().ForEach(o => totalSum += o.PriceSum);
                return totalSum;
            }
        }

        /// <summary>
        /// Себестоимость КП.
        /// </summary>
        public decimal TotalCostPriceSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => totalSum += o.CostPriceSum);
                return totalSum;
            }
        }

        /// <summary>
        /// Усрелнённая наценка в текущем КП.
        /// </summary>
        public decimal AverageMarkup { get => TotalCostPriceSum != 0 ? TotalSum / TotalCostPriceSum : 0; }

        /// <summary>
        /// Предполагаемая сумма прибыли с КП.
        /// </summary>
        public decimal ProfitSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => totalSum += o.ProfitSum);
                return totalSum;
            }
        }

        /// <summary>
        /// Спрятать текст НДС из КП если IsHiddenTextNds=true.
        /// </summary>
        public bool IsHiddenTextNds
        {
            get => isHiddenTextNds;
            set
            {
                isHiddenTextNds = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если IsWithNds=true, то считать цену с НДС.
        /// </summary>
        public bool IsWithNds
        {
            get => isWithNds;
            set
            {
                isWithNds = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если IsShowPriceDetails=true, то отображаем в конструкторе цену детально.
        /// </summary>
        public bool IsShowPriceDetails
        {
            get => isShowPriceDetails;
            set
            {
                isShowPriceDetails = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если IsCreateByCostPrice=true, то формируем КП по себестоимости.
        /// </summary>
        public bool IsCreateByCostPrice
        {
            get => isCreateByCostPrice;
            set
            {
                isCreateByCostPrice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если IsHideNomsPrice=true, то скрываем в КП цены отдельных номенклатур.
        /// </summary>
        public bool IsHideNomsPrice
        {
            get => isHideNomsPrice;
            set
            {
                isHideNomsPrice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если ResultSummInRub=true, то считать цену в рублях.
        /// </summary>
        public bool ResultSummInRub
        {
            get => resultSummInRub;
            set
            {
                resultSummInRub = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Объект дисконта.
        /// </summary>
        public Discount Discount { get => discount; }

        private Offer() { }

        public Offer(Constructor constructor)
        {
            this.constructor = constructor;
            PropertyChanged += Offer_PropertyChanged;
            OfferInfoBlocks.CollectionChanged += OfferInfoBlocks_CollectionChanged;
            discount = new Discount(this);
        }

        private void OfferInfoBlocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(string.Empty);

        private void Offer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => constructor?.OnPropertyChanged(string.Empty);

        /// <summary>
        /// Чтобы не получить переполнение стека.
        /// </summary>
        /// <param name="curr"></param>
        internal void SetCurrencySilent(Currency curr) => currency = curr;

    }
}
