using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Shared;

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
        string photoNumberTeh;
        int pdfControlSelectedIndex;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Индекс нужен, чтобы знать, открыт ли документ и надо ли его обновлять.
        /// Обновление ресурсозатратно.
        /// </summary>
        public int PdfControlSelectedIndex
        {
            get => pdfControlSelectedIndex;
            set
            {
                pdfControlSelectedIndex = value;
                if (pdfControlSelectedIndex == 1)
                    CreatePdf(); 
            }
        }

        /// <summary>
        /// Сущность коммерческого предложения, хранящая все настройки и данные.
        /// </summary>
        public Offer Offer { get; set; }

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

        public string PhotoNumberTeh
        {
            get => photoNumberTeh;
            private set
            {
                photoNumberTeh = value;
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
            PhotoNumberTeh = AppDomain.CurrentDomain.BaseDirectory + "common_images\\telephone.png";
            InitNewOffer();
        }

        private static readonly Constructor instance = new Constructor();

        public static Constructor GetInstance() => instance;

        #endregion Singleton

        private void InitNewOffer()
        {
            Offer = new Offer(this);
            Offer.OfferInfoBlocks = GetInfoBlocks();
            Offer.Banner = Settings.GetDefaultBanner();
            Offer.PropertyChanged += Offer_PropertyChanged;
        }

        private ObservableCollection<OfferInfoBlock> GetInfoBlocks()
        {
            return new ObservableCollection<OfferInfoBlock>()
            {
                new OfferInfoBlock(){
                    Title = "Срок готовности товара к отгрузке",
                    Text ="до 30 рабочих дней",
                    ImagePath=AppDomain.CurrentDomain.BaseDirectory + "informIcons\\Commertial1.png"
                },
                new OfferInfoBlock(){
                    Title = "Срок проведения монтажных и пусконаладочных работ",
                    Text ="до 12 рабочих дней",
                    ImagePath=AppDomain.CurrentDomain.BaseDirectory + "informIcons\\Commertial2.png"
                },
                new OfferInfoBlock(){
                    Title = "Условие оплаты",
                    Text ="50% - аванс, 40% - после извещения о готовности товара к отгрузке, " +
                    "5% - после поступления товара на склад Покупателя, " +
                    "оставшиеся 5 % -в трехдневный срок после подписания акта проведения ШМР и ПНР",
                    ImagePath=AppDomain.CurrentDomain.BaseDirectory + "informIcons\\Commertial3.png"
                },
                new OfferInfoBlock(){
                    Title = "Условия поставки",
                    Text ="Доставка осуществляется за счет Покупателя",
                    ImagePath=AppDomain.CurrentDomain.BaseDirectory + "informIcons\\Commertial4.png"
                }
            };
        }

        /// <summary>
        /// Для отладки информацию отображаем в TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Offer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //DebugTree.Clear();
            //var props = Offer.GetType().GetProperties();
            //foreach (var pr in props)
            //{
            //    DebugTreeItem item = new DebugTreeItem()
            //    {
            //        PropertyType = pr.PropertyType.Name,
            //        PropertyName = pr.Name,
            //        PropertyValue = pr.GetValue(Offer)?.ToString()
            //    };
            //    DebugTree.Add(item);
            //}
            CreatePdf();
        }

        bool isCreatorBusy;
        bool isNeedUpdate;
        /// <summary>
        /// Обновление документа КП.
        /// </summary>
        async public void CreatePdf()
        {
            isNeedUpdate = true;
            if (isCreatorBusy) return;
            if (pdfControlSelectedIndex != 1) return;

            isCreatorBusy = true;
            while (isNeedUpdate)
            {
                isNeedUpdate = false;
                try
                {
                    FlowDocument flowDocument = ((Views.MainWindow)viewModel.view).pdfControl.flowDocumentAll;
                    WrapperAllPages wrapper = new WrapperAllPages(flowDocument, viewModel, smallLogo);
                    PdfDocument = wrapper.GetPdf(2, 1, 1, 1);
                    await Task.Delay(4000);
                }
                catch (Exception ex)
                {
                    await Task.Delay(4000);
                    L.LW(ex);
                }
            }
            isCreatorBusy = false;
        }

        /// <summary>
        /// Добавить кастомный информ блок.
        /// </summary>
        /// <returns></returns>
        internal CallResult AddInformBlock()
        {
            if (Settings.GetMaxInfoblocksCount() == Offer.OfferInfoBlocks.Count) return new CallResult() { Error = new Error("Слишком много блоков") };
            var block = new OfferInfoBlock()
            {
                Title = "Дополнительное описание",
                Text = "Дополнительное описание",
                ImagePath = AppDomain.CurrentDomain.BaseDirectory + "informIcons\\Commertial5.png",
                IsCustom = true
            };
            Offer.OfferInfoBlocks.Add(block);
            return new CallResult();
        }

        /// <summary>
        /// Удалить кастомный информ блок.
        /// </summary>
        /// <param name="offerInfoBlock"></param>
        internal void RemoveInformBlock(OfferInfoBlock offerInfoBlock) => Offer.OfferInfoBlocks.Remove(offerInfoBlock);

        /// <summary>
        /// Добавление новой группы в конструктор.
        /// </summary>
        internal void AddOfferGroup() => Offer.OfferGroups.Add(new OfferGroup(Offer) { GroupTitle = "ГРУППА " + ++Offer.addGroupsCounter });

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
        /// Поднять группу вверх в списке.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void UpOfferGroup(OfferGroup offerGroup)
        {
            int index = Offer.OfferGroups.IndexOf(offerGroup);
            if (index > 0)
            {
                Offer.OfferGroups.Remove(offerGroup);
                Offer.OfferGroups.Insert(index - 1, offerGroup);
            }
        }

        /// <summary>
        /// Опустить группу вниз в списке.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void DownOfferGroup(OfferGroup offerGroup)
        {
            int index = Offer.OfferGroups.IndexOf(offerGroup);
            int maxIndex = Offer.OfferGroups.Count() - 1;
            if (index < maxIndex)
            {
                Offer.OfferGroups.Remove(offerGroup);
                Offer.OfferGroups.Insert(index + 1, offerGroup);
            }
        }

        /// <summary>
        /// Редактирование данных клиента и, по стечению обстоятельств, названия КП.
        /// </summary>
        internal void EditCustomer() => new SimpleViews.EditCustomer(Offer).ShowDialog();

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
            Offer = new Offer(this);
            Offer.PropertyChanged += Offer_PropertyChanged;
            Offer.OnPropertyChanged(String.Empty);
            viewModel.OnPropertyChanged(String.Empty);
        }

        /// <summary>
        /// Окно выбора баннера и рекламмы.
        /// </summary>
        async internal void OpenBanners()
        {
            MvvmFactory.CreateWindow(Global.Main.BannersManager, new ViewModels.BannersManagerViewModel(), new Views.BannersManager(), ViewMode.ShowDialog);
            if (Global.Main.BannersManager.SelectedBanner != null)
            {
                Offer.Banner = Global.Main.BannersManager.SelectedBanner;
            }
            Offer.AdvertisingsUp = Global.Main.BannersManager.AdvertisingsUp;
            Offer.AdvertisingsDown = Global.Main.BannersManager.AdvertisingsDown;
            viewModel.OnPropertyChanged(String.Empty);

            //т.к. документ формируется непосредственно из XAML, а XAML нужна задержка для обновления, то обновляем принудительно с задержкой
            await Task.Delay(100);
            CreatePdf();
        }

        /// <summary>
        /// Поднять описание в списке.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nomWrapper"></param>
        internal void DescriptionMoveUp(Description description, NomWrapper nomWrapper)
        {
            int index = nomWrapper.Nomenclature.Descriptions.IndexOf(description);
            if (index > 0)
            {
                nomWrapper.Nomenclature.Descriptions.Remove(description);
                nomWrapper.Nomenclature.Descriptions.Insert(index - 1, description);
            }
        }

        /// <summary>
        /// Опустить описание в списке.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nomWrapper"></param>
        internal void DescriptionMoveDown(Description description, NomWrapper nomWrapper)
        {
            int index = nomWrapper.Nomenclature.Descriptions.IndexOf(description);
            int maxIndex = nomWrapper.Nomenclature.Descriptions.Count() - 1;
            if (index < maxIndex)
            {
                nomWrapper.Nomenclature.Descriptions.Remove(description);
                nomWrapper.Nomenclature.Descriptions.Insert(index + 1, description);
            }
        }

        /// <summary>
        /// Показать описания номенклатуры.
        /// </summary>
        /// <param name="nomWrapper"></param>
        internal void OpenDescriptions(NomWrapper nomWrapper) => nomWrapper.IsRowDetailsVisibility = true;

        /// <summary>
        /// Скрыть описания номенклатуры.
        /// </summary>
        /// <param name="nomWrapper"></param>
        internal void CloseRowDetails(NomWrapper nomWrapper) => nomWrapper.IsRowDetailsVisibility = false;

        /// <summary>
        /// Добавить описание к номенклатуре.
        /// </summary>
        /// <param name="nomWrapper"></param>
        internal void AddDescriptionToNomWrapper(NomWrapper nomWrapper)
        {
            nomWrapper.Nomenclature.Descriptions.Add(new Description() { Text = "Новое описание" });
            OpenDescriptions(nomWrapper);
        }

        /// <summary>
        /// Добавить комментарий к номенклатуре.
        /// </summary>
        /// <param name="nomWrapper"></param>
        internal void AddCommentToNomWrapper(NomWrapper nomWrapper)
        {
            nomWrapper.Nomenclature.Descriptions.Add(new Description() { Text = "Комментарий", IsComment = true });
            OpenDescriptions(nomWrapper);
        }

        /// <summary>
        /// Поднять номенклатуру в списке.
        /// </summary>
        /// <param name="nomWrapper"></param>
        /// <param name="offerGroup"></param>
        internal void MoveUpNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup)
        {
            int index = offerGroup.NomWrappers.IndexOf(nomWrapper);
            if (index > 0)
            {
                offerGroup.NomWrappers.Remove(nomWrapper);
                offerGroup.NomWrappers.Insert(index - 1, nomWrapper);
            }
        }

        /// <summary>
        /// Опустить номенклатуру в списке.
        /// </summary>
        /// <param name="nomWrapper"></param>
        /// <param name="offerGroup"></param>
        internal void MoveDownNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup)
        {
            int index = offerGroup.NomWrappers.IndexOf(nomWrapper);
            int maxIndex = offerGroup.NomWrappers.Count() - 1;
            if (index < maxIndex)
            {
                offerGroup.NomWrappers.Remove(nomWrapper);
                offerGroup.NomWrappers.Insert(index + 1, nomWrapper);
            }
        }
    }
}
