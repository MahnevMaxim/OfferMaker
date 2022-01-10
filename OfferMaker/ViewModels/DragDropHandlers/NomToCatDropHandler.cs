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
    public class NomToCatDropHandler : DefaultDropHandler, IDropTarget
    {
        bool isList;

        /// <summary>
        /// Test the specified drop information for the right data.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        new public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return false;
            }

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                return false;
            }

            // do not drop on itself
            var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
                                 && dropInfo.VisualTargetItem is TreeViewItem;
            if (isTreeViewItem && dropInfo.VisualTargetItem == dropInfo.DragInfo.VisualSourceItem)
            {
                return false;
            }

            if (dropInfo.TargetCollection is null)
            {
                return false;
            }

            if (ReferenceEquals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection))
            {
                var targetList = dropInfo.TargetCollection.TryGetList();
                return targetList != null;
            }

            if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
            {
                var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                return !isChildOf;
            }

            if (dropInfo.TargetItem == null)
            {
                return false;
            }

            var t = dropInfo.Data.GetType().Name;
            var t2 = dropInfo.Data.GetType();
            if (dropInfo.Data.GetType() == typeof(Nomenclature))
            {
                var targetCat = (Category)dropInfo.TargetItem;
                var nomenclature = (Nomenclature)dropInfo.Data;
                if (nomenclature.CategoryGuid == targetCat.Guid)
                    return false;
                return true;
            }

            if (dropInfo.Data.GetType().GetInterfaces().Contains(typeof(IList)))
            {
                if (((IList)dropInfo.Data)[0].GetType() == typeof(Nomenclature))
                {
                    var targetCat = (Category)dropInfo.TargetItem;
                    //если хоть одна номенклатура не содержится, то перетаскиваем
                    foreach (var nom in (IList)dropInfo.Data)
                    {
                        var nomenclature = (Nomenclature)nom;
                        if (nomenclature.CategoryGuid != targetCat.Guid)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <inheritdoc />
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                var copyData = ShouldCopyData(dropInfo);
                dropInfo.Effects = DragDropEffects.Move;
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        public virtual void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return;
            }

            if (dropInfo.Data.GetType() == typeof(Nomenclature))
            {
                var targetCat = (Category)dropInfo.TargetItem;
                var nomenclature = (Nomenclature)dropInfo.Data;
                if (nomenclature.CategoryGuid == targetCat.Guid)
                    return;
                Global.Catalog.RemoveNomFromCat(nomenclature);
                nomenclature.SetCategoryGuid(targetCat.Guid);
                targetCat.Nomenclatures.Add(nomenclature);
                return;
            }

            if (dropInfo.Data.GetType().GetInterfaces().Contains(typeof(IList)))
            {
                if (((IList)dropInfo.Data)[0].GetType() == typeof(Nomenclature))
                {
                    var targetCat = (Category)dropInfo.TargetItem;
                    List<Nomenclature> noms = new List<Nomenclature>();
                    foreach (var nom in (IList)dropInfo.Data)
                    {
                        var nomenclature = (Nomenclature)nom;
                        if (nomenclature.CategoryGuid == targetCat.Guid)
                        {
                            noms.Add(nomenclature);

                            continue;
                        }
                        Global.Catalog.RemoveNomFromCat(nomenclature);
                        nomenclature.SetCategoryGuid(targetCat.Guid);
                        targetCat.Nomenclatures.Add(nomenclature);
                    }
                    noms.ForEach(n => ((IList)dropInfo.DragInfo.Data).Remove(n));
                }
                return;
            }

            var insertIndex = GetInsertIndex(dropInfo);
            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool isSameCollection = false;

            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (sourceList != null)
                {
                    isSameCollection = sourceList.IsSameObservableCollection(destinationList);
                    if (!isSameCollection)
                    {
                        foreach (var o in data)
                        {
                            var index = sourceList.IndexOf(o);
                            if (index != -1)
                            {
                                sourceList.RemoveAt(index);

                                // If source is destination too fix the insertion index
                                if (destinationList != null && ReferenceEquals(sourceList, destinationList) && index < insertIndex)
                                {
                                    --insertIndex;
                                }
                            }
                        }
                    }
                }
            }

            if (destinationList != null)
            {
                var objects2Insert = new List<object>();

                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy) || dropInfo.Effects.HasFlag(DragDropEffects.Link);

                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        if (o is ICloneable cloneable)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);

                    if (!cloneData && isSameCollection)
                    {
                        var index = destinationList.IndexOf(o);
                        if (index != -1)
                        {
                            if (insertIndex > index)
                            {
                                insertIndex--;
                            }

                            Move(destinationList, index, insertIndex++);
                        }
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }
                }

                SelectDroppedItems(dropInfo, objects2Insert);
            }
        }
    }
}
