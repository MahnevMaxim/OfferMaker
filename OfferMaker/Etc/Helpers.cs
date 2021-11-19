using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Shared;

namespace OfferMaker
{
    class Helpers
    {
        internal static string GetFilePath(string fileFilter = null)
        {
            string path = null;
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = fileFilter;
                Nullable<bool> result = dialog.ShowDialog();

                if (result == true)
                {
                    path = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
            return path;
        }

        public static CallResult SaveObject(string filePath, object obj)
        {
            try
            {
                string output = JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, output);
                return new CallResult();
            }
            catch (Exception ex)
            {
                L.LW(ex);
                return new CallResult() { Error = new Error(ex.Message) };
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
