using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Constructor : BaseModel
    {
        /// <summary>
        /// Группы номенклатур для контрола конструктора.
        /// </summary>
        public ObservableCollection<OfferGroup> OfferGroups { get; set; } = new ObservableCollection<OfferGroup>();

        /// <summary>
        /// Добавление новой группы в конструктор.
        /// </summary>
        internal void AddOfferGroup() => OfferGroups.Add(new OfferGroup() { GroupTitle = "ГРУППА" });
        

        /// <summary>
        /// Удаление группы из конструктора.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void DelOfferGroup(OfferGroup offerGroup) => OfferGroups.Remove(offerGroup);

        /// <summary>
        /// Добавление номенклатуры в группу конструктора.
        /// </summary>
        /// <param name="offerGroup"></param>
        internal void AddNomenclatureToOfferGroup(OfferGroup offerGroup) =>
            MvvmFactory.CreateWindow(new AddNomToConstructor(offerGroup), new ViewModels.AddNomToConstructorViewModel(), new Views.AddNomToConstructor(), ViewMode.ShowDialog);

        /// <summary>
        /// Удаление номенклатуры из группы в конструкторе
        /// </summary>
        /// <param name="nomWrapper"></param>
        /// <param name="offerGroup"></param>
        internal void DeleteNomWrapper(NomWrapper nomWrapper, OfferGroup offerGroup) => offerGroup.NomWrappers.Remove(nomWrapper);

        /// <summary>
        /// Удаление описания из номенклатуры из обертки номенклатуры для группы номенклатур в конструкторе.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nomWrapper"></param>
        internal void DeleteDescriptionFromNomWrapper(Description description, NomWrapper nomWrapper) => nomWrapper.Nomenclature.Descriptions.Remove(description);
        
    }
}
