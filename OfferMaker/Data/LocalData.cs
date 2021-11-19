using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

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
        internal void UpdateCache(object obj, string path) => Helpers.SaveObject(path, obj);

        #endregion Update cache

        #region Get data from cache

        /// <summary>
        /// Пытаемся получить данные из кэша
        /// </summary>
        /// <returns></returns>
        async internal Task<CallResult<T>> GetCache<T>(string path)
        {
            T res = Helpers.InitObject<T>(path);
            if (res != null)
                return new CallResult<T>() { Data = res };
            return new CallResult<T>() { Error = new Error("Ошибка при попытке получить кэш из файла" + path) };
        }

        #endregion Get data from cache
    }
}
