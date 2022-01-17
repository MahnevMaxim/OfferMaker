using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class Offer : BaseEntity
    {
        /// <summary>
        /// Счётчик добавлений групп в текущем КП,
        /// используется только для выводв названия групп.
        /// </summary>
        int addGroupsCounter;
        /// <summary>
        /// Обсервер.
        /// </summary>
        Constructor constructor;

        ObservableCollection<OfferGroup> offerGroups = new ObservableCollection<OfferGroup>();
        ObservableCollection<OfferInfoBlock> offerInfoBlocks = new ObservableCollection<OfferInfoBlock>();
        ObservableCollection<string> advertisingsUp = new ObservableCollection<string>();
        ObservableCollection<string> advertisingsDown = new ObservableCollection<string>();
        DateTime createDate = DateTime.Now;
        User manager;

        int id;
        string guid;
        User offerCreator;
        Currency currency;
        Customer customer = new Customer();
        int offerCreatorId;
        int managerId;
        Discount discount;
        string createDateString;
        string banner;
        string offerName = "Новое КП";
        bool isHiddenTextNds;
        bool isWithNds = true;
        bool isResultSummInRub;
        bool isShowPriceDetails;
        bool isCreateByCostPrice;
        bool isHideNomsPrice;
        bool isTemplate;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
                constructor?.viewModel.OnPropertyChanged(nameof(AltId));
            }
        }

        public string AltId { get => Id == 0 ? "" : CreateDate.ToShortDateString() + "-" + Id; }

        public string Guid
        {
            get => guid;
            set
            {
                guid = value;
                OnPropertyChanged();
            }
        }

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
        /// Группы, которые являются опциями.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroupsOptions { get => new ObservableCollection<OfferGroup>(OfferGroups.Where(o => o.IsOption && o.IsEnabled).ToList()); }

        /// <summary>
        /// Группы, которые не являются опциями.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroupsNotOptions { get => new ObservableCollection<OfferGroup>(OfferGroups.Where(o => !o.IsOption && o.IsEnabled).ToList()); }

        /// <summary>
        /// Имеются ли в наличии группы, которые не опции.
        /// </summary>
        public bool IsRequiredGroups { get => OfferGroupsNotOptions.Count == 0 ? false : true; }

        /// <summary>
        /// Имеются ли в наличии группы, которые опции.
        /// </summary>
        public bool IsRequiredOptions { get => OfferGroupsOptions.Count == 0 ? false : true; }

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
        [JsonIgnore]
        public User OfferCreator
        {
            get => offerCreator;
            set
            {
                offerCreator = value;
                if (value != null)
                    offerCreatorId = offerCreator.Id;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Назначенный менеджер КП.
        /// </summary>
        [JsonIgnore]
        public User Manager
        {
            get => manager ?? OfferCreator;
            set
            {
                manager = value;
                if (manager == null)
                    manager = OfferCreator;
                managerId = manager.Id;
                constructor?.viewModel.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Id создателя КП.
        /// </summary>
        public int OfferCreatorId
        {
            get => offerCreatorId;
            set => offerCreatorId = value;
        }

        /// <summary>
        /// Id менеджера.
        /// </summary>
        public int ManagerId
        {
            get => managerId == 0 ? offerCreatorId : managerId;
            set => managerId = value;
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
        /// Большая картинка с телефоном справа от менеджера.
        /// </summary>
        public string PhotoNumberTeh { get => constructor?.PhotoNumberTeh; }

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
        /// Время создания КП.
        /// </summary>
        public string CreateTimeString
        {
            get => CreateDate.ToLongTimeString();
            set => createDateString = value;
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
                constructor?.viewModel.OnPropertyChanged(nameof(OfferName));
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
                UpdatePrices();
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
                OfferGroups.ToList().ForEach(g => g.OnPropertyChanged(nameof(IsHiddenTextNds)));
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
        /// Если IsHideNomsPrice=true, то скрываем в КП цены отдельных номенклатур.
        /// </summary>
        public bool IsHideNomsPrice
        {
            get => isHideNomsPrice;
            set
            {
                isHideNomsPrice = value;
                OfferGroupsNotOptions.ToList().ForEach(o => o.NomWrappers.ToList().ForEach(n => n.OnPropertyChanged()));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Если IsTemplate=true, то это шаблон.
        /// </summary>
        public bool IsTemplate
        {
            get => isTemplate;
            set
            {
                isTemplate = value;
                OnPropertyChanged();
            }
        }

        #region Money

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
        /// Полная цена КП.
        /// </summary>
        [JsonIgnore]
        public decimal TotalSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => { if (o.IsEnabled) totalSum += o.PriceSum; });
                if (IsNeedCalculateInRub) totalSum = totalSum * Currency.Rate;
                return totalSum;
            }
        }

        /// <summary>
        /// Полная цена КП без опций.
        /// </summary>
        [JsonIgnore]
        public decimal TotalSumWithoutOptions
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().Where(o => !o.IsOption && o.IsEnabled).ToList().ForEach(o => totalSum += o.PriceSum);
                if (IsNeedCalculateInRub) totalSum = totalSum * Currency.Rate;
                Discount.TotalSum = totalSum;
                UpdateDiscountData();
                return totalSum;
            }
        }

        /// <summary>
        /// Полная цена КП с опциями.
        /// </summary>
        [JsonIgnore]
        public decimal TotalSumOptions
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().Where(o => o.IsOption && o.IsEnabled).ToList().ForEach(o => totalSum += o.PriceSum);
                if (IsNeedCalculateInRub) totalSum = totalSum * Currency.Rate;
                return totalSum;
            }
        }

        /// <summary>
        /// Себестоимость КП.
        /// </summary>
        [JsonIgnore]
        public decimal TotalCostPriceSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => { if (o.IsEnabled) totalSum += o.CostPriceSum; });
                if (IsNeedCalculateInRub) totalSum = totalSum * Currency.Rate;
                return totalSum;
            }
        }

        /// <summary>
        /// Усреднённая наценка в текущем КП.
        /// </summary>
        [JsonIgnore]
        public decimal AverageMarkup { get => TotalCostPriceSum != 0 ? TotalSum / TotalCostPriceSum : 0; }

        /// <summary>
        /// Предполагаемая сумма прибыли с КП.
        /// </summary>
        [JsonIgnore]
        public decimal ProfitSum
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().ForEach(o => { if (o.IsEnabled) totalSum += o.ProfitSum; });
                if (IsNeedCalculateInRub) totalSum = totalSum * Currency.Rate;
                return totalSum;
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
                UpdatePrices();
            }
        }

        /// <summary>
        /// Если IsResultSummInRub=true, то считать цену в рублях.
        /// </summary>
        public bool IsResultSummInRub
        {
            get => isResultSummInRub;
            set
            {
                isResultSummInRub = value;
                UpdateOfferSums();
            }
        }

        /// <summary>
        /// Объект дисконта.
        /// </summary>
        public Discount Discount { get => discount; set => discount = value; }

        /// <summary>
        /// Нужно ли пересчитывать.
        /// </summary>
        public bool IsNeedCalculateInRub { get => isResultSummInRub && Currency.CharCode != "RUB" ? true : false; }

        /// <summary>
        /// Список валют для архива.
        /// </summary>
        public ObservableCollection<Currency> Currencies { get; set; }

        #endregion Money

        public Offer() { }

        /// <summary>
        /// Конструктор первой инициализации объекта.
        /// </summary>
        /// <param name="constructor"></param>
        public Offer(Constructor constructor, Currency currency, ObservableCollection<OfferInfoBlock> offerInfoBlocks, User offerCreator, string banner)
        {
            this.constructor = constructor;
            Currency = currency;
            OfferInfoBlocks = offerInfoBlocks;
            OfferCreator = offerCreator;
            Banner = banner;
            OfferInfoBlocks.CollectionChanged += OfferInfoBlocks_CollectionChanged;
            OfferGroups.CollectionChanged += OfferGroups_CollectionChanged;
            Guid = System.Guid.NewGuid().ToString();
            discount = new Discount(this);
        }

        public void SetConstructor(Constructor constructor)
        {
            this.constructor = constructor;
            OfferInfoBlocks.CollectionChanged += OfferInfoBlocks_CollectionChanged;
            OfferGroups.CollectionChanged += OfferGroups_CollectionChanged;
            constructor.viewModel.OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Чтобы не получить переполнение стека.
        /// </summary>
        /// <param name="curr"></param>
        internal void SetCurrencySilent(Currency curr)
        {
            currency = curr;
            UpdateOfferCurrency();
        }

        public int GetAddGroupsCounter() => ++addGroupsCounter;

        internal Offer SetCurrency()
        {
            Currencies = new ObservableCollection<Currency>();
            Currencies.Add(Currency);
            foreach(OfferGroup offerGroup in OfferGroups)
            {
                foreach(NomWrapper nw in offerGroup.NomWrappers)
                {
                    if(!Currencies.Contains(nw.Currency))
                    {
                        Currencies.Add(nw.Currency);
                    }

                    if(!Currencies.Contains(nw.DefaultCurrency))
                    {
                        Currencies.Add(nw.DefaultCurrency);
                    }
                }
            }
            return this;
        }

        #region InterfaceUpdaters

        public void OfferGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            constructor.viewModel.OnPropertyChanged(nameof(ProfitSum));
            constructor.viewModel.OnPropertyChanged(nameof(TotalCostPriceSum));
            constructor.viewModel.OnPropertyChanged(nameof(AverageMarkup));
            constructor.viewModel.OnPropertyChanged(nameof(TotalSumOptions));
            constructor.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
            constructor.viewModel.OnPropertyChanged(nameof(TotalSum));
            UpdateColls();
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredGroups));
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredOptions));
        }

        internal void UpdateIsOption()
        {
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredGroups));
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredOptions));
            constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsOptions));
            constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsNotOptions));
            constructor.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
        }

        private void OfferInfoBlocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(string.Empty);
        }

        private void UpdatePrices()
        {
            OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(w =>
            {
                w.OnPropertyChanged("Markup");
                w.OnPropertyChanged("Price");
                w.OnPropertyChanged("Sum");
                w.OnPropertyChanged("ProfitSum");
                w.OnPropertyChanged("CostPrice");
                w.OnPropertyChanged("CostSum");
            }));
            OfferGroups.ToList().ForEach(g =>
            {
                g.OnPropertyChanged("PriceSum");
                g.OnPropertyChanged("ProfitSum");
                g.OnPropertyChanged("CommmonMarkup");
                g.OnPropertyChanged("CostPriceSum");
            });
            constructor?.viewModel.OnPropertyChanged(nameof(ProfitSum));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalCostPriceSum));
            constructor?.viewModel.OnPropertyChanged(nameof(AverageMarkup));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSum));
        }

        bool isCreatorBusy;
        bool isNeedUpdate;
        DateTime beginExecuteTime;
        DateTime needUpdateSetTime;
        async void UpdateColls()
        {
            isNeedUpdate = true;
            needUpdateSetTime = DateTime.UtcNow; //если какое-то событие произошло втечение определённого времени, то скипаем его в цикле while
            if (isCreatorBusy) return;
            int countNoms = OfferGroups.SelectMany(g => g.NomWrappers).Count();
            isCreatorBusy = true;
            beginExecuteTime = DateTime.UtcNow;
            while (isNeedUpdate)
            {
                isNeedUpdate = false;
                try
                {
                    //скидыаем быстрые действия, вроде быстрых нажатий на клавиатуру и быстрые щелчки мышью, выполняем только последнее событие
                    if (countNoms > 10)
                        await Task.Delay(2000);
                    if (isNeedUpdate) continue;
                    if (countNoms > 10)
                        await Task.Delay(2000);
                    constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsOptions));
                    constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsNotOptions));
                    if (countNoms > 10)
                        await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    await Task.Delay(4000);
                    Log.Write(ex);
                }
                var difference = (needUpdateSetTime - beginExecuteTime).Milliseconds;
                if (difference < 1000) break;
            }
            isCreatorBusy = false;
        }

        void UpdateDiscountData() => Discount.CalculateByTotalSum();

        public void UpdateDiscountUI()
        {
            Discount.OnPropertyChanged("PriceWithDiscount");
            Discount.OnPropertyChanged("DiscountSum");
            Discount.OnPropertyChanged("Percentage");
            Discount.OnPropertyChanged("Percentage");
            constructor?.viewModel?.OnPropertyChanged(nameof(Currency));
        }

        private void UpdateOfferCurrency()
        {
            OfferGroups.ToList().ForEach(g =>
            {
                g.OnPropertyChanged("PriceSum");
                g.OnPropertyChanged("ProfitSum");
                g.OnPropertyChanged("CostPriceSum");
                g.OnPropertyChanged("Currency");
            });
            constructor?.viewModel.OnPropertyChanged(nameof(AverageMarkup));
            UpdateOfferSums();
        }

        private void UpdateOfferSums()
        {
            constructor?.viewModel.OnPropertyChanged(nameof(ProfitSum));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalCostPriceSum));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSum));
            constructor?.viewModel.OnPropertyChanged(nameof(Currency));
        }

        internal void UpdateIsEnabled()
        {
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredGroups));
            constructor.viewModel.OnPropertyChanged(nameof(IsRequiredOptions));
            constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsOptions));
            constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsNotOptions));
            constructor.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(ProfitSum));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalCostPriceSum));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
            constructor?.viewModel.OnPropertyChanged(nameof(TotalSum));
            constructor?.viewModel.OnPropertyChanged(nameof(AverageMarkup));
        }

        #endregion InterfaceUpdaters
    }
}