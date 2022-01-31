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
            //SaveToPdf(fixedDoc);
            _ = SaveToPdf(fixedDoc);
        }

        /// <summary>
        /// Сохранение Pdf без баннеров.
        /// </summary>
        internal void SaveToPdfWithoutBanner()
        {

            constructor.CreateDocumentWithoutBanner();
            FixedDocument fixedDoc = constructor.PdfDocumentShort;
            _ = SaveToPdf(fixedDoc);
        }
      
        internal async Task PrintPdf(FixedDocument fixedDocument)
        {
            OfferCreateAnswer resultOfferCreate = await OfferCreate(false);//Сохранение КП в архив
            //отправлять на печать, если 
            //КП уже есть в архиве
            //или КП добавлено в архив
            if ((resultOfferCreate == OfferCreateAnswer.AddedToArchive) || (resultOfferCreate == OfferCreateAnswer.AllreadyInArchive))
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
        }
        internal async Task SaveToPdf(FixedDocument fixedDoc)
        {
            OfferCreateAnswer resultOfferCreate = await OfferCreate(false);//Сохранение КП в архив
            if ((resultOfferCreate == OfferCreateAnswer.AddedToArchive)|| (resultOfferCreate == OfferCreateAnswer.AllreadyInArchive))
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
        }

        internal void OfferTemplateCreate() => OfferCreate(true);

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
                Offer offer_ = Utils.RestoreOffer(offer, Global.Users, false);
                Global.Constructor.LoadOfferFromArchive(offer_);
            }
        }

        /// <summary>
        /// Сохранение(создание) шаблонов и КП на сервер/локально.
        /// </summary>
        /// <param name="isTemplate"></param>
        //async internal void OfferCreate(bool isTemplate = false)
        //{
        //    if (constructor.Offer.Id != 0)
        //    {
        //        Global.Main.SendMess("Нельзя перезаписать архив.");
        //        return;
        //    }

        //    //если создаётся архивное КП
        //    if (constructor.Offer.Id == 0 && !isTemplate)
        //    {
        //        if(string.IsNullOrWhiteSpace(constructor.Offer.Customer.FullName)
        //            || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Organization)
        //            || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Location))
        //        {
        //            Global.Main.SendMess("Имя клиента, компания и город должны быть заполнены.");
        //            return;
        //        }    
        //    }

        //    CallResult cr;
        //    if (isTemplate)
        //    {
        //        Offer temp = CreateTemplate(constructor.Offer);
        //        Global.Main.TemplatesStore.AddOffer(temp);
        //        cr = await Global.Main.DataRepository.OfferTemplateCreate(temp, Global.OfferTemplates);
        //    }
        //    else
        //    {
        //        Global.Main.ArchiveStore.AddOffer(constructor.Offer.PrepareArchive());
        //        Global.Main.OnPropertyChanged(nameof(Global.Main.UsingCurrencies));
        //        cr = await Global.Main.DataRepository.OfferCreate(constructor.Offer, Global.Offers);
        //    }

        //    if (cr.Success)
        //        Global.Main.SendMess(cr.SuccessMessage);
        //    else
        //        Global.Main.SendMess(cr.Error.Message);
        //    Global.Main.ArchiveStore.ApplyOfferFilter();
        //}

        public enum OfferCreateAnswer
        {
            AddedToArchive,
            NotAddedToArchive,
            AllreadyInArchive
        }
        //возвращает состояния: 
        //КП добавлено в архив
        //КП не добавлено в архив
        //КП уже есть в архиве
        async internal Task<OfferCreateAnswer> OfferCreate(bool isTemplate = false)
        {
            if (constructor.Offer.Id != 0)
            {
                Global.Main.SendMess("Нельзя перезаписать архив.");
                return OfferCreateAnswer.AllreadyInArchive;
                //КП уже есть в архиве
            }

            //если создаётся архивное КП
            if (constructor.Offer.Id == 0 && !isTemplate)
            {
                if (string.IsNullOrWhiteSpace(constructor.Offer.Customer.FullName)
                    || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Organization)
                    || string.IsNullOrWhiteSpace(constructor.Offer.Customer.Location))
                {
                    Global.Main.SendMess("Имя клиента, компания и город должны быть заполнены.");
                    return OfferCreateAnswer.NotAddedToArchive;
                }
            }

            CallResult cr;
            if (isTemplate)
            {
                Offer temp = CreateTemplate(constructor.Offer);
                Global.Main.TemplatesStore.AddOffer(temp);
                cr = await Global.Main.DataRepository.OfferTemplateCreate(temp, Global.OfferTemplates);
            }
            else
            {
                Global.Main.ArchiveStore.AddOffer(constructor.Offer.PrepareArchive());
                Global.Main.OnPropertyChanged(nameof(Global.Main.UsingCurrencies));
                cr = await Global.Main.DataRepository.OfferCreate(constructor.Offer, Global.Offers);
            }

            if (cr.Success)
            {
                Global.Main.SendMess(cr.SuccessMessage);
                return OfferCreateAnswer.AddedToArchive;
            }
            else
            {
                Global.Main.SendMess(cr.Error.Message);
                return OfferCreateAnswer.NotAddedToArchive;

            }
            Global.Main.ArchiveStore.ApplyOfferFilter();
        }

        private Offer CreateTemplate(Offer offer)
        {
            Offer temp_ = Helpers.CloneObject<Offer>(offer);
            Offer temp = Utils.RestoreOffer(temp_, Global.Main.Users, false);
            temp.IsTemplate = true;
            temp.Id = 0;
            temp.CreateDate = DateTime.Now;
            return temp;
        }
    }
}
