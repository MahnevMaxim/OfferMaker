using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Documents;
using System.Drawing.Printing;
using System.IO;
using Shared;

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
            constructor.CreateDocumentWithBanner();
            FixedDocument fixedDoc = constructor.PdfDocument;
            SaveToPdf(fixedDoc);
        }

        /// <summary>
        /// Сохранение Pdf без баннеров.
        /// </summary>
        internal void SaveToPdfWithoutBanner()
        {
            constructor.CreateDocumentWithoutBanner();
            FixedDocument fixedDoc = constructor.PdfDocumentShort;
            SaveToPdf(fixedDoc);
        }

        internal void SaveToPdf(FixedDocument fixedDoc)
        {
            PrintDialog printDialog = new PrintDialog();
            var serv = new LocalPrintServer();
            printDialog.PrintQueue = serv.GetPrintQueue("Microsoft Print to PDF");
            
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

        internal void SaveTemplateToArchive() => SaveOffer(true);
        
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

        async internal void SaveOffer(bool isTemplate=false)
        {
            constructor.Offer.IsTemplate = isTemplate;
            Global.Main.offers.Add(constructor.Offer);
            CallResult cr = await Global.Main.DataRepository.SaveOffer(constructor.Offer, Global.Main.offers);
            if (!cr.Success)
                Global.Main.SendMess(cr.Error.Message);
            Global.Main.ArchiveOffers = Global.Main.ArchiveFilter.GetFilteredOffers();
        }
    }
}
