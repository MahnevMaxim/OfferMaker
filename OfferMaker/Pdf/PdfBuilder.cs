using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OfferMaker.Controls;
using OfferMaker.Pdf.Views;
using System.Windows;
using System.Windows.Controls;

namespace OfferMaker.Pdf
{
    class PdfBuilder
    {
        public List<PageContainer> allContainers = new List<PageContainer>();
        private object context;
        double width;
        double height;
        int pageNumber;
        private BitmapImage image;
        ViewModels.MainViewModel vm;
        double heightDifference = 90;

        public PdfBuilder(object context, BitmapImage image)
        {
            this.image = image;
            this.context = context;
            width = PrintLayout.A4.Size.Width;
            height = PrintLayout.A4.Size.Height;
            vm = (ViewModels.MainViewModel)context;
        }

        #region TitulView

        internal void AddTitulView(bool isBannerVisibility)
        {
            TitulView titul = new TitulView(context, isBannerVisibility);
            PageContainer pageContainerTitul = new PageContainer(width, height, ++pageNumber, image, context);
            pageContainerTitul.TryAddElement(titul);
            pageContainerTitul.PageStatus = isBannerVisibility ? PageStatus.Close : PageStatus.Open;
            allContainers.Add(pageContainerTitul);
        }

        #endregion TitulView

        #region Рекламные баннеры

        internal void AddAdvertisings()
        {
            foreach (var item in vm.AdvertisingsUp)
            {
                PageContainer pageContainerAd = new PageContainer(width, height, -1, image, context);
                AdView adBlock = new AdView(item);
                pageContainerAd.TryAddAdvertisingElement(adBlock);
                pageContainerAd.PageStatus = PageStatus.Close;
                allContainers.Add(pageContainerAd);
            }
        }

        #endregion Рекламные баннеры

        #region Краткий расчёт

        internal void AddShortCalcs()
        {
            if (vm.OfferGroupsNotOptions.Count == 0) return;

            ShortCalculateBegin shortCalculateBegin = new ShortCalculateBegin(context);
            AddElement(shortCalculateBegin);

            foreach (var offerGroup in vm.OfferGroupsNotOptions)
            {
                AddOfferGroup(offerGroup);
            }

            ShortCalculateEnd shortCalculateEnd = new ShortCalculateEnd(vm);
            AddElement(shortCalculateEnd);
        }

        private void AddOfferGroup(OfferGroup offerGroup)
        {
            ShortCalculateBody shortBody = new ShortCalculateBody(offerGroup);
            AddElement(shortBody);
        }

        #endregion Краткий расчёт

        #region Краткий расчёт опций

        internal void AddShortCalcsOption()
        {
            if (vm.OfferGroupsOptions.Count == 0) return;

            ShortCalculateOptionBegin shortCalculateOptionBegin = new ShortCalculateOptionBegin(vm);
            AddElement(shortCalculateOptionBegin);

            foreach (var offerGroup in vm.OfferGroupsOptions)
            {
                AddOfferGroup(offerGroup);
            }

            ShortCalculateOptionEnd shortCalculateOptionEnd = new ShortCalculateOptionEnd(vm);
            AddElement(shortCalculateOptionEnd);
        }



        #endregion Краткий расчёт опций

        #region InformBlockPDF

        internal void AddInformBlockPdf()
        {
            InformBlocks ib = new InformBlocks();
            foreach (var offerInfoBlock in vm.OfferInfoBlocks)
            {
                InformBlockPDF informBlock = new InformBlockPDF(offerInfoBlock);
                ib.panel.Children.Add(informBlock);
            }
            AddElement(ib);
        }

        #endregion InformBlockPDF

        #region EmployeePDF

        internal void AddEmployee()
        {
            EmployeePDF employee = new EmployeePDF(vm);
            AddElement(employee);
        }

        #endregion EmployeePDF

        #region Расчёт стоимости

        internal void AddCalc()
        {
            if (vm.OfferGroupsNotOptions.Count == 0) return;

            // У нас несколько условий для форматирования:
            // - Заголовок расчёта должен быть на одной странице с первой номенклатурой первой группы
            // - Заголовок группы должен быть на одной странице с первой номенклатурой группы
            // - Сумма группы должна быть на одной странице с последней номенклатурой группы
            // - Итоговая стоимость КП должна быть на одной странице с последней номенклатурой КП
            // - на основании условий создаём список списков неразделимых контролов, а уже потом их добавляем

            List<UserControl> controls = new List<UserControl>();
            DetailCalculateBegin detailCalculateBegin = new DetailCalculateBegin(vm);
            controls.Add(detailCalculateBegin);

            foreach (var offerGroup in vm.OfferGroupsNotOptions)
            {
                if (!offerGroup.IsContainsNoms) continue;
                BlockBegin nameGroup = new BlockBegin(offerGroup);
                controls.Add(nameGroup);

                foreach (var nom in offerGroup.NomWrappers)
                {
                    BlockBody nomenclatureBody = new BlockBody(nom);
                    controls.Add(nomenclatureBody);
                }

                BlockEnd summGroup = new BlockEnd(offerGroup);
                controls.Add(summGroup);
            }

            DetailCalculateEnd summDetailCalculate = new DetailCalculateEnd(vm);
            controls.Add(summDetailCalculate);

            List<List<UserControl>> result = GetAtomics(controls);
            foreach(var els in result)
            {
                AddElements(els);
            }
        }

        #endregion Расчёт стоимости

        #region Расчёт опций

        internal void AddCalcOptions()
        {
            if (vm.OfferGroupsOptions.Count == 0) return;

            List<UserControl> controls = new List<UserControl>();
            DetailCalculateBeginOption detailCalculateOptionBegin = new DetailCalculateBeginOption(vm);
            controls.Add(detailCalculateOptionBegin);

            foreach (var offerGroup in vm.OfferGroupsOptions)
            {
                if (!offerGroup.IsContainsNoms) continue;

                BlockBeginOption nameGroupOption = new BlockBeginOption(offerGroup);
                controls.Add(nameGroupOption);

                foreach (var nu in offerGroup.NomWrappers)
                {
                    BlockBodyOption nomenclatureBody = new BlockBodyOption(nu);
                    controls.Add(nomenclatureBody);
                }

                BlockEndOption summGroup = new BlockEndOption(offerGroup);
                controls.Add(summGroup);
            }

            DetailCalculateEndOption summDetailCalculate = new DetailCalculateEndOption(vm);
            controls.Add(summDetailCalculate);

            List<List<UserControl>> result = GetAtomics(controls);
            foreach (var els in result)
            {
                AddElements(els);
            }
        }

        #endregion Расчёт опций

        #region adv2

        internal void AddAdvertisingsDown()
        {
            foreach (var item in vm.AdvertisingsDown)
            {
                PageContainer pageContainerAd = new PageContainer(width, height, -1, image, context);
                AdView adBlock = new AdView(item);
                pageContainerAd.TryAddAdvertisingElement(adBlock);
                pageContainerAd.PageStatus = PageStatus.Close;
                allContainers.Add(pageContainerAd);
            }
        }

        #endregion adv2

        /// <summary>
        /// Возвращает коллекцию "неделимых" (которые надо разместить на 1-ой странице) списков контролов.
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        private List<List<UserControl>> GetAtomics(List<UserControl> controls)
        {
            List<List<UserControl>> result = new List<List<UserControl>>();
            List<UserControl> first = new List<UserControl>(controls.Take(3)); //первые 3 всегда: название КП, название группы и тело первой номенклатуры - вместе
            result.Add(first);
            for (int i = 3; i < controls.Count; i++)
            {
                var ctrl = controls[i];
                var lastAddedCtrl = controls[i - 1];
                string controlName = ctrl.GetType().Name;
                if (controlName == "BlockBody" || controlName == "BlockBodyOption")
                {
                    if (lastAddedCtrl.GetType().Name == "BlockBegin" || lastAddedCtrl.GetType().Name == "BlockBeginOption")
                        result.Last().Add(ctrl);
                    else
                        result.Add(new List<UserControl>() { ctrl });
                }
                else if (controlName == "BlockEnd" || controlName == "DetailCalculateEnd" || controlName == "BlockEndOption" || controlName == "DetailCalculateEndOption")
                {
                    result.Last().Add(ctrl);
                }
                else if (controlName == "BlockBegin" || controlName == "BlockBeginOption")
                {
                    result.Add(new List<UserControl>() { ctrl });
                }
            }
            return result;
        }

        /// <summary>
        /// Добавление элемента.
        /// </summary>
        /// <param name="element"></param>
        void AddElement(UserControl element)
        {
            bool isNewPage = false;
            if (allContainers.Last().PageStatus == PageStatus.Close)
            {
                allContainers.Add(new PageContainer(width, height, ++pageNumber, image, context));
                isNewPage = true;
            }

            PageContainer page = allContainers.Last();
            page.TryAddElement(element);
            if (page.container.ActualHeight + page.colontitul.ActualHeight > (height - heightDifference))
            {
                if (!isNewPage)
                {
                    page.RemoveElement(element);
                    page.PageStatus = PageStatus.Close;
                    PageContainer pageNew = new PageContainer(width, height, ++pageNumber, image, context);
                    allContainers.Add(pageNew);
                    pageNew.TryAddElement(element);

                }
                page.PageStatus = PageStatus.Close;
            }
        }

        /// <summary>
        /// Добавление списка елементов
        /// </summary>
        /// <param name="els"></param>
        private void AddElements(List<UserControl> els)
        {
            bool isNewPage = false;
            if (allContainers.Last().PageStatus == PageStatus.Close)
            {
                allContainers.Add(new PageContainer(width, height, ++pageNumber, image, context));
                isNewPage = true;
            }

            PageContainer page = allContainers.Last();
            page.AddElements(els);
            if (page.container.ActualHeight + page.colontitul.ActualHeight > (height - heightDifference))
            {
                if (!isNewPage)
                {
                    page.RemoveElements(els);
                    page.PageStatus = PageStatus.Close;
                    PageContainer pageNew = new PageContainer(width, height, ++pageNumber, image, context);
                    allContainers.Add(pageNew);
                    pageNew.AddElements(els);

                }
                page.PageStatus = PageStatus.Close;
            }
        }
    }
}
