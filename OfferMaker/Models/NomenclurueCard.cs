using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ImageCropperLibrary;
using System.Windows;
using System.IO;

namespace OfferMaker
{
    public class NomenclurueCard : BaseModel
    {
        string currencyCharCode;
        Catalog catalog;
        Image selectedImage;
        string categoryTitle;

        public Nomenclature Nomenclature { get; set; }

        public bool IsEditConstructor { get; set; }

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

        public string CategoryTitle
        {
            get => catalog?.Categories.Where(c => c.Guid == Nomenclature.CategoryGuid).FirstOrDefault()?.Title;
            set
            {
                categoryTitle = value;
                OnPropertyChanged();
            }
        }

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
        /// Конструктор для редактирования копии номенклатуры из конструктора КП.
        /// </summary>
        /// <param name="nomWrapper"></param>
        public NomenclurueCard(NomWrapper nomWrapper)
        {
            Nomenclature = nomWrapper.Nomenclature;
            CurrencyCharCode = nomWrapper.Nomenclature.CurrencyCharCode;
            Currencies = Global.Main.UsingCurrencies.ToList();
            if (!Currencies.Contains(CurrencyCharCode))
                Currencies.Add(CurrencyCharCode);
            IsEditConstructor = true;
        }

        /// <summary>
        /// Обновляем дату цены
        /// </summary>
        public void RefreshDate() => Nomenclature.LastChangePriceDate = DateTime.UtcNow;

        /// <summary>
        /// Удаляем через фильтр, т.к. там происходит согласование данных.
        /// </summary>
        public void RemoveFromCategory()
        {
            catalog.CatalogFilter.RemoveFromCategory(Nomenclature);
            CategoryTitle = null;
        }

        /// <summary>
        /// Заменяем/добавляем фото
        /// </summary>
        public void AddImage()
        {
            string path = SomeMethod();
            if (path != null && path != "")
            {
                Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                Global.ImageManager.Add(image);
                Nomenclature.SetPhoto(image);
            }
        }

        private string SomeMethod()
        {
            byte[] result = ImageCropper.GetImage(new Size(600, 400));

            string currentDirectory = Directory.GetCurrentDirectory();
            string newImagePath = currentDirectory + @"\images\new\";
            if (!Directory.Exists(newImagePath))
            {
                Directory.CreateDirectory(newImagePath);
            }
            string retPath = newImagePath + DateTime.Now.ToShortDateString() + ".png";
            try
            {
                System.IO.File.WriteAllBytes(retPath, result);
            }
            catch (Exception ex)
            {
                retPath = "";
            }

            //string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");

            return retPath;
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
