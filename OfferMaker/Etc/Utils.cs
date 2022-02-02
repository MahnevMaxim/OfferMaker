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

        static public Offer RestoreOldOffer(Offer offer, OldModelCommercial.MainViewModelContainer mainViewModelContainer, ObservableCollection<User> users, bool isArchive)
        {
            try
            {
                //string firstName = 
                Position position = new Position(mainViewModelContainer.SelectedUser.Status);
                User manager = new User
                {
                    FirstName = mainViewModelContainer.SelectedUser.Name,
                    LastName = mainViewModelContainer.SelectedUser.Name,
                    PhoneNumber1 = mainViewModelContainer.SelectedUser.Tel1,
                    PhoneNumber2 = mainViewModelContainer.SelectedUser.Tel2,
                    Email = mainViewModelContainer.SelectedUser.Email,
                    Position = position
                };
                
                
                offer.Manager = manager;
                offer.OfferCreator = manager;
                //offer.OfferCreator = users.Where(u => u.FullName == mainViewModelContainer.SelectedUser.Name).FirstOrDefault();
                //offer.Manager = users.Where(u => u.FullName == mainViewModelContainer.SelectedUser.Name).FirstOrDefault();
                if (isArchive) offer.IsArchive = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return offer;
        }


    }
}
