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
using System.Windows.Xps.Packaging;
using System.IO.Packaging;

namespace OfferMaker
{
    public class DocManager
    {
        Constructor constructor;
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Offer Maker Projects\\";
        string omfFilter = "Offer Maker file |*.omf;*.kcc";

        #region Singleton

        private DocManager() => constructor = Global.Main.Constructor;

        private static readonly DocManager instance = new DocManager();

        public static DocManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Сохранение PDF.
        /// </summary>
        async internal Task<CallResult> SaveToPdf(bool isWithBanner)
        {
            if (constructor.Offer.OfferState != OfferState.Archive)
            {
                CallResult cr = await OfferCreate();
                if (!cr.Success)
                {
                    return cr;
                }
            }


            //название кп заказчик номер
            //номер с датой, конечно
            string path;
            var offer = Global.Constructor.Offer;
            string defaultFileName = offer.OfferName + " " + offer.Customer.Organization + " " + offer.AltId + ".pdf";
            defaultFileName = defaultFileName.Replace("\"","");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = defaultFileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                path = saveFileDialog.FileName;
            else
                return new CallResult();

            FixedDocument fixedDoc;
            if (isWithBanner)
            {
                constructor.CreateDocumentWithBanner();
                fixedDoc = constructor.PdfDocument;
            }
            else
            {
                constructor.CreateDocumentWithoutBanner();
                fixedDoc = constructor.PdfDocumentShort;
            }
            await Task.Delay(1000);
            SaveToPdf(fixedDoc, path);
            return new CallResult();
        }

        /// <summary>
        /// Отправка на печать.
        /// </summary>
        /// <param name="isWithBanner"></param>
        /// <returns></returns>
        async internal Task<CallResult> PrintPdf(bool isWithBanner)
        {
            if (constructor.Offer.OfferState != OfferState.Archive)
            {
                CallResult cr = await OfferCreate();
                if (!cr.Success)
                {
                    return cr;
                }
            }

            FixedDocument fixedDoc;
            if (isWithBanner)
            {
                constructor.CreateDocumentWithBanner();
                fixedDoc = constructor.PdfDocument;
            }
            else
            {
                constructor.CreateDocumentWithoutBanner();
                fixedDoc = constructor.PdfDocumentShort;
            }

            PrintPdf(fixedDoc);
            return new CallResult();
        }

        async internal void PrintPdf(FixedDocument fixedDocument)
        {
            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
            dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            if (dialog.ShowDialog() == true)
            {
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(dialog.PrintQueue);
                writer.WriteAsync(fixedDocument as FixedDocument, dialog.PrintTicket);
            }
        }

        internal void SaveToPdf(FixedDocument fixedDoc, string filePath) => FixedDocument2Pdf(fixedDoc, filePath);
        //{
        //    System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
        //    var serv = new LocalPrintServer();
        //    printDialog.PrintQueue = serv.GetPrintQueue("Microsoft Print to PDF");

        //    PrintTicket pt = new PrintTicket()
        //    {
        //        OutputColor = OutputColor.Color,
        //        PageBorderless = PageBorderless.Borderless,
        //        PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4),
        //        OutputQuality = OutputQuality.Photographic,
        //        PageMediaType = PageMediaType.ScreenPaged,
        //        PageOrder = PageOrder.Standard,
        //        PageOrientation = PageOrientation.Portrait,
        //        PageResolution = new PageResolution(PageQualitativeResolution.High),
        //        PagesPerSheet = 1
        //    };

        //    printDialog.PrintTicket = pt;
        //    FixedDocumentSequence fixedDocumentSequence1 = (IDocumentPaginatorSource)fixedDoc as FixedDocumentSequence;

        //    if (fixedDoc != null)
        //        fixedDoc.PrintTicket = printDialog.PrintTicket;

        //    if (fixedDocumentSequence1 != null)
        //        fixedDocumentSequence1.PrintTicket = printDialog.PrintTicket;

        //    XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);

        //    if (fixedDoc != null)
        //        writer.WriteAsync(fixedDoc, printDialog.PrintTicket);

        //    if (fixedDocumentSequence1 != null)
        //        writer.WriteAsync(fixedDocumentSequence1, printDialog.PrintTicket);
        //}

        public static void FixedDocument2Pdf(FixedDocument fd, string filePath)
        {
            // Convert FixedDocument to XPS file in memory
            var ms = new MemoryStream();
            var package = Package.Open(ms, FileMode.Create);
            var doc = new XpsDocument(package);
            var writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write(fd.DocumentPaginator);
            doc.Close();
            package.Close();

            // Get XPS file bytes
            var bytes = ms.ToArray();
            ms.Dispose();

            // Print to PDF
            PdfFilePrinter.PrintXpsToPdf(bytes, filePath, "Document Title");
        }

        async internal Task<CallResult<Offer>> OfferTemplateCreate()
        {
            Offer temp = CreateTemplate(constructor.Offer);
            CallResult cr = await Global.Main.DataRepository.OfferTemplateCreate(temp);
            return new CallResult<Offer>() { Data = temp };
        }

        internal void SaveOfferToFile()
        {
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            var sfd = new SaveFileDialog();
            sfd.Filter = omfFilter;
            if (Global.Offer.Id != 0)
                sfd.FileName = String.Format("Offer from {0}", Global.Offer.AltId);
            else
                sfd.FileName = String.Format("Offer from {0}", DateTime.Today.ToShortDateString());
            sfd.InitialDirectory = defaultPath;

            if (sfd.ShowDialog() == DialogResult.OK)
                Helpers.SaveObject(sfd.FileName, Global.Offer);
        }

        internal void OpenOfferFromFile()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = omfFilter;
            ofd.InitialDirectory = defaultPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);
                Offer offer = null;

                if (ext == ".omf")
                    offer = Helpers.InitObject<Offer>(ofd.FileName);
                else if (ext == ".kcc")
                    offer = Utils.GetOldOffer(ofd.FileName);

                if (offer != null)
                {
                    if (offer.OfferState == OfferState.Archive || offer.OfferState == OfferState.OldArchive)
                        Global.Constructor.LoadOfferFromArchive(offer);
                    else
                        Global.Constructor.LoadOfferTemplate(offer);
                }
                else
                {
                    Global.Main.SendMess("Ошибка при попытке считать файл");
                }
            }
        }

        /// <summary>
        /// Сохранение КП на сервер/локально.
        /// </summary>
        async internal Task<CallResult> OfferCreate()
        {
            if (string.IsNullOrWhiteSpace(constructor.Offer.Customer.FullName)
                   || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Organization)
                   || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Location))
            {
                return new CallResult() { Error = new Error("Имя клиента, компания и город должны быть заполнены.") };
            }

            if (constructor.Offer.IsArchive)
            {
                //создаём дочерний архив, меняем родителя только для первого потомка, для остальных родитель тот же, 1 поколение только у потомков
                if (constructor.Offer.ParentGuid == null)
                {
                    string parentGuid = constructor.Offer.Guid;
                    int parentId = constructor.Offer.Id;
                    constructor.Offer.ParentId = parentId;
                    constructor.Offer.ParentGuid = parentGuid;
                }

                constructor.Offer.Id = 0;
                constructor.Offer.Guid = Guid.NewGuid().ToString();

                CallResult cr = await Global.Main.DataRepository.OfferHistoryCreate(constructor.Offer.PrepareArchive());
                return cr;
            }
            else
            {
                CallResult cr = await Global.Main.DataRepository.OfferCreate(constructor.Offer.PrepareArchive());
                return cr;
            }
        }

        private Offer CreateTemplate(Offer offer)
        {
            offer.Customer = new Customer();
            Offer temp_ = Helpers.CloneObject<Offer>(offer);
            Offer temp = Utils.RestoreOffer(temp_, Global.Main.Users, false);
            temp.OfferState = OfferState.Template;
            temp.OfferState = OfferState.Template;
            temp.IsEditableState = true;
            temp.IsEdited = false;
            temp.IsDelete = false;
            temp.Id = 0;
            temp.Guid = Guid.NewGuid().ToString();
            temp.CreateDate = DateTime.Now;
            return temp;
        }
    }
}
