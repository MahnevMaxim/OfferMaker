using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

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

        internal static Offer GetOldOffer(string fileName)
        {
            Offer offer = new Offer();

            string xmlText = File.ReadAllText(fileName);
            XDocument doc = XDocument.Parse(xmlText);

            var cust = doc.Root.Element("Customer");
            var kpNumber = cust.Element("KpNumber").Value;
            var kpName = cust.Element("KpName").Value;
            var date = cust.Element("Date").Value;
            var customerName = cust.Element("Name").Value;
            var organization = cust.Element("Organization").Value;
            var location = cust.Element("Location").Value;

            string title = kpName;
            Customer customer = new Customer()
            {
                FullName = customerName,
                Location=location,
                Organization=organization
            };

            return offer;
        }
    }
}
