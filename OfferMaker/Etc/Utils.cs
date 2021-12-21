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
        /// Объекты Image не клонируются должным образом, метод временный.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        //internal static ObservableCollection<Nomenclature> CloneNomsCollection(ICollection<ApiLib.Nomenclature> response)
        //{
        //    ObservableCollection<Nomenclature> res = Helpers.CloneObject<ObservableCollection<Nomenclature>>(response);
        //    foreach (var nom in response)
        //    {
        //        var target = res.Where(n => n.Id == nom.Id).First();
        //        target.Image = new Image(nom.Image.Guid, nom.Image.Creatorid, nom.Image.OriginalPath);
        //        if (nom.Images != null)
        //        {
        //            ObservableCollection<Image> images = new ObservableCollection<Image>();
        //            foreach (var im in nom.Images)
        //            {
        //                images.Add(new Image(im.Guid, im.Creatorid, im.OriginalPath));
        //            }
        //            target.Images = images;
        //        }
        //    }
        //    return res;
        //}

        //internal static Nomenclature CloneNom(Nomenclature selectedNomenclature)
        //{
        //    Nomenclature nomenclature = Helpers.CloneObject<Nomenclature>(selectedNomenclature);
        //    nomenclature.Image = new Image(selectedNomenclature.Image.Guid, selectedNomenclature.Image.Creatorid, selectedNomenclature.Image.OriginalPath);
        //    ObservableCollection<Image> images = new ObservableCollection<Image>();
        //    if(selectedNomenclature.Images!=null)
        //    {
        //        foreach (var im in selectedNomenclature.Images)
        //        {
        //            images.Add(new Image(im.Guid, im.Creatorid, im.OriginalPath));
        //        }
        //    }
        //    nomenclature.Images = images;
        //    return nomenclature;
        //}
    }
}
