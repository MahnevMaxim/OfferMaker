using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

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
        ObservableCollection<string> advertisingsUp = new ObservableCollection<string>();
        ObservableCollection<string> advertisingsDown = new ObservableCollection<string>();
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
        /// Группы, которые являются опциями.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroupsOptions { get => new ObservableCollection<OfferGroup>(OfferGroups.Where(o => o.IsOption).ToList()); }

        /// <summary>
        /// Группы, которые не являются опциями.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroupsNotOptions { get => new ObservableCollection<OfferGroup>(OfferGroups.Where(o => !o.IsOption).ToList()); }

        /// <summary>
        /// Имеются ли в наличии группы, которые не опции.
        /// </summary>
        public bool IsRequiredGroups { get => OfferGroupsNotOptions.Count == 0 ? false : true; }

        /// <summary>
        /// Имеются ли в наличии группы, которые опции.
        /// </summary>
        public bool IsRequiredOptions { get => OfferGroupsOptions.Count == 0 ? false : true; }

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
                constructor?.viewModel.OnPropertyChanged(nameof(OfferName));
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
        /// Полная цена КП без опций.
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
        /// Полная цена КП с опциями.
        /// </summary>
        public decimal TotalSumOptions
        {
            get
            {
                decimal totalSum = 0;
                OfferGroups.ToList().Where(o => o.IsOption).ToList().ForEach(o => totalSum += o.PriceSum);
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
                OfferGroups.ToList().ForEach(g=>g.OnPropertyChanged(nameof(IsHiddenTextNds)));
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
                OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(w => 
                {
                    w.OnPropertyChanged("Markup");
                    w.OnPropertyChanged("Price");
                    w.OnPropertyChanged("Sum");
                    w.OnPropertyChanged("ProfitSum");
                }));
                OfferGroups.ToList().ForEach(g => { 
                    g.OnPropertyChanged("PriceSum");
                    g.OnPropertyChanged("ProfitSum");
                    g.OnPropertyChanged("CommmonMarkup");
                });
                OnPropertyChanged(nameof(ProfitSum));
                OnPropertyChanged(nameof(TotalCostPriceSum));
                OnPropertyChanged(nameof(AverageMarkup));
                OnPropertyChanged(nameof(TotalSumOptions));
                constructor?.viewModel.OnPropertyChanged(nameof(TotalSumWithoutOptions));
                OnPropertyChanged(nameof(TotalSum));
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
        public Discount Discount { get => discount; set => discount = value; }

        private Offer() { }

        public Offer(Constructor constructor)
        {
            this.constructor = constructor;
            PropertyChanged += Offer_PropertyChanged;
            OfferInfoBlocks.CollectionChanged += OfferInfoBlocks_CollectionChanged;
            OfferGroups.CollectionChanged += OfferGroups_CollectionChanged;
            discount = new Discount(this);
        }

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

        bool isCreatorBusy;
        bool isNeedUpdate;
        DateTime beginExecuteTime;
        DateTime needUpdateSetTime;
        async void UpdateColls()
        {
            isNeedUpdate = true;
            needUpdateSetTime = DateTime.Now; //если кто-то пришёл втечение определённого времени, то скипаем его в цикле while
            if (isCreatorBusy) return;

            isCreatorBusy = true;
            beginExecuteTime = DateTime.Now;
            while (isNeedUpdate)
            {
                isNeedUpdate = false;
                try
                {
                    //скидыаем быстрые действия, вроде быстрых нажатий на клавиатуру и быстрые щелчки мышью, выполняем только последнее событие
                    await Task.Delay(2000);
                    if (isNeedUpdate) continue;
                    await Task.Delay(6000);
                    constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsOptions));
                    constructor.viewModel.OnPropertyChanged(nameof(OfferGroupsNotOptions));
                    await Task.Delay(4000);
                }
                catch (Exception ex)
                {
                    await Task.Delay(4000);
                    L.LW(ex);
                }
                var difference = (needUpdateSetTime - beginExecuteTime).Milliseconds;
                if (difference < 1000) break;
            }
            isCreatorBusy = false;
        }

        private void OfferInfoBlocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(string.Empty);
        }
            
        private void Offer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Чтобы не получить переполнение стека.
        /// </summary>
        /// <param name="curr"></param>
        internal void SetCurrencySilent(Currency curr) => currency = curr;
    }
}
