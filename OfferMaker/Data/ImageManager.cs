using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    /// <summary>
    /// Класс, который копирует, сохраняет, кэширует, изменяет и загружает картинки.
    /// Также выступает в качестве прокси между сервером и клиентом.
    /// Просто получает имя файла(guid), есть есть локально - отдаёт, нет - качает и отдаёт.
    /// </summary>
    public class ImageManager
    {
        #region Singleton

        private ImageManager() { }

        private static readonly ImageManager instance = new ImageManager();

        public static ImageManager GetInstance() => instance;

        #endregion Singleton
    }
}
