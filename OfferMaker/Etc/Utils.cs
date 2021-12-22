using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        static public Offer RestoreOffer(Offer offer, ObservableCollection<User> users)
        {
            try
            {
                offer.OfferCreator = users.Where(u => u.Id == offer.OfferCreatorId).FirstOrDefault();
                offer.Manager = users.Where(u => u.Id == offer.ManagerId).FirstOrDefault();
                offer.OfferGroups.ToList().ForEach(g => g.NomWrappers.ToList().ForEach(n => n.SetOfferGroup(g)));
                offer.OfferGroups.ToList().ForEach(g => g.SetOffer(offer));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return offer;
        }
    }
}
