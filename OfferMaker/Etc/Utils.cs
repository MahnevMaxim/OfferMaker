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
        public Image AddImage(byte[] img)
        {
            string path = GetPathToImage(img);
            if (path != null && path != "")
            {
                Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                return image;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Получение пути к новому изображению, созданному с использованием редактора
        /// </summary>
        /// <returns></returns>
        private static string GetPathToImage(byte[] img)
        {
            byte[] result = img;

            string currentDirectory = Directory.GetCurrentDirectory();
            string newImagePath = currentDirectory + @"\OldOffer\images\";
            if (!Directory.Exists(newImagePath))
            {
                Directory.CreateDirectory(newImagePath);
            }
            string retPath = newImagePath + DateTime.Now.ToShortDateString() + ".png";
            try
            {
                System.IO.File.WriteAllBytes(retPath, result);
            }
            catch (Exception ex)
            {
                retPath = "";
            }

            return retPath;
        }
        static public Offer RestoreOldOffer(Offer offer, OldModelCommercial.MainViewModelContainer mainViewModelContainer, ObservableCollection<User> users, bool isArchive)
        {
            try
            {
                //string phrase = mainViewModelContainer.SelectedUser.Name;
                //string[] words = phrase.Split(' ');
                ////tranlaterVM.Manager.Image = ToImage(mainViewModelContainer.SelectedUser.Foto);
                //Position position = new Position(mainViewModelContainer.SelectedUser.Status);

                

                
                //User manager = new User
                //{
                //    FirstName = words[0],
                //    LastName = words[1],
                //    PhoneNumber1 = mainViewModelContainer.SelectedUser.Tel1,
                //    PhoneNumber2 = mainViewModelContainer.SelectedUser.Tel2,
                //    Email = mainViewModelContainer.SelectedUser.Email,
                //    Position = position,
                //};

                //string path = GetPathToImage(mainViewModelContainer.SelectedUser.Foto);
                //if (path != null && path != "")
                //{
                //    Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                //    manager.Image = image;
                //}

                //offer.Manager = manager;
                offer.OfferCreator = manager;
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
