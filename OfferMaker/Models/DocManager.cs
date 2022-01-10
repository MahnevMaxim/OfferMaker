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
using System.Windows.Forms;

namespace OfferMaker
{
    public class DocManager
    {
        Constructor constructor;
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Offer Maker Projects\\";
        string omfFilter = "Offer Maker file | *.omf";

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
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
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

        internal void SaveOfferToFile()
        {
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }

            var sfd = new SaveFileDialog();
            sfd.Filter = omfFilter;
            sfd.FileName = String.Format("Offer from {0}", DateTime.Today.ToShortDateString());
            sfd.InitialDirectory = defaultPath;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Helpers.SaveObject(sfd.FileName, Global.Offer);
            }
        }

        internal void OpenOfferFromFile()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = omfFilter;
            ofd.InitialDirectory = defaultPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Offer offer = Helpers.InitObject<Offer>(ofd.FileName);
                Offer offer_ = Utils.RestoreOffer(offer, Global.Users);
                Global.Constructor.LoadOfferFromArchive(offer_);
            }
        }

        /// <summary>
        /// Сохранение шаблонов и КП.
        /// </summary>
        /// <param name="isTemplate"></param>
        async internal void SaveOffer(bool isTemplate = false)
        {
            Offer offer;
            if (isTemplate)
            {
                Offer temp = CreateTemplate(constructor.Offer);
                Global.Main.offers.Add(temp);
                offer = temp;
            }
            else
            {
                Global.Main.offers.Add(constructor.Offer);
                offer = constructor.Offer;
            }

            CallResult cr = await Global.Main.DataRepository.SaveOffer(offer, Global.Main.offers);
            if (!cr.Success)
                Global.Main.SendMess(cr.Error.Message);
            Global.Main.ArchiveOffers = Global.Main.ArchiveFilter.GetFilteredOffers();
        }

        private Offer CreateTemplate(Offer offer)
        {
            Offer temp_ = Helpers.CloneObject<Offer>(offer);
            Offer temp = Utils.RestoreOffer(temp_, Global.Main.Users);
            temp.IsTemplate = true;
            temp.Id = 0;
            temp.CreateDate = DateTime.Now;
            return temp;
        }
    }
}
