using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class ImageGuid
    {
        [Key]
        public string Guid { get; set; }
    }
}
