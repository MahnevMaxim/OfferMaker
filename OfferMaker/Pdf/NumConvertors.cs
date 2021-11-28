using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace OfferMaker.Pdf
{
    public class NumConvertors : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int del = 10;
                double doValue = System.Convert.ToDouble(value);
                double resultValue = Math.Round(doValue / del);
                resultValue *= del;
                string finalResult = String.Format("{0:### ### ### ### ###}", resultValue);

                return finalResult;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
