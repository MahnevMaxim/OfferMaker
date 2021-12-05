using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using OfferMaker.Pdf;
using OfferMaker.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using OfferMaker.Pdf.Views;
using System.Windows.Controls;

namespace OfferMaker
{
    class WrapperOnePage
    {
        private BitmapImage image;
        object _context;
        private ShortCalculateBegin shortCalculateBegin = null;
        private ShortCalculateBody shortCalculateBody = null;
        bool shortCalculateStart = false;
        private DetailCalculateBegin DCB = null;
        private EmployeePDF employeePDF = null;
        private InformBlockPDF informBlockPDF = null;
        bool rewriting = true;
        bool shortCalculateRewriting = true;
        bool detailCalculateRewriting = true;
        bool shortCalculateOptionRewriting = false;
        bool detailCalculateOptionRewriting = false;
        bool lastNU = true;
        private double left, top, right, bottom;

        public WrapperOnePage(object context, BitmapImage image)
        {
            this.image = image;
            _context = context;
        }
        
        /// <summary>
        /// Функция возвращает страничный фиксированный документ
        /// </summary>
        public FixedDocument GetPdf(double Left, double Top, double Right, double Bottom)
        {
            left = Left; 
            top = Top; 
            right = Right; 
            bottom = Bottom;
            FixedDocument result = new FixedDocument();
            result.DocumentPaginator.PageSize = PrintLayout.A4.Size;

            List<PageContainer> p_container = GetPagesContainer();
            result = GetFixedDocument(p_container);
            return result;
        }

        List<PageContainer> GetPagesContainer()
        {
            List<PageContainer> p_container = new List<PageContainer>();
            
            int pageNumber = 0;
            int countNomenclatures = 0;
            
            BlockBody lastNomenclatureInGroup = null;
            BlockBegin nameGroup = null;
            BlockEnd lastSumm = null;
            
            #region OnePageTitul

            OnePageTitul titul = new OnePageTitul(_context);
            PageContainer pageContainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
            pageContainer.TryAddElement(titul);

            #endregion OnePageTitul

            #region DetailCalculateBegin

            List<PageContainer> detailBody_container = new List<PageContainer>();
            DetailCalculateBegin detailCalculateBegin = new DetailCalculateBegin(_context);
            pageContainer.TryAddElement(detailCalculateBegin);
            detailBody_container.Add(pageContainer);

            #endregion

            #region DetailBody

            PageContainer pageContainerGroup = detailBody_container.Last();//добавили в контейнер заголовок
            int countGrops = 0;
            ViewModels.MainViewModel vm = (ViewModels.MainViewModel)_context;
            //проходим по группам
            object lastGroup = null;
            foreach (var offerGroup in vm.OfferGroupsNotOptions)
            {
                countGrops++;
                //добавить название группы
                nameGroup = new BlockBegin(offerGroup);
                BlockBegin nameGroupCopy = new BlockBegin(offerGroup);
                if (offerGroup.NomWrappers.Count > 0) //если в группе есть хотя бы 1 номенклатура
                {
                    countNomenclatures = 0;
                    //добавить номенклатуры с описаниями
                    foreach (var nu in offerGroup.NomWrappers)
                    {
                        countNomenclatures++;
                        BlockBody nomenclature = new BlockBody(nu);
                        BlockBody nomenclatureCopy = (BlockBody)((IClonable)nomenclature).Copy();

                        //если это первая номенклатура
                        //пытаемся добавить на страницу название группы и номенклатуру вместе
                        if (nameGroupCopy != null)
                        {

                            pageContainerGroup.TryAddElement(nameGroup);
                            pageContainerGroup.TryAddElement(nomenclatureCopy);
                            if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                            {
                                detailBody_container.Last().RemoveElement(nomenclatureCopy);
                                detailBody_container.Last().RemoveElement(nameGroup);
                                pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                pageContainerGroup.TryAddElement(nameGroup);
                                pageContainerGroup.TryAddElement(nomenclatureCopy);
                                detailBody_container.Add(pageContainerGroup);
                                pageContainerGroup = detailBody_container.Last();

                            }
                            nameGroupCopy = null;
                        }
                        //не первая номенклатура
                        else
                        {
                            pageContainerGroup.TryAddElement(nomenclatureCopy);
                            if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                            {
                                detailBody_container.Last().RemoveElement(nomenclatureCopy);
                                //добавили старую страницу в контейнер

                                pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                pageContainerGroup.TryAddElement(nomenclatureCopy);
                                detailBody_container.Add(pageContainerGroup);
                                pageContainerGroup = detailBody_container.Last();
                            }

                            //если это первая и последняя
                            //переносим еще и название 
                        }
                        lastNomenclatureInGroup = nomenclatureCopy;
                    }
                    #region Сумма группы
                    BlockEnd summGroup = new BlockEnd(offerGroup);
                    BlockEnd summGroupCopy = (BlockEnd)((IClonable)summGroup).Copy();
                    if (detailBody_container.Count == 0)//если пройденные группы уместились на 1 странице
                    {
                        detailBody_container.Add(pageContainerGroup);//добавили страницу с группой дез итоговой стоимости группы
                        pageContainerGroup = detailBody_container.Last();
                    }

                    pageContainerGroup.TryAddElement(summGroupCopy);//добавили итоговую стоимость группы
                    if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())//если не поместилась
                    {

                        if (countNomenclatures == 1)
                        {
                            detailBody_container.Last().RemoveElement(summGroupCopy);
                            detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                            detailBody_container.Last().RemoveElement(nameGroup);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement(nameGroup);
                            pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                            pageContainerGroup.TryAddElement(summGroupCopy);
                            detailBody_container.Add(pageContainerGroup);
                        }
                        else
                        {
                            detailBody_container.Last().RemoveElement(summGroupCopy);
                            detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                            pageContainerGroup.TryAddElement(summGroupCopy);
                            detailBody_container.Add(pageContainerGroup);
                        }
                    }
                    //поместилась
                    else
                    {
                        detailBody_container.RemoveAt(detailBody_container.Count - 1);
                        detailBody_container.Add(pageContainerGroup);
                    }
                    lastSumm = summGroupCopy;
                    lastGroup = offerGroup;
                    #endregion
                }
            }

            #endregion DetailBody

            #region DetailCalculateEndOnePage

            DetailCalculateEndOnePage detailCalculateEndOnePage = new DetailCalculateEndOnePage(_context);
            if (detailBody_container.Count == 0) //зайдем, если вообще нет групп 
            {
                detailBody_container.Add(pageContainerGroup);
            }
            pageContainerGroup = detailBody_container.Last();
            pageContainerGroup.TryAddElement(detailCalculateEndOnePage);
            //если итоговая не поместилась
            if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
            {
                if (countNomenclatures == 1)
                {
                    detailBody_container.Last().RemoveElement(detailCalculateEndOnePage);
                    detailBody_container.Last().RemoveElement(lastSumm);
                    detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                    detailBody_container.Last().RemoveElement(nameGroup);
                    pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                    pageContainerGroup.TryAddElement(nameGroup);
                    pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                    pageContainerGroup.TryAddElement(lastSumm);
                    pageContainerGroup.TryAddElement(detailCalculateEndOnePage);
                    detailBody_container.Add(pageContainerGroup);
                }
                else
                {
                    detailBody_container.Last().RemoveElement(detailCalculateEndOnePage);
                    detailBody_container.Last().RemoveElement(lastSumm);
                    detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                    pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                    pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                    pageContainerGroup.TryAddElement(lastSumm);
                    pageContainerGroup.TryAddElement(detailCalculateEndOnePage);
                    detailBody_container.Add(pageContainerGroup);
                }
            }
            else
            {
                detailBody_container.RemoveAt(detailBody_container.Count - 1);
                detailBody_container.Add(pageContainerGroup);
            }
            foreach (PageContainer page in detailBody_container)
            {
                p_container.Add(page);//когда страница закончилась/ создаем новую страницу только если текущая закончилась или необходим перенос для установленной верстки
            }

            #endregion

            #region InformBlockPDF

            foreach (var offerInfoBlock in vm.OfferInfoBlocks)
            {
                InformBlockPDF informBlockPdf = new InformBlockPDF(offerInfoBlock);
                PageContainer page = p_container.Last();
                page.TryAddElement(informBlockPdf);
                if (page.container.ActualHeight + page.colontitul.ActualHeight > GetHeight() || pageNumber == 0)
                {
                    page.RemoveElement(informBlockPdf);
                    PageContainer newPagecontainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                    newPagecontainer.TryAddElement(informBlockPdf);
                    p_container.Add(newPagecontainer);
                }
            }

            #endregion InformBlockPDF

            #region Manager

            EmployeePDF employee = new EmployeePDF(_context);
            PageContainer page_ = p_container.Last();
            page_.TryAddElement(employee);
            if (page_.container.ActualHeight + page_.colontitul.ActualHeight > GetHeight())
            {
                page_.RemoveElement(employee);
                page_ = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                page_.TryAddElement(employee);
                p_container.Add(page_);
            }

            #endregion Manager

            return p_container;
        }

        double GetWidth() => PrintLayout.A4.Size.Width - ((left + right) * PrintLayout.A4.Size.Width / 21);
        
        double GetHeight() => PrintLayout.A4.Size.Height - ((top + bottom) * PrintLayout.A4.Size.Height / 29.7);

        FixedDocument GetFixedDocument(List<PageContainer> pagesContainer)
        {
            FixedDocument result = new FixedDocument();
            result.DocumentPaginator.PageSize = PrintLayout.A4.Size;
            foreach (PageContainer page in pagesContainer)
            {
                result.Pages.Add(GetPageContent(new List<UIElement>() { page }, Colontitul.WithNumeric));
            }
            return result;
        }

        enum Colontitul { None, WithNumeric, WithoutNumeric }

        PageContent GetPageContent(List<UIElement> elements, Colontitul colontitul)
        {
            PageContent result = new PageContent();
            FixedPage page = new FixedPage();
            page.Margin = new Thickness(left * PrintLayout.A4.Size.Width / 21,
                top * PrintLayout.A4.Size.Height / 29.7,
                right * PrintLayout.A4.Size.Width / 21,
                bottom * PrintLayout.A4.Size.Height / 29.7);

            int pNumber = ((PageContainer)elements[0]).pNumber;
            PageContainer pc = new PageContainer(GetWidth(), GetHeight(), pNumber, image, _context);

            foreach (UIElement element in elements)
            {
                if (element is IClonable)
                {
                    FrameworkElement el = ((IClonable)element).Copy();
                    if (elements.Count == 1)
                    {
                        el.Width = PrintLayout.A4.Size.Width - page.Margin.Left - page.Margin.Right;
                        el.Height = PrintLayout.A4.Size.Height - page.Margin.Top - page.Margin.Bottom;
                        page.Children.Add(el);
                        ((IAddChild)result).AddChild(page);
                        return result;
                    }
                    else
                    {
                        pc.TryAddElement((UserControl)el);
                    }
                }
                else
                {
                    pc.TryAddElement((UserControl)element);
                }
            }
            page.Children.Add(pc);
            ((IAddChild)result).AddChild(page);
            return result;
        }
    }
}
