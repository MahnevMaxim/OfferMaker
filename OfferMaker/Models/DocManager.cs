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

        private System.Collections.ObjectModel.ObservableCollection<OfferInfoBlock> GetInfoBlocks(OldModelCommercial.MainViewModelContainer mainViewModelContainer)
        {
            return new System.Collections.ObjectModel.ObservableCollection<OfferInfoBlock>()
            {
                new OfferInfoBlock(){
                    Title = "Срок готовности товара к отгрузке",
                    Text = mainViewModelContainer.Notification.ShipmentText,
                    ImagePath ="Images\\informIcons\\Commertial1.png"
                },
                new OfferInfoBlock(){
                    Title = "Срок проведения монтажных и пусконаладочных работ",
                    Text = mainViewModelContainer.Notification.MountText,
                    ImagePath= "Images\\informIcons\\Commertial2.png"
                },
                new OfferInfoBlock(){
                    Title = "Условие оплаты",
                    Text = mainViewModelContainer.Notification.PaymentText ,
                    ImagePath= "Images\\informIcons\\Commertial3.png"
                },
                new OfferInfoBlock(){
                    Title = "Условия поставки",
                    Text = mainViewModelContainer.Notification.DeliveryText,
                    ImagePath="Images\\informIcons\\Commertial4.png"
                },
                 new OfferInfoBlock(){
                    Title = "Гарантия",
                    Text = mainViewModelContainer.Notification.WarrantyText,
                    ImagePath= "Images\\informIcons\\Commertial2.png"
                }
            };
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
                        //magic
                        Offer tranlaterVM = new Offer();
                        tranlaterVM.SetConstructor(constructor);
                        tranlaterVM.OfferName = mainViewModelContainer.Customer.KpName;
                        tranlaterVM.CreateDate = mainViewModelContainer.Customer.Date;
                        tranlaterVM.CreateDateString = mainViewModelContainer.Customer.Date.ToShortDateString();
                        #region Customer
                        tranlaterVM.Customer.FullName = mainViewModelContainer.Customer.Name;
                        tranlaterVM.Customer.Location = mainViewModelContainer.Customer.Location;
                        tranlaterVM.Customer.Organization = mainViewModelContainer.Customer.Organization;
                        //tranlaterVM.Customer.Id
                        #endregion

                        // Коллекция информблоков.
                        #region InformBlock
                        tranlaterVM.OfferInfoBlocks = GetInfoBlocks(mainViewModelContainer);
                        #endregion

                        #region CustomerVM
                       
                        //tranlaterVM.CustomerVM.KpNumber = mainViewModelContainer.Customer.KpNumber.ToString();
                        #endregion
                        tranlaterVM.Currencies = Global.Currencies;
                        tranlaterVM.Currency = Global.Currencies.FirstOrDefault(x => x.IsoCode == 810);
                        #region Groups
                         tranlaterVM.OfferGroups = new System.Collections.ObjectModel.ObservableCollection<OfferGroup>();
                        foreach (var group in mainViewModelContainer.Groups)
                        {
                            OfferGroup offerGroup = new OfferGroup(tranlaterVM);
                            offerGroup.NomWrappers = new System.Collections.ObjectModel.ObservableCollection<NomWrapper>();
                            offerGroup.GroupTitle = group.Name;
                            foreach (var item in group.Items)
                            {
                                Nomenclature nom = new Nomenclature();
                                nom.Markup = (decimal)item.MarkUp;
                                nom.Title = item.Name;
                                nom.CostPrice = item.CostPrice;
                                nom.Descriptions = new System.Collections.ObjectModel.ObservableCollection<Description>();
                            
                                foreach (var description in item.Description)
                                {
                                    Description descrip = new Description();
                                    descrip.IsEnabled = true;
                                    descrip.Text = description;
                                    nom.Descriptions.Add(descrip);
                                    nom.CurrencyCharCode = tranlaterVM.Currency.CharCode;
                                }
                                NomWrapper nomWrapper = new NomWrapper(offerGroup, nom);
                                nomWrapper.Amount = (int)item.Number;
                                offerGroup.NomWrappers.Add(nomWrapper);
                            }
                            tranlaterVM.OfferGroups.Add(offerGroup);
                        }
                        #endregion

                        tranlaterVM.Discount = new Discount(tranlaterVM);
                        tranlaterVM.Discount.DiscountSum = 0;
                        tranlaterVM.Discount.IsEnabled = false;
                        tranlaterVM.Discount.Percentage = 0;
                        tranlaterVM.Discount.TotalSum = 0;

                        //tranlaterVM.OfferCreator.Position.PositionName = mainViewModelContainer.SelectedUser.Status;
                        //User manager = new User
                        //{
                        //    FirstName = mainViewModelContainer.SelectedUser.Name,
                        //    LastName = mainViewModelContainer.SelectedUser.Name,
                        //    PhoneNumber1 = mainViewModelContainer.SelectedUser.Tel1,
                        //    PhoneNumber2 = mainViewModelContainer.SelectedUser.Tel2,
                        //    Email = mainViewModelContainer.SelectedUser.Email
                        //};
                        
                        //tranlaterVM.Manager = manager;

                        //tranlaterVM.Manager.Image = ToImage(mainViewModelContainer.SelectedUser.Foto);

                        //tranlaterVM.OfferCreator.PhoneNumber1
                        //tranlaterVM.OfferCreator.PhoneNumber2
                        //tranlaterVM.OfferCreator.Email
                        //tranlaterVM.OfferCreator.Account

                        //концовка +- такая
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
        async internal void OfferCreate(bool isTemplate = false)
        {
            if (constructor.Offer.Id != 0)
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
                Global.Main.SendMess(cr.SuccessMessage);
            else
                Global.Main.SendMess(cr.Error.Message);
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
