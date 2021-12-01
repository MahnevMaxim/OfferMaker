using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class OfferInfoBlock
    {
        public bool IsEnabled { get; set; }

        public bool IsCustom { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string ImagePath { get; set; }
    }
}
