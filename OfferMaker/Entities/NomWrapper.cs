using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    /// <summary>
    /// Обёртка для номенклатуры в КП, содержащая количество, настройки и прочую хуету.
    /// </summary>
    public class NomWrapper
    {
        public int Count { get; set; } = 1;

        public bool IsEnabled { get; set; } = true;

        public bool IsShowPrice { get; set; } = true;

        public Nomenclature Nomenclature { get; set; }
}
}
