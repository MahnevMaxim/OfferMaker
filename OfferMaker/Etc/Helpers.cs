using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Shared;
using System.Text.Json;
using System.Xml.Serialization;

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

        public static CallResult SaveObject(string filePath, object obj)
        {
            try
            {
                if (!Directory.Exists(LocalDataConfig.ServerCacheDataDir))
                {
                    Directory.CreateDirectory(LocalDataConfig.ServerCacheDataDir);
                }
                if (!Directory.Exists(LocalDataConfig.LocalDataDir))
                {
                    Directory.CreateDirectory(LocalDataConfig.LocalDataDir);
                }
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

        public static T InitObject<T>(string filePath, bool isJson)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    if (isJson)
                    {
                        string jsonText = File.ReadAllText(filePath);
                        var desObject = JsonConvert.DeserializeObject<T>(jsonText);
                        return desObject;
                    }
                    else
                    {
                        string xmlText = File.ReadAllText(filePath);
                        var desObject = XmlToObject<T>(xmlText);
                        return desObject;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return default(T);
        }
        public static T XmlToObject<T>(string str_stream)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                T container = (T)serializer.Deserialize(GenerateStreamFromString(str_stream));
                return container;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T CloneObject<T>(object obj) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }
}
