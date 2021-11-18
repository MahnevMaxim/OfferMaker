using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace OfferMaker
{
    public class Description : BaseModel, IDescription
    {
        public string Text { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsComment { get; set; }
    }
}
