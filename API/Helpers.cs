using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace API
{
    public class Helpers
    {
        public static T CloneObject<T>(object obj) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }
}
