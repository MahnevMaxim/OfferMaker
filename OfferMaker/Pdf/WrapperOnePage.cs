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

        public WrapperOnePage(object context, BitmapImage image)
        {
            this.image = image;
            _context = context;
        }
        
        /// <summary>
        /// Функция возвращает страничный фиксированный документ
        /// </summary>
        public FixedDocument GetPdf()
        {
            List<PageContainer> pages = GetPagesContainer();
            FixedDocument result = GetFixedDocument(pages);
            return result;
        }

        List<PageContainer> GetPagesContainer()
        {
            PdfBuilder builder = new PdfBuilder(_context, image);
            ViewModels.MainViewModel vm = (ViewModels.MainViewModel)_context;

            builder.AddTitulView(false);
            builder.AddCalc();
            builder.AddInformBlockPdf();
            builder.AddEmployee(false);

            return builder.allContainers;
        }

        public static FixedDocument GetFixedDocument(List<PageContainer> pagesContainer)
        {
            FixedDocument result = new FixedDocument();
            result.DocumentPaginator.PageSize = PrintLayout.A4.Size;
            foreach (PageContainer page in pagesContainer)
            {
                result.Pages.Add(GetPageContent(page));
            }
            return result;
        }

        public static PageContent GetPageContent(PageContainer pc)
        {
            try
            {
                PageContent result = new PageContent();
                FixedPage page = new FixedPage();
                page.Children.Add(pc);
                ((IAddChild)result).AddChild(page);
                return result;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }
        }
    }
}
