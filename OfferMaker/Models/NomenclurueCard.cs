using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class NomenclurueCard : BaseModel
    {
        string currencyCharCode;
        string image;

        public Nomenclature Nomenclature { get; set; }

        public string CurrencyCharCode
        {
            get => currencyCharCode;
            set
            {
                currencyCharCode = value;
                Nomenclature.CurrencyCharCode = currencyCharCode;
                OnPropertyChanged();
            }
        }

        public List<string> Currencies { get; set; }

        public string Image
        {
            get
            {
                if (image != null)
                    return image;
                return Environment.CurrentDirectory + @"\Images\no-image.jpg";
            }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public NomenclurueCard(Nomenclature nomenclature)
        {
            Nomenclature = nomenclature;
            CurrencyCharCode = nomenclature.CurrencyCharCode;
            Currencies = Global.Main.UsingCurrencies.ToList();
            if (!Currencies.Contains(CurrencyCharCode))
            {
                Currencies.Add(CurrencyCharCode);
            }
            if (Nomenclature.Photo != null) Image = Nomenclature.Photo;
        }

        /// <summary>
        /// Обновляем дату цены
        /// </summary>
        public void RefreshDate() => Nomenclature.LastChangePriceDate = DateTime.Now;
        

        /// <summary>
        /// Заменяем/добавляем фото
        /// </summary>
        public void EditImage()
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
            {
                Nomenclature.Photo=path;
                Image = path;
            }
        }

        /// <summary>
        /// Удаляем фото номенклатуры
        /// </summary>
        public void RemoveImage()
        {
            Nomenclature.Photo=null;
            Image = null;
        }

        /// <summary>
        /// Добавляем описание к номенклатуре
        /// </summary>
        public void AddDesription()
        {
            if (Nomenclature.Descriptions == null) Nomenclature.Descriptions = new ObservableCollection<Description>();
            Nomenclature.Descriptions.Add(new Description() { Text = "Описание" });
        }

        /// <summary>
        /// Удаляем описание к номенклатуре
        /// </summary>
        public void RemoveDescription(Description description) =>
            Nomenclature.Descriptions.Remove(description);
    }
}
