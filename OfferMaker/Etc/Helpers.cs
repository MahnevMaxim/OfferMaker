using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Shared;
using System.Text.Json;

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
                Log.Write(ex);
            }
            return path;
        }

        /// <summary>
        /// Сериализация и сохранение объектов по указанному пути.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CallResult SaveObject(string filePath, object obj)
        {
            try
            {
                if (!Directory.Exists(LocalDataConfig.ServerCacheDataDir))
                    Directory.CreateDirectory(LocalDataConfig.ServerCacheDataDir);
                if (!Directory.Exists(LocalDataConfig.LocalDataDir))
                    Directory.CreateDirectory(LocalDataConfig.LocalDataDir);

                string output = JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, output);
                return new CallResult() { SuccessMessage = "Изменения сохранены" };
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(ex.Message) };
            }
        }

        /// <summary>
        /// Десериализация json объектов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T InitObject<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonText = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<T>(jsonText);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return default(T);
        }

        public static T CloneObject<T>(object obj) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }
}
