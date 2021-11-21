using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace OfferMaker
{
    /// <summary>
    /// Центральный класс приложения.
    /// Содержит саму сущность КП в свойстве Offer и методы для её изменения и обработки.
    /// </summary>
    public class Constructor : BaseModel
    {
        FixedDocument pdfDocument;
        string photoLogo;

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

        #region Singleton

        private Constructor()
        {
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

        private void CreatePdf()
        {
            FlowDocument flowDocument = ((Views.MainWindow)viewModel.view).pdfControl.flowDocumentAll;
            WrapperAllPages wrapper = new WrapperAllPages(flowDocument, viewModel);
            FixedDocument fd1 = wrapper.GetPdf(2, 1, 1, 1);
            PdfDocument = fd1;
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
    }
}
