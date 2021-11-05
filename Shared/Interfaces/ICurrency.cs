using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface ICurrency
    {
        public int Id { get; set; }

        public int Code { get; set; }

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public string Symbol { get; set; }
    }
}
