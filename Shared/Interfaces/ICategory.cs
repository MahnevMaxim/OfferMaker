using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface ICategory
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Guid { get; set; }

        public string ParentGuid { get; set; }

        public int? ParentId { get; set; }
    }
}
