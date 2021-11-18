using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Shared
{
    public class Category : ICategory
    {
        public int Id { get; set ; }

        [Required]
        public string Title { get; set; }

        public int ParentId { get; set; }
    }
}
