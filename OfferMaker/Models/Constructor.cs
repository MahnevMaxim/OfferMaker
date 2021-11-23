using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace OfferMaker
{
    /// <summary>
    /// Центральный класс приложения.
    /// Содержит саму сущность КП в свойстве Offer и методы для её изменения и обработки.
    /// </summary>
    public class Constructor : BaseModel
    {
        #region MVVM

        #region Fields

        FixedDocument pdfDocument;
        string photoLogo;
        string photoNumber;
        string photoCustomer;
        string photoAdress;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Сущность коммерческого предложения, хранящая все настройки и данные.
        /// </summary>
        public Offer Offer { get; set; } = new Offer();

        /// <summary>
        /// Группы номенклатур для контрола конструктора.
        /// </summary>
        public ObservableCollection<DebugTreeItem> DebugTree { get; set; } = new ObservableCollection<DebugTreeItem>();

        public string PhotoLogo
        {
            get => photoLogo;
            set
            {
                photoLogo = value;
                OnPropertyChanged();
            }
        }

        public string PhotoNumber
        {
            get => photoNumber;
            set
            {
                photoNumber = value;
                OnPropertyChanged();
            }
        }

        public string PhotoCustomer
        {
            get => photoCustomer;
            set
            {
                photoCustomer = value;
                OnPropertyChanged();
            }
        }

        public string PhotoAdress
        {
            get => photoAdress;
            set
            {
                photoAdress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Документ для предпросмотра.
        /// </summary>
        public FixedDocument PdfDocument
        {
            get => pdfDocument;
            set
            {
                pdfDocument = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #endregion MVVM

        #region Fields

        /// <summary>
        /// Маленький логотип внизу документа.
        /// </summary>
        BitmapImage smallLogo;

        #endregion Fields

        #region Singleton

        private Constructor()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "common_images\\logosmall.png", UriKind.Absolute);
            smallLogo = new BitmapImage(uri);
            PhotoLogo = AppDomain.CurrentDomain.BaseDirectory + "common_images\\logo.png";
            PhotoNumber = AppDomain.CurrentDomain.BaseDirectory + "common_images\\number.png";
            PhotoCustomer = AppDomain.CurrentDomain.BaseDirectory + "common_images\\customer.png";
            PhotoAdress = AppDomain.CurrentDomain.BaseDirectory + "common_images\\address.png";
            Offer.Banner = Settings.GetDefaultBanner();
            Offer.AfterTitleCollection = new ObservableCollection<string>() { Offer.Banner, Offer.Banner, Offer.Banner, Offer.Banner, Offer.Banner, Offer.Banner };
            Offer.PropertyChanged += Offer_PropertyChanged;
        }
            
        private static readonly Constructor instance = new Constructor();

        public static Constructor GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Для отладки информацию отображаем в TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Offer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DebugTree.Clear();
            var props = Offer.GetType().GetProperties();
            foreach (var pr in props)
            {
                DebugTreeItem item = new DebugTreeItem()
                {
                    PropertyType = pr.PropertyType.Name,
                    PropertyName = pr.Name,
                    PropertyValue = pr.GetValue(Offer)?.ToString()
                };
                DebugTree.Add(item);
            }
            CreatePdf();
        }

        /// <summary>
        /// Обновление документа КП.
        /// </summary>
        private void CreatePdf()
        {
            FlowDocument flowDocument = ((Views.MainWindow)viewModel.view).pdfControl.flowDocumentAll;
            WrapperAllPages wrapper = new WrapperAllPages(flowDocument, viewModel, smallLogo);
            PdfDocument = wrapper.GetPdf(2, 1, 1, 1);
        }

        /// <summary>
        /// Добавление новой группы в конструктор.
        /// </summary>
        internal void AddOfferGroup() => Offer.OfferGroups.Add(new OfferGroup() { GroupTitle = "ГРУППА" });


        /// <summary>
        /// Удаление группы из конструктора.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void DelOfferGroup(OfferGroup offerGroup) => Offer.OfferGroups.Remove(offerGroup);

        /// <summary>
        /// Добавление номенклатуры в группу конструктора.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void AddNomenclatureToOfferGroup(OfferGroup offerGroup) =>
            MvvmFactory.CreateWindow(new AddNomToConstructor(offerGroup), new ViewModels.AddNomToConstructorViewModel(), new Views.AddNomToConstructor(), ViewMode.ShowDialog);

        /// <summary>
        /// Удаление номенклатуры из группы в конструкторе
        /// </summary>
        /// <param name="nomWrapper"></param>
        /// <param name="offerGroup"></param>
        internal void DeleteNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => offerGroup.NomWrappers.Remove(nomWrapper);

        /// <summary>
        /// Удаление описания из номенклатуры из обертки номенклатуры для группы номенклатур в конструкторе.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nomWrapper"></param>
        internal void DeleteDescriptionFromNomWrapper(Description description, NomWrapper nomWrapper) => nomWrapper.Nomenclature.Descriptions.Remove(description);

        /// <summary>
        /// Test
        /// </summary>
        internal void SkipOffer()
        {
            Offer = new Offer();
            Offer.PropertyChanged += Offer_PropertyChanged;
            Offer.OnPropertyChanged(String.Empty);
            viewModel.OnPropertyChanged(String.Empty);
        }

        /// <summary>
        /// Окно выбора баннера и рекламмы.
        /// </summary>
        internal void OpenBanners()
        {
            MvvmFactory.CreateWindow(Global.Main.BannersManager, new ViewModels.BannersManagerViewModel(), new Views.BannersManager(), ViewMode.ShowDialog);
            if(Global.Main.BannersManager.SelectedBanner!=null)
            {
                Offer.Banner = Global.Main.BannersManager.SelectedBanner;
                viewModel.OnPropertyChanged(String.Empty);
            }
        }
    }
}
