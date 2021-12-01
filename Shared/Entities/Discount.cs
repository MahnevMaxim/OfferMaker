using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Discount
    {
        public bool IsEnabled { get; set; }

        public bool IsPercentage { get; set; }

        public decimal Percentage { get; set; }

        public decimal DiscountSum { get; set; }
    }
}
