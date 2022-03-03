using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class Nomenclature : BaseEntity
    {
        string categoryGuid;
        string title;
        ObservableCollection<Description> descriptions = new ObservableCollection<Description>();
        decimal costPrice;
        decimal markup;
        decimal price;
        decimal profit;
        string currencyCharCode = "RUB";
        DateTime? lastChangePriceDate = DateTime.Now;
        int actualPricePeriod = 30;
        Image image;
        ObservableCollection<Image> images = new ObservableCollection<Image>();
        bool isDelete;

        public int Id { get; set; }

        public string Guid { get; set; }

        /// <summary>
        /// Название номенклатуры.
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                SetIsEdit();
            }
        }


        /// <summary>
        /// Категория.
        /// </summary>
        public string CategoryGuid
        {
            get => categoryGuid;
            set
            {
                categoryGuid = value;
                SetIsEdit();
            }
        }

        /// <summary>
        /// Описания.
        /// </summary>
        public ObservableCollection<Description> Descriptions
        {
            get => descriptions;
            set
            {
                descriptions = value;
                SetIsEdit();
            }
        }

        /// <summary>
        /// Себестоимость.
        /// </summary>
        public decimal CostPrice
        {
            get => costPrice;
            set
            {
                costPrice = value;
                SetIsEdit();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(Profit));
            }
        }

        /// <summary>
        /// Наценка.
        /// </summary>
        public decimal Markup
        {
            get => markup;
            set
            {
                markup = value;
                SetIsEdit();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(Profit));
            }
        }

        /// <summary>
        /// Цена.
        /// </summary>
        [JsonIgnore]
        public decimal Price
        {
            get => CostPrice * Markup;
            set
            {
                price = value;
                SetIsEdit();
                if (price != 0 && CostPrice != 0)
                {
                    markup = price / costPrice;
                    profit = costPrice * markup - costPrice;
                    LastChangePriceDate = DateTime.UtcNow;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Profit));
                OnPropertyChanged(nameof(Markup));
            }
        }

        /// <summary>
        /// Сумма прибыли при продаже.
        /// </summary>
        [JsonIgnore]
        public decimal Profit
        {
            get => Price - CostPrice;
            set
            {
                profit = value;
                SetIsEdit();
                if (price != 0 && profit != 0)
                {
                    markup = (value + costPrice) / costPrice;
                    price = costPrice * markup;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Markup));
                OnPropertyChanged(nameof(Price));
            }
        }

        /// <summary>
        /// Символьный код валюты.
        /// </summary>
        public string CurrencyCharCode
        {
            get => currencyCharCode;
            set
            {
                currencyCharCode = value;
                SetIsEdit();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата последнего изменения цены.
        /// </summary>
        public DateTime? LastChangePriceDate
        {
            get => lastChangePriceDate;
            set
            {
                lastChangePriceDate = value;
                SetIsEdit();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Срок в днях, по истечении которого цена номенклатуры теряет актуальность.
        /// </summary>
        public int ActualPricePeriod
        {
            get => actualPricePeriod;
            set
            {
                actualPricePeriod = value;
                OnPropertyChanged(nameof(IsPriceActual));
                SetIsEdit();
            }
        }

        /// <summary>
        /// Главное фото.
        /// </summary>
        public Image Image
        {
            get => image;
            set
            {
                image = value;
                SetIsEdit();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция фото номенклатуры.
        /// </summary>
        public ObservableCollection<Image> Images
        {
            get
            {
                if (images == null) images = new ObservableCollection<Image>();
                return images;
            }
            set
            {
                images = value;
                SetIsEdit();
            }
        }

        /// <summary>
        /// Указывает, актуальна ли цена у номенклатуры.
        /// </summary>
        [JsonIgnore]
        public bool IsPriceActual
        {
            get
            {
                if (LastChangePriceDate == null) return true;
                return (DateTime.Now - LastChangePriceDate).Value.Days < ActualPricePeriod;
            }
        }

        /// <summary>
        /// Помечена как удалённая.
        /// </summary>
        public bool IsDelete
        {
            get => isDelete;
            set
            {
                isDelete = value;
                if (value) SetIsEdit();
            }
        }

        /// <summary>
        /// Была ли номенклатура отредактирована.
        /// </summary>
        bool isEdit;

        void SetIsEdit() => isEdit = true;

        public bool GetIsEdit() => isEdit;

        internal void SkipIsEdit() => isEdit = false;

        public void SetCategoryGuid(string categoryGuid)
        {
            CategoryGuid = categoryGuid;
            SetIsEdit();
        }

        internal void SetPhoto(Image image)
        {
            Image = image;
            Images.Add(image);
            SetIsEdit();
        }

        internal void RemoveImage(Image image)
        {
            Images.Remove(image);
            if (Images.Count > 0)
                Image = Images[0];
            else
                Image = null;
            SetIsEdit();
        }

        public override string ToString() => "Id:" + Id + " " + Title;

        internal bool IsEqual(Nomenclature nomenclature)
        {
            if (nomenclature.Title != Title)
                return false;

            if (CostPrice != nomenclature.CostPrice || Markup != nomenclature.Markup)
                return false;

            if (nomenclature.Image?.Guid != Image?.Guid)
                return false;

            if (nomenclature.Images.Count != Images.Count)
                return false;

            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i].Guid != nomenclature.Images[i].Guid)
                    return false;
            }

            if (nomenclature.Descriptions.Count != Descriptions.Count)
                return false;

            for (int i = 0; i < Descriptions.Count; i++)
            {
                if (!Descriptions[i].IsEqual(nomenclature.Descriptions[i]))
                    return false;
            }

            return true;
        }

        internal void Edit(Nomenclature nom)
        {
            Title = nom.Title;
            CostPrice = nom.CostPrice;
            Markup = nom.Markup;

            Images.Clear();
            nom.Images.ToList().ForEach(i => Images.Add(i));
            Image = nom.Image;

            Descriptions.Clear();
            nom.Descriptions.ToList().ForEach(d => Descriptions.Add(d));

            isEdit = true;
        }
    }
}
