using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class Image
    {
        [Required]
        public string Guid { get; set; }

        public int Creatorid { get; set; }

        public string OriginalPath { get; set; }

        public bool IsDeleted { get; set; }
    }
}
