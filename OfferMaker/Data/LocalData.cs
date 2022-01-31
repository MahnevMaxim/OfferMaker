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
        #region Update cache

        /// <summary>
        /// Обновление кэша
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        internal CallResult UpdateCache(object obj, string path) => Helpers.SaveObject(path, obj);

        /// <summary>
        /// Добавление объекта в локальные данные.
        /// Редактирует объект, если он уже есть (guid такой же если).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        async internal Task<CallResult> Post<T>(object obj, string path) where T : IEntity
        {
            string guid = GetGuidValue(obj);
            var dataCr = await GetData<List<T>>(path);
            if (dataCr.Success)
            {
                var item = dataCr.Data.FirstOrDefault(d => d.Guid == guid);
                if(item!=null)
                {
                    int index = dataCr.Data.IndexOf(item);
                    dataCr.Data.Remove(item);
                    dataCr.Data.Insert(index, (T)obj);
                }
                else
                    dataCr.Data.Add((T)obj);

                CallResult cr = Helpers.SaveObject(path, dataCr.Data);
                return cr;
            }
            else
            {
                CallResult cr = Helpers.SaveObject(path, new List<T>() { (T)obj });
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
            var dataCr = await GetData<List<T>>(path, true);

            var obj_ = dataCr.Data.FirstOrDefault(d => d.Guid == guid);
            //если данные локальные, то удаляем
            if (id == 0)
            {
                dataCr.Data.Remove(obj_);
            }
            //если данные с сервера, то помечаем к удалению и ложим в ту же коллекцию
            else
            {
                if (isMarkAsDeleted)
                {
                    //проверяем, нету ли объекта уже в коллекции
                    if (obj_ == null && isMarkAsDeleted)
                    {
                        obj.IsDelete = true;
                        dataCr.Data.Add((T)obj);
                    }
                    else
                    {
                        obj.IsDelete = true;
                        obj_.IsDelete = true;
                    }
                }
                else
                {
                    dataCr.Data.Remove(obj_);
                }
            }

            return Helpers.SaveObject(path, dataCr.Data);
        }

        private string GetGuidValue(object obj) => obj.GetType().GetProperties().Where(p => p.Name == "Guid").First().GetValue(obj).ToString();

        private int GetIdValue(object obj) => Int32.Parse(obj.GetType().GetProperties().Where(p => p.Name == "Id").First().GetValue(obj).ToString());

        #endregion Update cache

        #region Get data from cache

        /// <summary>
        /// Пытаемся получить данные из хранилища.
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<T>> GetData<T>(string path, bool isWithotErroMode = false)
        {
            T res = Helpers.InitObject<T>(path);
            if (res != null)
                return new CallResult<T>() { Data = res };
            
            T obj = (T)Activator.CreateInstance(typeof(T));
            if (isWithotErroMode)
                return new CallResult<T>() { Data = obj };
            return new CallResult<T>() { Error = new Error("Ошибка при попытке получить кэш из файла " + path), Data = obj };
        }

        #endregion Get data from cache
    }
}
