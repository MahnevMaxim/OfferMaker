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

namespace OfferMaker
{
    class WrapperAllPages
    {
        private FlowDocument sourceDocument;
        private System.Windows.Media.Imaging.BitmapImage image;
        object _context;
        private double left, top, right, bottom;

        public WrapperAllPages(FlowDocument flowDocumentAll, object context)
        {
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

            PageContainer pageContainerShort = null;
            PageContainer pageContainerOptionShort = null;
            PageContainer pageContainerGroup = null;
            PageContainer pageContainer = new PageContainer(GetWidth(), GetHeight(), pageNumber, image, _context);//создаем одинн контейнер на весь документ
            
            object lastGroup = null;
            foreach (Block block in blocks)
            {
                if (block is Paragraph)
                {
                    if (((Paragraph)block).Inlines.FirstInline is InlineUIContainer)
                    {

                        InlineUIContainer container = (InlineUIContainer)((Paragraph)block).Inlines.FirstInline;
                        //титульник
                        if (container.Child is TitulView)
                        {
                            pageContainer.TryAddElement(((IClonable)(System.Windows.Controls.UserControl)container.Child).Copy());
                            p_container.Add(pageContainer);//когда страница закончилась
                        }
                    }
                }
            }
            return p_container;
        }

        double GetWidth()
        {
            return PrintLayout.A4.Size.Width - ((left + right) * PrintLayout.A4.Size.Width / 21);
        }

        double GetHeight()
        {
            return PrintLayout.A4.Size.Height - ((top + bottom) * PrintLayout.A4.Size.Height / 29.7);
        }

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
            page.Margin = new Thickness(
            left * PrintLayout.A4.Size.Width / 21,
            top * PrintLayout.A4.Size.Height / 29.7,
            right * PrintLayout.A4.Size.Width / 21,
            bottom * PrintLayout.A4.Size.Height / 29.7);

            PageContainer pc = new PageContainer(GetWidth(), GetHeight(), 0, image, _context);

            switch (colontitul)
            {
                case Colontitul.None:
                    break;
                case Colontitul.WithoutNumeric:
                    break;
                case Colontitul.WithNumeric:
                    break;
            }

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
                        pc.TryAddElement((System.Windows.Controls.UserControl)el);
                    }
                }
                else
                {
                    pc.TryAddElement((System.Windows.Controls.UserControl)element);
                }
            }
            page.Children.Add(pc);
            ((IAddChild)result).AddChild(page);
            return result;
        }
    }
}
