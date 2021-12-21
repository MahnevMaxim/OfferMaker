using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Documents;

namespace OfferMaker
{
    public class DocManager
    {
        Constructor constructor;

        #region Singleton

        private DocManager() => constructor = Global.Main.Constructor;

        private static readonly DocManager instance = new DocManager();

        public static DocManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Сохранение PDF с баннером.
        /// </summary>
        internal void SaveToPdfWithBanner()
        {
            constructor.CreateDocument();
            FixedDocument fixedDoc = constructor.PdfDocument;
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = new LocalPrintServer().GetPrintQueue("Microsoft Print to PDF");

            PrintTicket pt = new PrintTicket()
            {
                OutputColor = OutputColor.Color,
                PageBorderless = PageBorderless.Borderless,
                PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4),
                OutputQuality = OutputQuality.Photographic,
                PageMediaType = PageMediaType.ScreenPaged,
                PageOrder = PageOrder.Standard,
                PageOrientation = PageOrientation.Portrait,
                PageResolution = new PageResolution(PageQualitativeResolution.High),
                PagesPerSheet = 1
            };

            printDialog.PrintTicket = pt;
            FixedDocumentSequence fixedDocumentSequence1 = (IDocumentPaginatorSource)fixedDoc as FixedDocumentSequence;

            if (fixedDoc != null)
                fixedDoc.PrintTicket = printDialog.PrintTicket;

            if (fixedDocumentSequence1 != null)
                fixedDocumentSequence1.PrintTicket = printDialog.PrintTicket;

            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);

            if (fixedDoc != null)
                writer.WriteAsync(fixedDoc, printDialog.PrintTicket);

            if (fixedDocumentSequence1 != null)
                writer.WriteAsync(fixedDocumentSequence1, printDialog.PrintTicket);
        }

        internal void SaveToPdfWithoutBanner()
        {
            throw new NotImplementedException();
        }

        internal void SaveTemplateToArchive()
        {
            throw new NotImplementedException();
        }

        internal void LoadTemplate()
        {
            throw new NotImplementedException();
        }

        internal void SaveOfferToFile()
        {
            throw new NotImplementedException();
        }

        internal void OpenOfferFromFile()
        {
            throw new NotImplementedException();
        }
    }
}
