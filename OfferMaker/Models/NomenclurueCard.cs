using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class NomenclurueCard : BaseModel
    {
        public string currencyCharCode;

        public Nomenclature Nomenclature { get; set; }

        public string CurrencyCharCode
        {
            get => currencyCharCode; 
            set
            {
                currencyCharCode = value;
                Nomenclature.CurrencyCharCode = currencyCharCode;
                OnPropertyChanged();
            }
        }

        public List<string> Currencies { get; set; }

        public string Image { get => Environment.CurrentDirectory + @"\Images\no-image.jpg"; set { } }

        public NomenclurueCard(Nomenclature nomenclature)
        {
            Nomenclature = nomenclature;
            CurrencyCharCode = nomenclature.CurrencyCharCode;
            Currencies = Global.Main.UsingCurrencies.Select(c => c.CharCode).ToList();
            if (!Currencies.Contains(CurrencyCharCode))
            {
                Currencies.Add(CurrencyCharCode);
            }
        }
    }
}
