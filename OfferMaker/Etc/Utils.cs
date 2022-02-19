using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

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
                if (isArchive) 
                    offer.OfferState = OfferState.Archive;
                else
                    offer.OfferState = OfferState.Template;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return offer;
        }

        internal static Offer GetOldOffer(string fileName)
        {
            Offer offer = new Offer();

            try
            {
                string xmlText = File.ReadAllText(fileName);
                XDocument doc = XDocument.Parse(xmlText);

                var cust = doc.Root.Element("Customer");

                offer.OldIdentifer = cust.Element("KpNumber").Value;
                offer.OfferName = cust.Element("KpName").Value;
                var date = cust.Element("Date").Value;
                offer.CreateDate = DateTime.Parse(date);

                var customerName = cust.Element("Name").Value;
                var organization = cust.Element("Organization").Value;
                var location = cust.Element("Location").Value;

                offer.Customer = new Customer()
                {
                    FullName = customerName,
                    Location = location,
                    Organization = organization
                };

                var user = doc.Root.Element("SelectedUser");
                string userEmail = user.Element("Email").Value;
                offer.OfferCreator = Global.Users.Where(u => u.Email == userEmail).FirstOrDefault() ?? Global.User;

                ObservableCollection<Description> descs = new ObservableCollection<Description>();
                var notification = doc.Root.Element("Notification");

                var shipmentIsVisible = notification.Element("ShipmentIsVisible").Value;
                var shipmentText = notification.Element("ShipmentText").Value;

                var mountIsVisible = notification.Element("MountIsVisible").Value;
                var mountText = notification.Element("MountText").Value;

                var paymentIsVisible = notification.Element("PaymentIsVisible").Value;
                var paymentText = notification.Element("PaymentText").Value;

                var deliveryIsVisible = notification.Element("DeliveryIsVisible").Value;
                var deliveryText = notification.Element("DeliveryText").Value;

                var warrantyIsVisible = notification.Element("WarrantyIsVisible").Value;
                var warrantyText = notification.Element("WarrantyText").Value;

                AddDescription(shipmentIsVisible, shipmentText, descs);
                AddDescription(mountIsVisible, mountText, descs);
                AddDescription(paymentIsVisible, paymentText, descs);
                AddDescription(deliveryIsVisible, deliveryText, descs);
                AddDescription(warrantyIsVisible, warrantyText, descs);

                var nomenclatureGroups = doc.Root.Element("Groups");
                ObservableCollection<OfferGroup> offerGroups = new ObservableCollection<OfferGroup>();
                foreach (var group in nomenclatureGroups.Elements())
                {
                    OfferGroup offerGroup = new OfferGroup(offer);
                    ObservableCollection<NomWrapper> nomWrappers = new ObservableCollection<NomWrapper>();
                    offerGroup.GroupTitle = group.Element("Name").Value;
                    foreach (var nomenclature in group.Element("Items").Elements())
                    {
                        var name = nomenclature.Element("Name").Value;
                        var costPrice = decimal.Parse(nomenclature.Element("CostPrice").Value, CultureInfo.InvariantCulture);
                        var markUp = decimal.Parse(nomenclature.Element("MarkUp").Value, CultureInfo.InvariantCulture);
                        var markUp_ = decimal.Parse(nomenclature.Element("MarkUp").Value, CultureInfo.InvariantCulture);
                        var number = nomenclature.Element("Number").Value;

                        Nomenclature nom = new Nomenclature()
                        {
                            Title = name,
                            CostPrice = costPrice,
                            Markup = markUp,
                            CurrencyCharCode = "RUB"
                        };

                        foreach (var desc in nomenclature.Element("Description").Elements())
                        {
                            nom.Descriptions.Add(new Description() { IsEnabled = true, Text = desc.Value });
                        }

                        nomWrappers.Add(new NomWrapper(offerGroup, nom));
                    }
                    offerGroup.NomWrappers = nomWrappers;
                    offerGroups.Add(offerGroup);
                }
                offer.OfferGroups = offerGroups;
                offer.Discount = new Discount(offer);
                offer.Currency = Global.GetRub();
                offer.Currencies = new ObservableCollection<Currency>() { Global.GetRub() };
                offer.OfferState = OfferState.OldArchive;
                offer.Guid = Guid.NewGuid().ToString();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return null;
            }

            return offer;
        }

        private static void AddDescription(string isVisible, string text, ObservableCollection<Description> descs)
        {
            if (isVisible == null || text == null) return;
            bool isEnabled = bool.Parse(isVisible);
            descs.Add(new Description() { IsEnabled = isEnabled, Text = text });
        }
    }
}
