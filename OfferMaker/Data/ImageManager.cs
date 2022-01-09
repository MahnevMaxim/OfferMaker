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
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

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
        string token;
        int maxImageWidth = 500;

        #region Singleton

        private ImageManager()
        {
            token = Settings.GetToken();
            if (token != null)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client = new Client(apiEndpoint, httpClient);
        }

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
                FileCopy(image.OriginalPath, filePath);
                image.IsCopied = true;
            }
            catch (Exception ex)
            {
                Log.Write("исключение при попытке скорировать изображение в кэш", ex);
            }
        }

        /// <summary>
        /// Копирование с возможным изменением.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        private void FileCopy(string source, string destinationPath)
        {
            Bitmap img = new Bitmap(source);
            int width = img.Width;
            int height = img.Height;
            if (width > maxImageWidth)
            {
                int newHeight = height * maxImageWidth / width;
                Bitmap result = ResizeBitmap(img, maxImageWidth, newHeight);
                result.Save(destinationPath, GetImageFormatFromPath(destinationPath));
            }
            else
            {
                File.Copy(source, destinationPath);
            }
        }

        /// <summary>
        /// Получаем формат из пути файла.
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        private ImageFormat GetImageFormatFromPath(string destinationPath)
        {
            string format = Path.GetExtension(destinationPath).ToLower();
            return format switch 
            {
                ".jpg" => ImageFormat.Jpeg,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".jpeg" => ImageFormat.Jpeg,
                ".ico" => ImageFormat.Icon,
                ".png" => ImageFormat.Png,
                ".tiff" => ImageFormat.Tiff,
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Изменение размеров изображения.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
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
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
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

        async internal void UploadNewImages(List<Nomenclature> newNoms)
        {
            foreach (var nom in newNoms)
            {
                foreach (var image in nom.Images)
                {
                    if (image.IsNew)
                    {
                        try
                        {
                            var file = TryGetLocalFilePath(image.Guid);
                            using var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
                            FileParameter param = new FileParameter(stream, Path.GetFileName(file));
                            var res = await client.ImagesPOSTAsync(param);
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                    }
                }
            }
        }

        async internal void UploadImage(User user)
        {
            try
            {
                var file = TryGetLocalFilePath(user.Image.Guid);
                using var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
                FileParameter param = new FileParameter(stream, Path.GetFileName(file));
                var res = await client.ImagesPOSTAsync(param);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
