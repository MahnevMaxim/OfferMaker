using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Controls;
using System.Windows;
using System.Collections;

namespace OfferMaker.ViewModels
{
    public class NomToCatDragHandler : DefaultDragHandler, IDragSource
    {
        public override void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            if (operationResult == DragDropEffects.Move)
            {
                if (dragInfo.Data.GetType().GetInterfaces().Contains(typeof(IList)))
                {
                    if (((IList)dragInfo.Data)[0].GetType() == typeof(Nomenclature))
                    {
                        foreach (var nom in (IList)dragInfo.Data)
                        {
                            Global.Catalog.CatalogFilter.RemoveDropedNom((Nomenclature)nom);
                        }
                    }
                }
                else
                {
                    var nomenclature = dragInfo.Data as Nomenclature;
                    Global.Catalog.CatalogFilter.RemoveDropedNom(nomenclature);
                }
            }
        }
    }
}
