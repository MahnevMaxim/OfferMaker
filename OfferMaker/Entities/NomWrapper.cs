using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// Название номенклатуры.
        /// </summary>
        public string Title { get => Nomenclature.Title; }

        /// <summary>
        /// Объект номенклатурной единицы, которую оборачмваем.
        /// </summary>
        public Nomenclature Nomenclature { get; set; }

        /// <summary>
        /// Кол-во номенклатурных единиц.
        /// </summary>
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                L.LW("start");
                OnPropertyChanged();
                OnPropertyChanged(nameof(Sum));
                OnPropertyChanged(nameof(CostSum));
                OnPropertyChanged(nameof(ProfitSum));
                offerGroup?.NomWrappers_CollectionChanged(null, null); 
            }
        }

        /// <summary>
        /// Стоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal Sum 
        {
            get 
            {
                if(offerGroup.IsCreateByCostPrice) return Nomenclature.CostPrice * amount;
                return Nomenclature.Price * amount;
            } 
        }

        /// <summary>
        /// Цена одной номенклатурной единицы.
        /// </summary>
        public decimal Price 
        {
            get
            {
                if (offerGroup.IsCreateByCostPrice) return Nomenclature.CostPrice;
                return Nomenclature.Price;
            }
        }

        /// <summary>
        /// Наценка.
        /// </summary>
        public decimal Markup
        {
            get
            {
                if (offerGroup.IsCreateByCostPrice) return 1;
                return Nomenclature.Markup;
            }
        }

        /// <summary>
        /// Себестоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal CostSum { get => Nomenclature.CostPrice * amount; }

        /// <summary>
        /// Суммарная прибыль с заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal ProfitSum 
        {
            get
            {
                if (offerGroup.IsCreateByCostPrice) return 0;
                return Nomenclature.Profit * amount;
            } 
        }

        /// <summary>
        /// Включать/отключать позицию.
        /// Надо узнать, что делает чекбокс NBU слева. А то хуй поймёшь.
        /// </summary>
        public bool IsIncludeIntoOffer { get; set; } = true;

        /// <summary>
        /// Отображать стоимость отдельной позиции в КП или нет.
        /// При этом стоимость позиции независимо от того, показывается цена или нет, включается в общую стоимость, т.е. считается.
        /// </summary>
        public bool IsShowPrice { get; set; } = true;

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
        public ObservableCollection<Description> Descriptions { get=> Nomenclature.Descriptions; }

        /// <summary>
        /// Коллекция путей к фото номенклатуры.
        /// </summary>
        public string Photo { get => Nomenclature.Photo; }

        /// <summary>
        /// Валюта номенклатуры.
        /// </summary>
        public Currency Currency
        {
            get
            {
                if (currency == null)
                    currency = Global.Main.Currencies.Where(c => c.CharCode == Nomenclature.CurrencyCharCode).FirstOrDefault();
                return currency;
            }
            set
            {
                currency = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Спрятать цены номенклатур не в опциях.
        /// </summary>
        public bool IsHideNomsPrice { get => offerGroup.IsHideNomsPrice; }

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
    }
}
