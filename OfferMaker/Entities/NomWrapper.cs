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
                OnPropertyChanged(nameof(Sum));
                OnPropertyChanged(nameof(CostSum));
                OnPropertyChanged(nameof(ProfitSum));
                offerGroup?.OnPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Передаём в конструктор родительский класс, чтобы оповещать его об изменениях.
        /// Это вариация обсервера минимальными затратами.
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
        /// Стоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal Sum { get => Nomenclature.Price * amount; }

        /// <summary>
        /// Цена одной номенклатурной единицы.
        /// </summary>
        public decimal Price { get => Nomenclature.Price; }

        /// <summary>
        /// Себестоимость заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal CostSum { get => Nomenclature.CostPrice * amount; }

        /// <summary>
        /// Суммарная прибыль с заданного кол-ва номенклатурных единиц.
        /// </summary>
        public decimal ProfitSum { get => Nomenclature.Profit * amount; }

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

        public ObservableCollection<Description> Descriptions { get=> Nomenclature.Descriptions; }

        public ObservableCollection<string> Photos { get => Nomenclature.Photos; }

        public string Title { get => Nomenclature.Title; }

        /// <summary>
        /// Объект номенклатурной единицы, которую оборачмваем.
        /// </summary>
        public Nomenclature Nomenclature { get; set; }
    }
}
