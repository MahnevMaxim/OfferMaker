using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Controls;
using System.Windows;

namespace OfferMaker.ViewModels
{
    public class NomToCatDragHandler : DefaultDragHandler, IDragSource
    {
        public override void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            var nomenclature = dragInfo.Data as Nomenclature;
            Global.Catalog.CatalogFilter.RemoveDropedNom(nomenclature);
        }
    }
}
