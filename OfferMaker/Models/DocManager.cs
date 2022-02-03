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
using System.Windows.Media.Imaging;

namespace OfferMaker
{
    public class DocManager
    {
        Constructor constructor;
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Offer Maker Projects\\";
        string omfFilter = "Offer Maker file | *.omf";
        string omfKccFilter = "Offer file | *.omf;*.kcc";

        #region Singleton

        private DocManager() => constructor = Global.Main.Constructor;

        private static readonly DocManager instance = new DocManager();

        public static DocManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Сохранение PDF с баннером.
        /// </summary>
        async internal void SaveToPdfWithBanner()
        {
            bool isArchive = await IsArchive();
            if (!isArchive) return;

            constructor.CreateDocumentWithBanner();
            FixedDocument fixedDoc = constructor.PdfDocument;
            SaveToPdf(fixedDoc);
        }

        /// <summary>
        /// Сохранение Pdf без баннеров.
        /// </summary>
        async internal void SaveToPdfWithoutBanner()
        {
            bool isArchive = await IsArchive();
            if (!isArchive) return;

            constructor.CreateDocumentWithoutBanner();
            FixedDocument fixedDoc = constructor.PdfDocumentShort;
            SaveToPdf(fixedDoc);
        }

        /// <summary>
        /// Не только проверяет архив или нет, но и пытается добавить в архив.
        /// </summary>
        /// <returns></returns>
        async Task<bool> IsArchive()
        {
            if (!constructor.Offer.IsArchive)
                await OfferCreate();
            return constructor.Offer.IsArchive;
        }

        async internal void PrintPdfWithoutBanner()
        {
            bool isArchive = await IsArchive();
            if (!isArchive) return;

            constructor.CreateDocumentWithoutBanner();
            FixedDocument fixedDoc = constructor.PdfDocumentShort;
            PrintPdf(fixedDoc);
        }

        async internal void PrintPdfWithBanner()
        {
            bool isArchive = await IsArchive();
            if (!isArchive) return;

            constructor.CreateDocumentWithBanner();
            FixedDocument fixedDoc = constructor.PdfDocument;
            PrintPdf(fixedDoc);
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

        internal void OfferTemplateCreate() => OfferCreate(true);

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
            ofd.Filter = omfKccFilter;//старая и новая версия 
            ofd.InitialDirectory = defaultPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);
                if (ext == ".omf")
                {
                    Offer offer = Helpers.InitObject<Offer>(ofd.FileName, true);
                    Offer offer_ = Utils.RestoreOffer(offer, Global.Users, false);
                    Global.Constructor.LoadOfferFromArchive(offer_);
                }
                else
                {
                    OldModelCommercial.MainViewModelContainer mainViewModelContainer = Helpers.InitObject<OldModelCommercial.MainViewModelContainer>(ofd.FileName, false);
                    if (mainViewModelContainer != null)
                    {
                        Offer tranlaterVM = new Offer();
                        tranlaterVM.SetConstructor(constructor);
                        Offer tranlaterVM_ = Utils.RestoreOldOffer(tranlaterVM, mainViewModelContainer, Global.Users, true);
                        Global.Constructor.LoadOfferFromArchive(tranlaterVM_);
                    }
                }
            }
        }
        public BitmapImage ToImage(byte[] array)
        {
            if (array == null)
            {
                return new BitmapImage();
            }
            System.IO.MemoryStream memorystream = new System.IO.MemoryStream();
            memorystream.Write(array, 0, (int)array.Length);
            BitmapImage imgsource = new BitmapImage();
            imgsource.BeginInit();
            imgsource.StreamSource = memorystream;
            imgsource.EndInit();
            return imgsource;
        }
        /// <summary>
        /// Сохранение(создание) шаблонов и КП на сервер/локально.
        /// </summary>
        /// <param name="isTemplate"></param>
        async internal Task OfferCreate(bool isTemplate = false)
        {
            if(!isTemplate)
            {
                if (constructor.Offer.Id != 0 || constructor.Offer.IsArchive)
                {
                    Global.Main.SendMess("Нельзя перезаписать архив.");
                    return;
                }

                //если создаётся архивное КП
                if (constructor.Offer.Id == 0 && !isTemplate)
                {
                    if (string.IsNullOrWhiteSpace(constructor.Offer.Customer.FullName)
                        || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Organization)
                        || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Location))
                    {
                        Global.Main.SendMess("Имя клиента, компания и город должны быть заполнены.");
                        return;
                    }
                }
            }

            CallResult cr;
            if (isTemplate)
            {
                Offer temp = CreateTemplate(constructor.Offer);
                cr = await Global.Main.DataRepository.OfferTemplateCreate(temp); 
                if (cr.Success)
                {
                    Global.Main.TemplatesStore.AddOffer(temp);
                    Global.Constructor.LoadOfferTemplate(temp);
                }
            }
            else
            {
                cr = await Global.Main.DataRepository.OfferCreate(constructor.Offer.PrepareArchive());
                if(cr.Success)
                {
                    Global.Main.ArchiveStore.AddOffer(constructor.Offer);
                    Global.Constructor.LoadOfferTemplate(constructor.Offer);
                }
            }

            Global.Main.SendMess(cr.Message);
            Global.Main.ArchiveStore.ApplyOfferFilter();
        }

        private Offer CreateTemplate(Offer offer)
        {
            Offer temp_ = Helpers.CloneObject<Offer>(offer);
            Offer temp = Utils.RestoreOffer(temp_, Global.Main.Users, false);
            temp.IsTemplate = true;
            temp.IsArchive = false;
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
