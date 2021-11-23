using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Offer : BaseModel
    {
        ObservableCollection<OfferGroup> offerGroups = new ObservableCollection<OfferGroup>();
        ObservableCollection<string> afterTitleCollection;
        User manager;
        User offerCreator;
        bool isHiddenTextNds;
        bool isWithNds = true;
        Currency currency;
        bool resultSummInRub;
        bool isShowPriceDetails;
        bool isCreateByCostPrice;
        bool isHideNomsPrice;
        DateTime createDate = DateTime.Now;
        string createDateString;
        string banner;

        public int Id { get; set; }

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
            get => manager;
            set
            {
                manager = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<string> Images { get; set; }

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

        public string OfferName { get; set; }

        public Customer Customer { get; set; }

        public decimal TotalSum { get; set; }

        /// <summary>
        /// Рекламмные материалы, идущие после титульника.
        /// </summary>
        public ObservableCollection<string> AfterTitleCollection
        {
            get => afterTitleCollection;            
            set
            {
                afterTitleCollection = value;
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
    }
}
