using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.IO;
using System.Reflection;

namespace OfferMaker
{
    class LocalData
    {
        public static string DataCacheDir = LocalDataConfig.DataCacheDir;
        public static string LocalDataDir = LocalDataConfig.LocalDataDir;

        #region Update cache

        /// <summary>
        /// Обновление кэша
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        internal CallResult UpdateCache(object obj, string path) => Helpers.SaveObject(Path.Combine(DataCacheDir, path), obj);

        /// <summary>
        /// Добавление объекта в локальные данные.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        async internal Task<CallResult> Post<T>(object obj, string path)
        {
            string localDataPath = Path.Combine(LocalDataDir, path);
            var dataCr = await GetData<List<T>>(localDataPath);
            if (dataCr.Success)
            {
                dataCr.Data.Add((T)obj);
                CallResult cr = Helpers.SaveObject(localDataPath, dataCr.Data);
                return cr;
            }
            else
            {
                CallResult cr = Helpers.SaveObject(localDataPath, new List<T>() { (T)obj });
                return cr;
            }
        }

        /// <summary>
        /// Ответственность по поиску и удалению локальных объектов лежит на этом методе.
        /// В зависимости от Id удаляется из одного или другого фала.
        /// В зависимости от isNeedRemove удаляем или помечаем к удалению.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        async internal Task<CallResult> Delete<T>(IEntity obj, string path, bool isMarkAsDeleted = false) where T : IEntity
        {
            int id = obj.Id;
            string guid = obj.Guid;
            if (guid == null)
                throw new Exception("guid is null"); //guid ни при каких обстоятельствах не должен быть null
            string dataPath = Path.Combine(LocalDataDir, path);
            var dataCr = await GetData<List<T>>(dataPath);

            if (dataCr.Success)
            {
                var obj_ = dataCr.Data.FirstOrDefault(d => d.Guid == guid);
                //если данные локальные, то удаляем
                if (id == 0)
                {
                    dataCr.Data.Remove(obj_);
                }
                //если данные с сервера, то помечаем к удалению и ложим в ту же коллекцию
                else
                {
                    if(isMarkAsDeleted)
                    {
                        //проверяем, нету ли объекта уже в коллекции
                        if (obj_ == null && isMarkAsDeleted)
                        {
                            obj.IsDelete = true;
                            dataCr.Data.Add((T)obj);
                        }
                        else
                        {
                            obj_.IsDelete = true;
                        }
                    }
                    else
                    {
                        dataCr.Data.Remove(obj_);
                    }
                }

                CallResult cr = Helpers.SaveObject(dataPath, dataCr.Data);
                return cr;
            }
            else
            {
                CallResult cr = Helpers.SaveObject(dataPath, new List<T>() { (T)obj });
                return cr;
            }
        }

        private string GetGuidValue(object obj) => obj.GetType().GetProperties().Where(p => p.Name == "Guid").First().GetValue(obj).ToString();

        private int GetIdValue(object obj) => Int32.Parse(obj.GetType().GetProperties().Where(p => p.Name == "Id").First().GetValue(obj).ToString());

        #endregion Update cache

        #region Get data from cache

        /// <summary>
        /// Пытаемся получить данные из кэша.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<T>> GetCache<T>(string path) => await GetData<T>(Path.Combine(DataCacheDir, path));


        /// <summary>
        /// Пытаемся получить локальные данные данные.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<T>> GetLocalData<T>(string path) => await GetData<T>(Path.Combine(LocalDataDir, path));

        /// <summary>
        /// Пытаемся получить данные из хранилища.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<T>> GetData<T>(string path)
        {
            T res = Helpers.InitObject<T>(path);
            if (res != null)
                return new CallResult<T>() { Data = res };
            T obj = (T)Activator.CreateInstance(typeof(T));
            return new CallResult<T>() { Error = new Error("Ошибка при попытке получить кэш из файла " + path), Data = obj };
        }

        #endregion Get data from cache
    }
}
