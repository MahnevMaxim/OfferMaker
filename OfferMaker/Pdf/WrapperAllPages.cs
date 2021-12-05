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
    class WrapperAllPages
    {
        private FlowDocument sourceDocument;
        private BitmapImage image;
        object _context;
        private double left, top, right, bottom;

        private ShortCalculateBegin shortCalculateBegin = null;
        private ShortCalculateOptionBegin shortCalculateOptionBegin = null;
        private EmployeePDF employeePDF = null;
        private InformBlockPDF informBlockPDF = null;
        private DetailCalculateBegin detailCalculateBegin = null;
        private DetailCalculateBeginOption detailCalculateOptionBegin = null;

        public WrapperAllPages(FlowDocument flowDocumentAll, object context, BitmapImage image)
        {
            this.image = image;
            sourceDocument = flowDocumentAll;
            _context = context;
        }

        internal FixedDocument GetPdf(double Left, double Top, double Right, double Bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
            FixedDocument result = new FixedDocument();
            result.DocumentPaginator.PageSize = PrintLayout.A4.Size;

            List<PageContainer> p_container = GetPagesContainer(sourceDocument.Blocks);
            result = GetFixedDocument(p_container);
            return result;
        }

        List<PageContainer> GetPagesContainer(BlockCollection blocks)
        {
            List<PageContainer> p_container = new List<PageContainer>();
            List<PageContainer> ad_container = new List<PageContainer>();
            List<PageContainer> ad_container2 = new List<PageContainer>();
            List<PageContainer> shortBody_container = new List<PageContainer>();
            List<PageContainer> shortBodyOption_container = new List<PageContainer>();
            List<PageContainer> detailBody_container = new List<PageContainer>();
            List<PageContainer> detailOptionBody_container = new List<PageContainer>();

            int pageNumber = 0;
            int countNomenclatures = 0;
            ShortCalculateBody lastShortCalculateElement = null;
            ShortCalculateOptionBody lastShortCalculateOptionElement = null;
            PageContainer pageContainerShort = null;
            PageContainer pageContainerOptionShort = null;
            PageContainer pageContainerGroup = null;
            PageContainer pageContainer = new PageContainer(GetWidth(), GetHeight(), pageNumber, image, _context); //создаем одинн контейнер на весь документ

            BlockBegin nameGroup = null;
            BlockBody lastNomenclatureInGroup = null;
            BlockEnd lastSumm = null;
            BlockBeginOption nameGroupOption = null;
            BlockBodyOption lastNomenclatureInGroupOption = null;
            BlockEndOption lastSummOption = null;

            object lastGroup = null;
            foreach (Block block in blocks)
            {
                if (!(block is Paragraph)) continue;
                if (!(((Paragraph)block).Inlines.FirstInline is InlineUIContainer)) continue;

                InlineUIContainer container = (InlineUIContainer)((Paragraph)block).Inlines.FirstInline;

                #region TitulView

                //титульник
                if (container.Child is TitulView)
                {
                    pageContainer.TryAddElement(((IClonable)(UserControl)container.Child).Copy());
                    p_container.Add(pageContainer); //когда страница закончилась
                    continue;
                }

                #endregion

                #region adv

                //рекламные баннеры
                if (block.Name == "adv")
                {
                    PageContainer pageContainerAd = new PageContainer(GetWidth(), GetHeight(), -1, image, _context);
                    foreach (var item in ((ItemsControl)container.Child).Items)
                    {
                        AdView adBlock = new AdView(item);
                        AdView adBlock2 = (AdView)((IClonable)adBlock).Copy();
                        if (adBlock2 != null)
                        {
                            pageContainerAd.TryAddElement(adBlock2);
                            ad_container.Add(pageContainerAd);
                            pageContainerAd = new PageContainer(GetWidth(), GetHeight(), -1, image, _context);
                        }
                    }

                    foreach (PageContainer page in ad_container)
                    {
                        p_container.Add(page);
                    }
                    continue;
                }

                #endregion

                #region ShortCalculateBegin

                //КРАТКИЙ РС
                //краткий расчет НАЧАЛО
                if (container.Child is ShortCalculateBegin)
                {
                    shortCalculateBegin = (ShortCalculateBegin)container.Child;
                    if (shortCalculateBegin.Visibility == Visibility.Visible)
                    {
                        pageContainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);//начинаем с новой страницы
                        pageContainer.TryAddElement(((IClonable)(UserControl)container.Child).Copy());
                    }
                    continue;
                }

                #endregion

                #region ShortBody

                if (block.Name == "ShortBody")
                {
                    if (shortCalculateBegin.Visibility != Visibility.Visible) continue;
                    pageContainerShort = pageContainer;
                    foreach (var item in ((ItemsControl)container.Child).Items)
                    {
                        ShortCalculateBody shortBody = new ShortCalculateBody(item);
                        ShortCalculateBody shortBodyCopy = (ShortCalculateBody)((IClonable)shortBody).Copy();
                        if (shortBodyCopy != null)
                        {
                            //добавили в pageContainerShort позицию
                            pageContainerShort.TryAddElement(shortBodyCopy);
                            //если она не поместилась
                            if (pageContainerShort.container.ActualHeight + pageContainerShort.colontitul.ActualHeight > GetHeight())
                            {
                                //удалили ее
                                pageContainerShort.RemoveElement(shortBodyCopy);
                                //добавили старую страницу в контейнер
                                shortBody_container.Add(pageContainerShort);
                                //создали новую страницу
                                pageContainerShort = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                //добавили позицию на новую 
                                pageContainerShort.TryAddElement(shortBodyCopy);
                            }
                            lastShortCalculateElement = shortBodyCopy;
                        }
                    } //прошли все позиции 

                    shortBody_container.Add(pageContainerShort);
                    continue;
                }

                #endregion

                #region ShortCalculateEnd

                if (container.Child is ShortCalculateEnd)
                {
                    if (shortCalculateBegin.Visibility != Visibility.Visible) continue;

                    UserControl a = ((IClonable)(UserControl)container.Child).Copy();
                    pageContainerShort.TryAddElement(a);
                    //если итоговая не поместилась
                    if (pageContainerShort.container.ActualHeight + pageContainerShort.colontitul.ActualHeight > GetHeight())
                    {
                        pageContainerShort.RemoveElement(a);
                        shortBody_container.Last().RemoveElement(lastShortCalculateElement);
                        //создали новую страницу
                        pageContainerShort = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                        //добавили позицию на новую 
                        pageContainerShort.TryAddElement((ShortCalculateBody)((IClonable)lastShortCalculateElement).Copy());
                        pageContainerShort.TryAddElement(a);
                        shortBody_container.Add(pageContainerShort);
                    }
                    foreach (PageContainer page in shortBody_container)
                    {
                        p_container.Add(page);//когда страница закончилась/ создаем новую страницу только если текущая закончилась или необходим перенос для установленной верстки
                    }
                    shortBody_container.Clear();
                    continue;
                }

                #endregion

                #region ShortCalculateOptionBegin

                if (container.Child is ShortCalculateOptionBegin)
                {
                    shortCalculateOptionBegin = (ShortCalculateOptionBegin)container.Child;
                    if (shortCalculateOptionBegin.Visibility != Visibility.Visible) continue;

                    if (shortCalculateBegin.Visibility != Visibility.Visible)
                    {
                        pageContainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);//начинаем с новой страницы
                        pageContainer.TryAddElement(((IClonable)(System.Windows.Controls.UserControl)container.Child).Copy());
                    }
                    else
                    {
                        pageContainer = p_container.Last();
                        p_container.RemoveAt(p_container.Count - 1);
                        pageContainer.TryAddElement(((IClonable)(System.Windows.Controls.UserControl)container.Child).Copy());
                    }
                    continue;
                }

                #endregion

                #region ShortOptionBody

                if (block.Name == "ShortOptionBody")
                {
                    if (shortCalculateOptionBegin.Visibility != Visibility.Visible) continue;
                    pageContainerOptionShort = pageContainer;
                    foreach (var offerGroup_ in ((ItemsControl)container.Child).Items)
                    {
                        OfferGroup offerGroup = (OfferGroup)offerGroup_;
                        ShortCalculateOptionBody shortBody = new ShortCalculateOptionBody(offerGroup);
                        ShortCalculateOptionBody shortBodyCopy = (ShortCalculateOptionBody)((IClonable)shortBody).Copy();
                        if (shortBodyCopy != null)
                        {
                            //добавили в pageContainerShort позицию
                            pageContainerOptionShort.TryAddElement(shortBodyCopy);
                            //если она не поместилась
                            if (pageContainerOptionShort.container.ActualHeight + pageContainerOptionShort.colontitul.ActualHeight > GetHeight())
                            {
                                //удалили ее
                                pageContainerOptionShort.RemoveElement(shortBodyCopy);
                                //добавили старую страницу в контейнер
                                shortBodyOption_container.Add(pageContainerOptionShort);
                                //создали новую страницу
                                pageContainerOptionShort = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                //добавили позицию на новую 
                                pageContainerOptionShort.TryAddElement(shortBodyCopy);
                            }
                            lastShortCalculateOptionElement = shortBodyCopy;
                        }
                    } //прошли все позиции 

                    shortBodyOption_container.Add(pageContainerOptionShort);
                    continue;
                }

                #endregion

                #region ShortCalculateOptionEnd

                if (container.Child is ShortCalculateOptionEnd)
                {
                    if (shortCalculateOptionBegin.Visibility != Visibility.Visible) continue;
                    UserControl a = ((IClonable)(UserControl)container.Child).Copy();
                    pageContainerOptionShort.TryAddElement(a);
                    //если итоговая не поместилась
                    if (pageContainerOptionShort.container.ActualHeight + pageContainerOptionShort.colontitul.ActualHeight > GetHeight())
                    {
                        pageContainerOptionShort.RemoveElement(a);
                        shortBodyOption_container.Last().RemoveElement(lastShortCalculateOptionElement);
                        //создали новую страницу
                        pageContainerOptionShort = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                        //добавили позицию на новую 
                        pageContainerOptionShort.TryAddElement((ShortCalculateBody)((IClonable)lastShortCalculateOptionElement).Copy());
                        pageContainerOptionShort.TryAddElement(a);
                        shortBodyOption_container.Add(pageContainerOptionShort);
                    }
                    foreach (PageContainer page in shortBodyOption_container)
                    {
                        p_container.Add(page);//когда страница закончилась/ создаем новую страницу только если текущая закончилась или необходим перенос для установленной верстки
                    }
                    shortBodyOption_container.Clear();
                    continue;
                }

                #endregion ShortCalculateOptionEnd

                #region InformBlockPDF

                if (block.Name.Contains("InformBlockPDF"))
                {

                    foreach (var offerInfoBlock_ in ((ItemsControl)container.Child).Items)
                    {
                        OfferInfoBlock offerInfoBlock = (OfferInfoBlock)offerInfoBlock_;
                        InformBlockPDF informBlockPdf = new InformBlockPDF(offerInfoBlock);

                        informBlockPDF = (InformBlockPDF)((IClonable)informBlockPdf).Copy();
                        if (informBlockPDF != null)
                        {
                            UserControl informBlock = informBlockPDF;
                            PageContainer page = p_container.Last();
                            page.TryAddElement(informBlock);
                            if (page.container.ActualHeight + page.colontitul.ActualHeight > GetHeight() || pageNumber==0)
                            {
                                page.RemoveElement(informBlock);
                                PageContainer newPagecontainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                newPagecontainer.TryAddElement(informBlock);
                                p_container.Add(newPagecontainer);
                            }
                        }
                    }
                    continue;
                }

                #endregion InformBlockPDF

                #region EmployeePDF

                if (container.Child is EmployeePDF)
                {
                    employeePDF = (EmployeePDF)((IClonable)container.Child).Copy();
                    if (employeePDF != null)
                    {
                        UserControl employee = employeePDF;

                        PageContainer page = p_container.Last();
                        page.TryAddElement(employee);
                        if (page.container.ActualHeight + page.colontitul.ActualHeight > GetHeight())
                        {
                            page.RemoveElement(employee);
                            page = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            page.TryAddElement(employee);
                            p_container.Add(page);
                        }

                    }
                    continue;
                }

                #endregion EmployeePDF

                #region DetailCalculateBegin

                if (container.Child is DetailCalculateBegin)
                {
                    detailCalculateBegin = (DetailCalculateBegin)container.Child;
                    if (detailCalculateBegin.Visibility != Visibility.Visible) continue;
                    pageContainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);//начинаем с новой страницы
                    pageContainer.TryAddElement(((IClonable)(UserControl)container.Child).Copy());
                    detailBody_container.Add(pageContainer);
                    continue;
                }

                #endregion

                #region DetailBody

                if (block.Name == "DetailBody")
                {
                    if (detailCalculateBegin.Visibility != Visibility.Visible) continue;

                    pageContainerGroup = detailBody_container.Last();//добавили в контейнер заголовок
                    int countGrops = 0;
                    //проходим по группам
                    foreach (var item in ((ItemsControl)container.Child).Items)
                    {
                        OfferGroup offerGroup = (OfferGroup)item;
                        countGrops++;
                        //добавить название группы
                        nameGroup = new BlockBegin(item);
                        BlockBegin nameGroupCopy = new BlockBegin(item);
                        if (!offerGroup.IsContainsNoms) continue;

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

                        BlockEnd summGroup = new BlockEnd(item);
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
                        lastGroup = item;

                        #endregion
                    }
                    continue;
                }

                #endregion

                #region DetailCalculateEnd

                //детальный расчет КОНЕЦ
                if (container.Child is DetailCalculateEnd)
                {
                    if (detailCalculateBegin.Visibility != Visibility.Visible) continue;

                    UserControl summDetailCalculate = ((IClonable)(UserControl)container.Child).Copy();
                    if (detailBody_container.Count == 0) //зайдем, если вообще нет групп 
                    {
                        detailBody_container.Add(pageContainerGroup);
                    }
                    pageContainerGroup = detailBody_container.Last();
                    pageContainerGroup.TryAddElement(summDetailCalculate);
                    //если итоговая не поместилась
                    if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                    {
                        if (countNomenclatures == 1)
                        {
                            detailBody_container.Last().RemoveElement(summDetailCalculate);
                            detailBody_container.Last().RemoveElement(lastSumm);
                            detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                            detailBody_container.Last().RemoveElement(nameGroup);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement(nameGroup);
                            pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                            pageContainerGroup.TryAddElement(lastSumm);
                            pageContainerGroup.TryAddElement(summDetailCalculate);
                            detailBody_container.Add(pageContainerGroup);
                        }
                        else
                        {
                            detailBody_container.Last().RemoveElement(summDetailCalculate);
                            detailBody_container.Last().RemoveElement(lastSumm);
                            detailBody_container.Last().RemoveElement(lastNomenclatureInGroup);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement((BlockBody)((IClonable)lastNomenclatureInGroup).Copy());
                            pageContainerGroup.TryAddElement(lastSumm);
                            pageContainerGroup.TryAddElement(summDetailCalculate);
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
                    continue;
                }

                #endregion DetailCalculateEnd

                #region DetailCalculateOptionBegin  //детальный расчет опций НАЧАЛО

                if (container.Child is DetailCalculateBeginOption)
                {
                    detailCalculateOptionBegin = (DetailCalculateBeginOption)container.Child;
                    if (detailCalculateOptionBegin.Visibility != Visibility.Visible) continue;
                    pageContainer = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);//начинаем с новой страницы
                    pageContainer.TryAddElement(((IClonable)(UserControl)container.Child).Copy());
                    detailOptionBody_container.Add(pageContainer);
                    continue;
                }

                #endregion

                #region DetailOptionBody

                if (block.Name == "DetailOptionBody")
                {
                    if (detailCalculateOptionBegin.Visibility != Visibility.Visible) continue;

                    pageContainerGroup = detailOptionBody_container.Last();//добавили в контейнер заголовок
                    int countGrops = 0;
                    //проходим по группам
                    foreach (var item in ((ItemsControl)container.Child).Items)
                    {
                        OfferGroup offerGroup = (OfferGroup)item;
                        countGrops++;
                        //добавить название группы
                        nameGroupOption = new BlockBeginOption(item);
                        BlockBeginOption nameGroupCopy = new BlockBeginOption(item);
                        if (offerGroup.NomWrappers.Count > 0) //если в группе есть хотя бы 1 номенклатура
                        {
                            countNomenclatures = 0;
                            //добавить номенклатуры с описаниями
                            foreach (var nu in offerGroup.NomWrappers)
                            {
                                countNomenclatures++;
                                BlockBodyOption nomenclature = new BlockBodyOption(nu);
                                BlockBodyOption nomenclatureCopy = (BlockBodyOption)((IClonable)nomenclature).Copy();

                                //если это первая номенклатура
                                //пытаемся добавить на страницу название группы и номенклатуру вместе
                                if (nameGroupCopy != null)
                                {
                                    pageContainerGroup.TryAddElement(nameGroupOption);
                                    pageContainerGroup.TryAddElement(nomenclatureCopy);
                                    if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                                    {
                                        detailOptionBody_container.Last().RemoveElement(nomenclatureCopy);
                                        detailOptionBody_container.Last().RemoveElement(nameGroupOption);
                                        pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                        pageContainerGroup.TryAddElement(nameGroupOption);
                                        pageContainerGroup.TryAddElement(nomenclatureCopy);
                                        detailOptionBody_container.Add(pageContainerGroup);
                                        pageContainerGroup = detailOptionBody_container.Last();
                                    }
                                    nameGroupCopy = null;
                                }
                                //не первая номенклатура
                                else
                                {
                                    pageContainerGroup.TryAddElement(nomenclatureCopy);
                                    if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                                    {
                                        detailOptionBody_container.Last().RemoveElement(nomenclatureCopy);
                                        //добавили старую страницу в контейнер

                                        pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                        pageContainerGroup.TryAddElement(nomenclatureCopy);
                                        detailOptionBody_container.Add(pageContainerGroup);
                                        pageContainerGroup = detailOptionBody_container.Last();
                                    }

                                    //если это первая и последняя
                                    //переносим еще и название 
                                }
                                lastNomenclatureInGroupOption = nomenclatureCopy;
                            }

                            #region Сумма группы

                            BlockEndOption summGroup = new BlockEndOption(item);
                            BlockEndOption summGroupCopy = (BlockEndOption)((IClonable)summGroup).Copy();
                            if (detailOptionBody_container.Count == 0)//если пройденные группы уместились на 1 странице
                            {
                                detailOptionBody_container.Add(pageContainerGroup);//добавили страницу с группой дез итоговой стоимости группы
                                pageContainerGroup = detailOptionBody_container.Last();
                            }

                            pageContainerGroup.TryAddElement(summGroupCopy);//добавили итоговую стоимость группы
                            if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())//если не поместилась
                            {
                                if (countNomenclatures == 1)
                                {
                                    detailOptionBody_container.Last().RemoveElement(summGroupCopy);
                                    detailOptionBody_container.Last().RemoveElement(lastNomenclatureInGroupOption);
                                    detailOptionBody_container.Last().RemoveElement(nameGroupOption);
                                    pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                    pageContainerGroup.TryAddElement(nameGroupOption);
                                    pageContainerGroup.TryAddElement((BlockBodyOption)((IClonable)lastNomenclatureInGroupOption).Copy());
                                    pageContainerGroup.TryAddElement(summGroupCopy);
                                    detailOptionBody_container.Add(pageContainerGroup);
                                }
                                else
                                {
                                    detailOptionBody_container.Last().RemoveElement(summGroupCopy);
                                    detailOptionBody_container.Last().RemoveElement(lastNomenclatureInGroupOption);
                                    pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                                    pageContainerGroup.TryAddElement((BlockBodyOption)((IClonable)lastNomenclatureInGroupOption).Copy());
                                    pageContainerGroup.TryAddElement(summGroupCopy);
                                    detailOptionBody_container.Add(pageContainerGroup);
                                }
                            }
                            //поместилась
                            else
                            {
                                detailOptionBody_container.RemoveAt(detailOptionBody_container.Count - 1);
                                detailOptionBody_container.Add(pageContainerGroup);
                            }
                            lastSummOption = summGroupCopy;
                            lastGroup = item;

                            #endregion
                        }
                    }
                }

                #endregion DetailOptionBody

                #region DetailCalculateEndOption

                if (container.Child is DetailCalculateEndOption)
                {
                    //детальный расчет опций КОНЕЦ
                    if (detailCalculateOptionBegin.Visibility != Visibility.Visible) continue;

                    UserControl summDetailCalculate = ((IClonable)(UserControl)container.Child).Copy();
                    if (detailOptionBody_container.Count == 0) //зайдем, если вообще нет групп 
                    {
                        detailOptionBody_container.Add(pageContainerGroup);
                    }
                    pageContainerGroup = detailOptionBody_container.Last();
                    pageContainerGroup.TryAddElement(summDetailCalculate);
                    //если итоговая не поместилась
                    if (pageContainerGroup.container.ActualHeight + pageContainerGroup.colontitul.ActualHeight > GetHeight())
                    {
                        if (countNomenclatures == 1)
                        {
                            detailOptionBody_container.Last().RemoveElement(summDetailCalculate);
                            detailOptionBody_container.Last().RemoveElement(lastSummOption);
                            detailOptionBody_container.Last().RemoveElement(lastNomenclatureInGroupOption);
                            detailOptionBody_container.Last().RemoveElement(nameGroupOption);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement(nameGroupOption);
                            pageContainerGroup.TryAddElement((BlockBodyOption)((IClonable)lastNomenclatureInGroup).Copy());
                            pageContainerGroup.TryAddElement(lastSummOption);
                            pageContainerGroup.TryAddElement(summDetailCalculate);
                            detailOptionBody_container.Add(pageContainerGroup);
                        }
                        else
                        {
                            detailOptionBody_container.Last().RemoveElement(summDetailCalculate);
                            detailOptionBody_container.Last().RemoveElement(lastSummOption);
                            detailOptionBody_container.Last().RemoveElement(lastNomenclatureInGroupOption);
                            pageContainerGroup = new PageContainer(GetWidth(), GetHeight(), ++pageNumber, image, _context);
                            pageContainerGroup.TryAddElement((BlockBodyOption)((IClonable)lastNomenclatureInGroupOption).Copy());
                            pageContainerGroup.TryAddElement(lastSummOption);
                            pageContainerGroup.TryAddElement(summDetailCalculate);
                            detailOptionBody_container.Add(pageContainerGroup);
                        }
                    }
                    else
                    {
                        detailOptionBody_container.RemoveAt(detailOptionBody_container.Count - 1);
                        detailOptionBody_container.Add(pageContainerGroup);
                    }
                    foreach (PageContainer page in detailOptionBody_container)
                    {
                        p_container.Add(page);//когда страница закончилась/ создаем новую страницу только если текущая закончилась или необходим перенос для установленной верстки
                    }
                    continue;
                }

                #endregion

                #region adv2

                if (block.Name == "adv2")
                {
                    PageContainer pageContainerAd2 = new PageContainer(GetWidth(), GetHeight(), -1, image, _context);
                    foreach (var item in ((ItemsControl)container.Child).Items)
                    {
                        AdView adBlock = new AdView(item);
                        AdView adBlock2 = (AdView)((IClonable)adBlock).Copy();
                        if (adBlock2 != null)
                        {
                            pageContainerAd2.TryAddElement(adBlock2);
                            ad_container2.Add(pageContainerAd2);
                            pageContainerAd2 = new PageContainer(GetWidth(), GetHeight(), -1, image, _context);
                        }
                    }
                    foreach (PageContainer page in ad_container2)
                    {
                        p_container.Add(page);
                        //result.Pages.Add(GetPageContent(new List<UIElement>() { page }, Colontitul.None));
                    }
                }

                #endregion adv2
            }
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
            try
            {
                PageContent result = new PageContent();
                FixedPage page = new FixedPage();
                page.Margin = new Thickness(
                    left * PrintLayout.A4.Size.Width / 21,
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
            catch (Exception ex)
            {
                L.LW(ex);
                return null;
            }
        }
    }
}
