using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface ICustomer
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Location { get; set; }

        public string Organization { get; set; }
    }
}
