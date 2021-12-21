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
        Catalog catalog;
        Image selectedImage;

        public Nomenclature Nomenclature { get; set; }

        public Image SelectedImage
        {
            get { return selectedImage; }
            set
            {
                selectedImage = value;
                OnPropertyChanged();
            }
        }

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

        public string CategoryTitle { get => catalog.Categories.Where(c => c.Guid == Nomenclature.CategoryGuid).FirstOrDefault()?.Title; }

        public List<string> Currencies { get; set; }

        public NomenclurueCard(Nomenclature nomenclature, Catalog catalog)
        {
            this.catalog = catalog;
            Nomenclature = nomenclature;
            CurrencyCharCode = nomenclature.CurrencyCharCode;
            Currencies = Global.Main.UsingCurrencies.ToList();
            if (!Currencies.Contains(CurrencyCharCode))
                Currencies.Add(CurrencyCharCode);
        }

        /// <summary>
        /// Обновляем дату цены
        /// </summary>
        public void RefreshDate() => Nomenclature.LastChangePriceDate = DateTime.UtcNow;


        /// <summary>
        /// Заменяем/добавляем фото
        /// </summary>
        public void AddImage()
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
            {
                Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                Global.ImageManager.Add(image);
                Nomenclature.SetPhoto(image);
            }
        }

        /// <summary>
        /// Удаляем фото номенклатуры
        /// </summary>
        public void RemoveImage(Image image)
        {
            Nomenclature.RemoveImage(image);
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
