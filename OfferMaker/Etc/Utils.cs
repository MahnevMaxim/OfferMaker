using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    class Utils
    {
        /// <summary>
        /// Восстанавливаем данные и взаимосвязи класса Offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        static public Offer RestoreOffer(Offer offer, ObservableCollection<User> users, bool isArchive)
        {
            try
            {
                offer.OfferCreator = users.Where(u => u.Id == offer.OfferCreatorId).FirstOrDefault();
                offer.Manager = users.Where(u => u.Id == offer.ManagerId).FirstOrDefault();
                offer.OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(n => n.SetOfferGroup(g)));
                offer.OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(n => n.RestoreCurrencyCharCode()));
                offer.OfferGroups.ToList().ForEach(g => g.SetOffer(offer));
                if (isArchive) offer.IsArchive = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return offer;
        }
        //public Image AddImage(byte[] img)
        //{
        //    string path = GetPathToImage(img);
        //    if (path != null && path != "")
        //    {
        //        Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
        //        return image;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        /// <summary>
        /// Получение пути к новому изображению, созданному с использованием редактора
        /// </summary>
        /// <returns></returns>
        //private static string GetPathToImage(byte[] img)
        //{
        //    byte[] result = img;

        //    string currentDirectory = Directory.GetCurrentDirectory();
        //    string newImagePath = currentDirectory + @"\OldOffer\images\";
        //    if (!Directory.Exists(newImagePath))
        //    {
        //        Directory.CreateDirectory(newImagePath);
        //    }
        //    string retPath = newImagePath + DateTime.Now.ToShortDateString() + ".png";
        //    try
        //    {
        //        System.IO.File.WriteAllBytes(retPath, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        retPath = "";
        //    }

        //    return retPath;
        //}
        static public Offer RestoreOldOffer(Offer offer, OldModelCommercial.MainViewModelContainer mainViewModelContainer, ObservableCollection<User> users, bool isArchive)
        {
            try
            {
                #region Manager 
                offer.Manager = GetManager(mainViewModelContainer, users);
                #endregion
                #region Customer
                offer.OfferName = mainViewModelContainer.Customer.KpName;
                offer.OldKPNumber = mainViewModelContainer.Customer.KpNumber.ToString();
                offer.CreateDate = mainViewModelContainer.Customer.Date;
                offer.CreateDateString = mainViewModelContainer.Customer.Date.ToShortDateString();
                offer.Customer.FullName = mainViewModelContainer.Customer.Name;
                offer.Customer.Location = mainViewModelContainer.Customer.Location;
                offer.Customer.Organization = mainViewModelContainer.Customer.Organization;
                #endregion
                #region InformBlock
                offer.OfferInfoBlocks = GetInfoBlocks(mainViewModelContainer);
                #endregion
                #region Currencies
                //в старой версии всё в рублях
                offer.Currencies = Global.Currencies;
                offer.Currency = Global.Currencies.FirstOrDefault(x => x.IsoCode == 810);
                #endregion
                #region Groups
                offer.OfferGroups = new System.Collections.ObjectModel.ObservableCollection<OfferGroup>();
                foreach (var group in mainViewModelContainer.Groups)
                {
                    OfferGroup offerGroup = new OfferGroup(offer);
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
                            nom.CurrencyCharCode = offer.Currency.CharCode;
                        }
                        NomWrapper nomWrapper = new NomWrapper(offerGroup, nom);
                        nomWrapper.Amount = (int)item.Number;
                        offerGroup.NomWrappers.Add(nomWrapper);
                    }
                    offer.OfferGroups.Add(offerGroup);
                }
                #endregion
                #region Discount
                offer.Discount = new Discount(offer);
                offer.Discount.DiscountSum = 0;
                offer.Discount.IsEnabled = false;
                offer.Discount.Percentage = 0;
                offer.Discount.TotalSum = 0;
                #endregion

                if (isArchive) offer.IsArchive = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return offer;
        }
        static private User GetManager(OldModelCommercial.MainViewModelContainer mainViewModelContainer, ObservableCollection<User> users) 
        {
            User manager = new User();
            // Поиск менеждера из старого КП среди актуальных 
            //если найден - подтягиваются новые данные
            //не найден - подтягиваются данные из файла
            manager = users.Where(u => u.FullName == mainViewModelContainer.SelectedUser.Name).FirstOrDefault();
            if (manager == null)
            {
                string phrase = mainViewModelContainer.SelectedUser.Name;
                string[] words = phrase.Split(' ');
                //tranlaterVM.Manager.Image = ToImage(mainViewModelContainer.SelectedUser.Foto);
                Position position = new Position(mainViewModelContainer.SelectedUser.Status);
                manager = new User
                {
                    FirstName = words[0],
                    LastName = words[1],
                    PhoneNumber1 = mainViewModelContainer.SelectedUser.Tel1,
                    PhoneNumber2 = mainViewModelContainer.SelectedUser.Tel2,
                    Email = mainViewModelContainer.SelectedUser.Email,
                    Position = position,
                };
            }
            return manager;
        }
        static private ObservableCollection<OfferInfoBlock> GetInfoBlocks(OldModelCommercial.MainViewModelContainer mainViewModelContainer)
        {
            return new ObservableCollection<OfferInfoBlock>()
            {
                new OfferInfoBlock(){
                    Title = "Срок готовности товара к отгрузке",
                    Text = mainViewModelContainer.Notification.ShipmentText,
                    ImagePath ="Images\\informIcons\\Commertial1.png",
                    IsEnabled = mainViewModelContainer.Notification.ShipmentIsVisible
                },
                new OfferInfoBlock(){
                    Title = "Срок проведения монтажных и пусконаладочных работ",
                    Text = mainViewModelContainer.Notification.MountText,
                    ImagePath= "Images\\informIcons\\Commertial2.png",
                     IsEnabled = mainViewModelContainer.Notification.MountIsVisible
                },
                new OfferInfoBlock(){
                    Title = "Условие оплаты",
                    Text = mainViewModelContainer.Notification.PaymentText ,
                    ImagePath= "Images\\informIcons\\Commertial3.png",
                     IsEnabled = mainViewModelContainer.Notification.PaymentIsVisible
                },
                new OfferInfoBlock(){
                    Title = "Условия поставки",
                    Text = mainViewModelContainer.Notification.DeliveryText,
                    ImagePath="Images\\informIcons\\Commertial4.png",
                     IsEnabled = mainViewModelContainer.Notification.DeliveryIsVisible
                },
                 new OfferInfoBlock(){
                    Title = "Гарантия",
                    Text = mainViewModelContainer.Notification.WarrantyText,
                    ImagePath= "Images\\informIcons\\Commertial2.png",
                    IsEnabled = mainViewModelContainer.Notification.WarrantyIsVisible
                }
            };
        }

    }
}
