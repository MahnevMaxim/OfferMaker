using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace OfferMaker
{
    class Helpers
    {
        public static void SaveObject(string filePath, object obj)
        {
            try
            {
                string output = JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, output);
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
        }

        public static T InitObject<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
            return default(T);
        }

        public static T CloneObject<T>(object obj)
        {
            string output = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(output);
        }
    }
}
