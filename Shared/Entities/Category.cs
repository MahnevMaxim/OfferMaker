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
        //трибут Required представляет аннотацию, которая указывает, что свойство обязательно должно иметь значение
        [Required]
        public string Title { get; set; }

        [Required]
        public string Guid { get; set; }

        public string ParentGuid { get; set; }

        public int? ParentId { get; set; }

        public int Order { get; set; }
    }
}
