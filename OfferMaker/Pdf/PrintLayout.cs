using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OfferMaker.Pdf
{
    /// <summary>
    /// Вспомагательный класс
    /// Генерирует размеры стандартных форматов бумаги (А4)
    /// </summary>
    public class PrintLayout
    {
        public static readonly PrintLayout A4 = new PrintLayout("21cm", "29.7cm", "2cm", "1cm", "1cm", "1cm");
        public static readonly PrintLayout A3 = new PrintLayout("29.7cm", "42cm", "2cm", "1cm", "1cm", "1cm");

        private Size _Size;
        private Thickness _Margin;

        PrintLayout(string w, string h, string left, string top, string right, string bottom)
        {
            var converter = new LengthConverter();
            var width = (double)converter.ConvertFromInvariantString(w);
            var height = (double)converter.ConvertFromInvariantString(h);
            var marginLeft = (double)converter.ConvertFromInvariantString(left);
            var marginTop = (double)converter.ConvertFromInvariantString(top);
            var marginRight = (double)converter.ConvertFromInvariantString(right);
            var marginBottom = (double)converter.ConvertFromInvariantString(bottom);
            _Size = new Size(width, height);
            _Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);
        }

        /// <summary>
        /// отступы от печатной области страницы
        /// </summary>
        public Thickness Margin
        {
            get { return _Margin; }
            set { _Margin = value; }
        }

        /// <summary>
        /// Размер листа
        /// </summary>
        public Size Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// Размер колонки
        /// </summary>
        public double ColumnWidth
        {
            get { return this.Size.Width - Margin.Left - Margin.Right; }
        }
    }
}
