using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface IDescription
    {
        public string Text { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsComment { get; set; }
    }
}
