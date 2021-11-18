using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared
{
    [NotMapped]
    public class Description : IDescription
    {
        public string Text { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsComment { get; set; }
    }
}
