using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Shared;
using System.Threading;
using System.IO;

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
        FixedDocument pdfDocumentShort;
        string photoLogo;
        string photoNumber;
        string photoCustomer;
        string photoAdress;
        string photoNumberTeh;
        int pdfControlSelectedIndex;
        string offerStatus;

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
                if (pdfControlSelectedIndex != 0)
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

        /// <summary>
        /// Сокращённый документ для предпросмотра.
        /// </summary>
        public FixedDocument PdfDocumentShort
        {
            get => pdfDocumentShort;
            set
            {
                pdfDocumentShort = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Статус документа: новый, шаблон или архив
        /// </summary>
        public string OfferStatus
        {
            get
            {
                if(Offer.IsArchive)
                    return "архив Id " + Offer.Id;
                if (Offer.IsTemplate)
                {
                    if(Offer.Id==0)
                        return "шаблон";
                    else
                        return "шаблон Id " + Offer.Id;
                }
                else
                    return "новый";
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

        public ObservableCollection<Currency> Currencies
        {
            get
            {
                if (Offer.IsArchive)
                    return Offer.Currencies;
                else
                    return Global.Currencies;
            }
        }

        #region Singleton

        private Constructor()
        {
            Uri uri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\logosmall.png"), UriKind.Absolute);
            smallLogo = new BitmapImage(uri);
            PhotoLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\logo.png");
            PhotoNumber = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\number.png");
            PhotoCustomer = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\customer.png");
            PhotoAdress = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\address.png");
            PhotoNumberTeh = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\common_images\\telephone.png");
            InitNewOffer();
        }

        private static readonly Constructor instance = new Constructor();

        public static Constructor GetInstance() => instance;

        #endregion Singleton

        #region Offer

        /// <summary>
        /// Создание нового КП.
        /// </summary>
        private void InitNewOffer()
        {
            ObservableCollection<OfferInfoBlock> offerInfoBlocks = GetInfoBlocks();
            Currency currency = Global.GetRub();
            Offer = new Offer(this, currency, offerInfoBlocks, Global.Main.User, Settings.GetDefaultBannerGuid());
        }

        /// <summary>
        /// Загрузка КП из архива.
        /// </summary>
        /// <param name="offer"></param>
        internal void LoadOfferFromArchive(Offer offer)
        {
            Offer clone = Helpers.CloneObject<Offer>(offer);
            Offer offer_ = Utils.RestoreOffer(clone, Global.Users, true);
            LoadOffer(offer_);
        }
            
        /// <summary>
        /// Загрузка шаблона.
        /// </summary>
        /// <param name="offer"></param>
        internal void LoadOfferTemplate(Offer offer)
        {
            Offer clone = Helpers.CloneObject<Offer>(offer);
            Offer offer_ = Utils.RestoreOffer(clone, Global.Users, false);

            offer_.Id = 0;
            offer_.Guid = Guid.NewGuid().ToString();
            offer_.Manager = Global.Main.User;
            offer_.OfferCreator = Global.Main.User;
            LoadOffer(offer_);
        }

        /// <summary>
        /// Редактирование шаблона.
        /// </summary>
        /// <param name="offer"></param>
        internal void EditOfferTemplate(Offer offer)
        {
            offer.Manager = Global.Main.User;
            offer.OfferCreator = Global.Main.User;
            LoadOffer(offer);
            offer.SetEditableState();
        }

        /// <summary>
        /// Загрузка любой хуйни.
        /// </summary>
        /// <param name="offer"></param>
        internal void LoadOffer(Offer offer)
        {
            Offer = offer;
            offer.Discount.SetOffer(offer);
            Offer.SetConstructor(this);
            viewModel.OnPropertyChanged("OfferInfoBlocks");
            viewModel.OnPropertyChanged("Manager");
        }

        /// <summary>
        /// Test
        /// </summary>
        internal void SkipOffer()
        {
            ObservableCollection<OfferInfoBlock> offerInfoBlocks = GetInfoBlocks();
            Currency currency = Global.GetRub();
            Offer = new Offer(this, currency, offerInfoBlocks, Global.Main.User, Settings.GetDefaultBannerGuid());
            Offer.OnPropertyChanged(String.Empty);
            viewModel.OnPropertyChanged(String.Empty);
        }

        #endregion Offer

        #region Create PDF

        bool isCreatorBusy;
        bool isNeedUpdate;
        /// <summary>
        /// Обновление документа КП.
        /// </summary>
        async public Task CreatePdf()
        {
            isNeedUpdate = true;
            if (isCreatorBusy) return;
            if (pdfControlSelectedIndex == 0) return;

            isCreatorBusy = true;
            while (isNeedUpdate)
            {
                isNeedUpdate = false;
                try
                {
                    if (pdfControlSelectedIndex == 1)
                        CreateDocumentWithBanner();
                    else if (pdfControlSelectedIndex == 2)
                        CreateDocumentWithoutBanner();
                    await Task.Delay(4000);
                }
                catch (Exception ex)
                {
                    await Task.Delay(4000);
                    Log.Write(ex);
                }
            }
            isCreatorBusy = false;
        }

        public void CreateDocumentWithBanner()
        {
            FlowDocument flowDocument = ((Views.MainWindow)viewModel.view).pdfControl.flowDocumentAll;
            WrapperAllPages wrapper = new WrapperAllPages(flowDocument, viewModel, smallLogo);
            PdfDocument = wrapper.GetPdf(2, 1, 1, 1);
        }

        public void CreateDocumentWithoutBanner()
        {
            WrapperOnePage wrapper = new WrapperOnePage(viewModel, smallLogo);
            PdfDocumentShort = wrapper.GetPdf(2, 1, 1, 1);
        }

        #endregion Create PDF

        #region InformBlocks

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
                ImagePath = Path.Combine("Images\\informIcons\\Commertial5.png"),
                IsCustom = true
            };
            Offer.OfferInfoBlocks.Add(block);
            return new CallResult();
        }

        /// <summary>
        /// Редактирование копии номенклатуры.
        /// </summary>
        /// <param name="nomWrapper"></param>
        internal void OpenCardNomWrapper(NomWrapper nomWrapper)
        {
            Nomenclature source = Helpers.CloneObject<Nomenclature>(nomWrapper.Nomenclature);
            MvvmFactory.CreateWindow(new NomenclurueCard(nomWrapper), new ViewModels.NomenclatureCardViewModel(), new Views.NomenclatureCard(), ViewMode.ShowDialog);
            if(!source.IsEqual(nomWrapper.Nomenclature))
            {
                nomWrapper.UpdateCurrency();
                nomWrapper.OnPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Удалить кастомный информ блок.
        /// </summary>
        /// <param name="offerInfoBlock"></param>
        internal void RemoveInformBlock(OfferInfoBlock offerInfoBlock) => Offer.OfferInfoBlocks.Remove(offerInfoBlock);

        /// <summary>
        /// Инициализация инфоблоков.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<OfferInfoBlock> GetInfoBlocks()
        {
            return new ObservableCollection<OfferInfoBlock>()
            {
                new OfferInfoBlock(){
                    Title = "Срок готовности товара к отгрузке",
                    Text ="до 30 рабочих дней",
                    ImagePath="Images\\informIcons\\Commertial1.png"
                },
                new OfferInfoBlock(){
                    Title = "Срок проведения монтажных и пусконаладочных работ",
                    Text ="до 12 рабочих дней",
                    ImagePath= "Images\\informIcons\\Commertial2.png"
                },
                new OfferInfoBlock(){
                    Title = "Условие оплаты",
                    Text ="50% - аванс, 40% - после извещения о готовности товара к отгрузке, " +
                    "5% - после поступления товара на склад Покупателя, " +
                    "оставшиеся 5 % -в трехдневный срок после подписания акта проведения ШМР и ПНР",
                    ImagePath= "Images\\informIcons\\Commertial3.png"
                },
                new OfferInfoBlock(){
                    Title = "Условия поставки",
                    Text ="Доставка осуществляется за счет Покупателя",
                    ImagePath="Images\\informIcons\\Commertial4.png"
                }
            };
        }

        #endregion InformBlocks

        #region Discount

        /// <summary>
        /// Включение скидки.
        /// </summary>
        /// <returns></returns>
        internal CallResult SetDiscount()
        {
            if(Offer.Discount.DiscountSum>0)
            {
                Offer.Discount.IsEnabled = true;
                return new CallResult();
            }
            else
            {
                return new CallResult() { Error=new Error("Сумма скидки должна быть больше нуля.")};
            }
        }

        /// <summary>
        /// Выключение скидки.
        /// </summary>
        internal void CancelDiscount()
        {
            Offer.Discount.IsEnabled = false;
            Offer.Discount.Percentage = 0;
        }

        #endregion Discount

        #region OfferGroups

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
        /// Добавление новой группы в конструктор.
        /// </summary>
        internal void AddOfferGroup() => Offer.OfferGroups.Add(new OfferGroup(Offer) { GroupTitle = "ГРУППА " + Offer.GetAddGroupsCounter() });

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

        #endregion OfferGroups

        #region Descriptions

        /// <summary>
        /// Удаление описания из номенклатуры из обертки номенклатуры для группы номенклатур в конструкторе.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nomWrapper"></param>
        internal void DeleteDescriptionFromNomWrapper(Description description, NomWrapper nomWrapper) => nomWrapper.Nomenclature.Descriptions.Remove(description);

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
            nomWrapper.Nomenclature.Descriptions.Add(new Description() { Text = "Новое описание", IsEnabled = true });
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

        #endregion Descriptions

        #region NomWrappers

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

        /// <summary>
        /// Удаление номенклатуры из группы в конструкторе
        /// </summary>
        /// <param name="nomWrapper"></param>
        /// <param name="offerGroup"></param>
        internal void DeleteNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => offerGroup.NomWrappers.Remove(nomWrapper);

        #endregion NomWrappers

        #region Etc

        /// <summary>
        /// Редактирование данных клиента и, по стечению обстоятельств, названия КП.
        /// </summary>
        internal void EditCustomer() => new SimpleViews.EditCustomer(Offer).ShowDialog();

        /// <summary>
        /// Окно выбора баннера и рекламмы.
        /// </summary>
        internal void OpenBanners()
        {
            MvvmFactory.CreateWindow(Global.Main.BannersManager, new ViewModels.BannersManagerViewModel(), new Views.BannersManager(), ViewMode.ShowDialog);
            UpdateBanners();
            
        }

        private void UpdateBanners()
        {
            if (Global.Main.BannersManager.SelectedBanner != null &&
                Global.Main.BannersManager.SelectedBanner.LocalPhotoPath != Offer.BannerImagePath)
            {
                Offer.Banner_ = Global.Main.BannersManager.SelectedBanner;
                viewModel.OnPropertyChanged("BannerImagePath");
            }

            if(IsChangeAdvertisings(Offer.AdvertisingsUp, Global.Main.BannersManager.AdvertisingsUp))
            {
                Offer.AdvertisingsUp = GetPathCollection(Global.Main.BannersManager.AdvertisingsUp);
                viewModel.OnPropertyChanged("AdvertisingsUp");
            }

            if (IsChangeAdvertisings(Offer.AdvertisingsDown, Global.Main.BannersManager.AdvertisingsDown))
            {
                Offer.AdvertisingsDown = GetPathCollection(Global.Main.BannersManager.AdvertisingsDown);
                viewModel.OnPropertyChanged("AdvertisingsDown");
            }
        }

        private ObservableCollection<string> GetPathCollection(ObservableCollection<IImage> advertisingsUp)
        {
            var res = advertisingsUp.Select(i=>i.LocalPhotoPath);
            return new ObservableCollection<string>(res);
        }

        private bool IsChangeAdvertisings(ObservableCollection<string> advertisings, ObservableCollection<IImage> advertisings_)
        {
            if (advertisings.Count != advertisings_.Count)
                return true;
            for(int i=0;i<advertisings.Count;i++)
            {
                if (advertisings[i] != advertisings_[i].LocalPhotoPath)
                    return true;
            }
            return false;
        }

        #endregion Etc
    }
}
