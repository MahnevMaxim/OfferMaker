using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using ApiLib;
using System.Net;
using System.Net.Mime;

namespace OfferMaker
{
    /// <summary>
    /// Класс, который копирует, сохраняет, кэширует, изменяет и загружает картинки.
    /// Также выступает в качестве прокси между сервером и клиентом.
    /// Просто получает имя файла(guid), есть есть локально - отдаёт, нет - качает и отдаёт.
    /// </summary>
    public class ImageManager
    {
        static List<Image> images = new List<Image>();
        string apiEndpoint = Global.apiEndpoint;
        System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        Client client;

        #region Singleton

        private ImageManager() => client = new Client(apiEndpoint, httpClient);

        private static readonly ImageManager instance = new ImageManager();

        public static ImageManager GetInstance() => instance;

        #endregion Singleton

        /// <summary>
        /// Добавляем картинку во внутреннюю коллекцию, чтобы контролировать скопировалась она или нет.
        /// Если не скопировалась то пытаемся несколько раз сделать это повторно.
        /// </summary>
        /// <param name="image"></param>
        internal void Add(Image image)
        {
            Copy(image);
        }

        /// <summary>
        /// Копируем файл в кэш.
        /// </summary>
        /// <param name="image"></param>
        private void Copy(Image image)
        {
            try
            {
                if (!Directory.Exists(AppSettings.Default.ImageManagerDir))
                {
                    Directory.CreateDirectory(AppSettings.Default.ImageManagerDir);
                }

                string ext = Path.GetExtension(image.OriginalPath);
                string filePath = Path.Combine(AppSettings.Default.ImageManagerDir, image.Guid + ext);
                File.Copy(image.OriginalPath, filePath);
                image.IsCopied = true;
            }
            catch (Exception ex)
            {
                Log.Write("исключение при попытке скорировать изображение в кэш", ex);
            }
        }

        /// <summary>
        /// Если картинка есть на диске - возвращаем путь к картинке,
        /// если нет - пытаемся скачать, скопировать на диск и вернуть путь,
        /// если не удаётся, то возвращаем ошибку и картинку заглушку.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal string GetImagePath(string guid)
        {
            //сначала пытаемся получить изображение из кэша
            string localFilePath = TryGetLocalFilePath(guid);
            if (localFilePath != null)
                return localFilePath;

            //если в кэше нет, то пытаемся качнуть,
            //при скачивании файл кэшируется
            CallResult cr = GetImageFromServer(guid);
            if (cr.Success)
            {
                //ещё разок пытаемся получить файл из кэша,
                //сто пудов получится
                localFilePath = TryGetLocalFilePath(guid);
                if (localFilePath != null)
                    return localFilePath;

                //здесь исключение, потому что если скачали, а картинки нет, то что не так
                throw new Exception("image not found");
            }

            //если не удалось качнуть, то возвращаем картинку по умолчанию
            return Environment.CurrentDirectory + @"\Images\no-image.jpg";
        }

        /// <summary>
        /// Пытаемся вернуть файл из кэша.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private string TryGetLocalFilePath(string guid)
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), AppSettings.Default.ImageManagerDir);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var files = Directory.GetFiles(dir);
            var file = files.Where(f => f.Contains(guid)).FirstOrDefault();
            return file;
        }

        private CallResult GetImageFromServer(string guid)
        {
            string imageUrl = apiEndpoint + "api/Images?guid=" + guid;
            try
            {
                DownloadFile(imageUrl);
                return new CallResult();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return new CallResult() { Error = new Error(ex) };
            }
        }

        /// <summary>
        /// Качаем файл прямо в кэш.
        /// </summary>
        /// <param name="url"></param>
        void DownloadFile(string url)
        {
            WebClient client = new WebClient();
            client.OpenRead(new Uri(url));
            string headerContentDisposition = client.ResponseHeaders["content-disposition"];
            string filename = new ContentDisposition(headerContentDisposition).FileName;
            client.DownloadFile(new Uri(url), Path.Combine(AppSettings.Default.ImageManagerDir, filename));
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }

        async internal static void UploadNewImages(List<Nomenclature> newNoms)
        {

            //images.AddRange(newNoms);
        }
    }
}
