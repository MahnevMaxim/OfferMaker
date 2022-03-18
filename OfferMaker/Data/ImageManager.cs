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
        public delegate void UpdateHandler();
        public event UpdateHandler UpdateProgress;

        string apiEndpoint = Global.apiEndpoint;
        System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        Client client;
        string token;
        string imagesDirectory = AppSettings.Default.ImageManagerDir;
        public DownLoadProgress downLoadProgress;
        /// <summary>
        /// Перечисление того, что есть на сервере, для того, чтобы не опрашивать сервер лишний раз,
        /// по поводу не существующих картинок, которых там по сути быть не должно, но они иногда есть, при разработке 
        /// часто такое случается. 
        /// </summary>
        HashSet<string> serverGuids = new HashSet<string>();

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

        public static ImageManager GetInstance(HashSet<string> serverImageGuids)
        {
            instance.serverGuids = serverImageGuids;
            return instance;
        }

        #endregion Singleton

        /// <summary>
        /// Добавляем картинку во внутреннюю коллекцию, чтобы контролировать скопировалась она или нет.
        /// Если не скопировалась то пытаемся несколько раз сделать это повторно.
        /// </summary>
        /// <param name="image"></param>
        internal void Add(Image image, int? maxImageWidth)
        {
            Copy(image, maxImageWidth);
        }

        /// <summary>
        /// Копируем файл в кэш.
        /// </summary>
        /// <param name="image"></param>
        private void Copy(Image image, int? maxImageWidth)
        {
            try
            {
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);

                string ext = Path.GetExtension(image.OriginalPath);
                string filePath = Path.Combine(imagesDirectory, image.Guid + ext);
                if (image.OriginalPath != null)
                    FileCopy(image.OriginalPath, filePath, maxImageWidth);
                else
                    FileCreate(image.Bitmap, filePath);
                image.IsCopied = true;
            }
            catch (Exception ex)
            {
                Log.Write("исключение при попытке скорировать изображение в кэш", ex);
            }
        }

        /// <summary>
        /// С BitmapImage свойством на настоящий момент у нас только рекламные материалы. 
        /// Метод создаёт файл по указанному пути из BitmapImage.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="filePath"></param>
        private void FileCreate(BitmapImage bitmap, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fileStream = new FileStream(filePath + ".png", FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Копирование с возможным изменением.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        private void FileCopy(string source, string destinationPath, int? maxImageWidth)
        {
            Bitmap img = new Bitmap(source);
            int width = img.Width;
            int height = img.Height;
            if (width > maxImageWidth && maxImageWidth != null)
            {
                int newHeight = height * (int)maxImageWidth / width;
                Bitmap result = ResizeBitmap(img, (int)maxImageWidth, newHeight);
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
            if (guid == null) return null;

            //сначала пытаемся получить изображение из кэша
            string localFilePath = TryGetLocalFilePath(guid);
            if (localFilePath != null)
                return localFilePath;

            //если работаем в оффлайн режиме, то 
            if (Global.Settings.AppMode == AppMode.Offline)
                return null;

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

            //если не удалось качнуть
            return null;
        }

        /// <summary>
        /// Пытаемся вернуть файл из кэша.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private string TryGetLocalFilePath(string guid)
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), imagesDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var files = Directory.GetFiles(dir);
            var file = files.Where(f => f.Contains(guid)).FirstOrDefault();
            return file;
        }

        /// <summary>
        /// Скачивает изображение в кэш.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public CallResult GetImageFromServer(string guid)
        {
            if(!serverGuids.Contains(guid))
                return new CallResult() { Error = new Error("no image on server") };
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
            client.DownloadFile(new Uri(url), Path.Combine(imagesDirectory, filename));
        }

        public List<string> GetExceptImages(List<string> guids)
        {
            if (!Directory.Exists(imagesDirectory))
                Directory.CreateDirectory(imagesDirectory);
            string[] files = Directory.GetFiles(imagesDirectory);
            List<string> existingFilesGuids = new List<string>();
            files.ToList().ForEach(f => existingFilesGuids.Add(f.Split('.')[0].Replace(LocalDataConfig.ImageCacheDir + "\\", "")));
            return guids.Except(existingFilesGuids).ToList();
        }

        /// <summary>
        /// Подгружаем картинки в отдельном потоке, если надо.
        /// </summary>
        /// <param name="offer"></param>
        public void PrepareImages(Offer offer)
        {
            List<string> imageGuids = new List<string>();
            offer.OfferGroups.ToList().ForEach(o => o.NomWrappers.ToList().ForEach(n =>
            {
                if (n.Nomenclature.Image != null)
                    imageGuids.Add(n.Nomenclature.Image.Guid);
            }));

            List<string> needDownloadGuids = GetExceptImages(imageGuids);
            foreach (var guid in needDownloadGuids)
            {
                Global.Main.ProcessStatus = "Загрузка изображения " + guid;
                GetImageFromServer(guid);
            }
        }

        /// <summary>
        /// Синхронизация кэша с сервером.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        async internal Task SyncImagesWithServer(List<string> guids)
        {
            List<string> needDownloadGuids = GetExceptImages(guids);
            downLoadProgress = new DownLoadProgress() { BeginFilesCount = needDownloadGuids.Count };
            UpdateProgress();

            foreach (string guid in needDownloadGuids)
            {
                downLoadProgress.Status = "Загрузка " + guid;
                UpdateProgress();
                if (downLoadProgress.IsStop)
                    break;
                var cr = await Task.Run(() => GetImageFromServer(guid));
                if (cr.Success)
                {
                    downLoadProgress.CopiedFilesCount++;
                }
                else
                {
                    downLoadProgress.ErrorFilesCount++;
                    Log.Write("Загрузка изображения не удалась:\n" + cr.Message);
                }
                UpdateProgress();
            }
        }

        async internal Task DownloadImage(string guid)
        {
            await Task.Run(() => GetImageFromServer(guid));
        }

        internal void DownloadStop() => downLoadProgress.IsStop = true;

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

        internal void UploadNewImages(Offer offer)
        {
            List<Nomenclature> noms = new List<Nomenclature>();
            offer.OfferGroups.ToList().ForEach(o=>o.NomWrappers.ToList().ForEach(n=>noms.Add(n.Nomenclature)));
            UploadNewImages(noms);
        }

        async internal void UploadNewImages(List<Nomenclature> newNoms)
        {
            foreach (var nom in newNoms)
            {
                foreach (var image in nom.Images)
                {
                    if (image.IsNew && !serverGuids.Contains(image.Guid))
                    {
                        try
                        {
                            var file = TryGetLocalFilePath(image.Guid);
                            using var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
                            FileParameter param = new FileParameter(stream, Path.GetFileName(file));
                            var res = await client.ImagePostAsync(param);
                            serverGuids.Add(image.Guid);
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
                var res = await client.ImagePostAsync(param);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        async internal Task<CallResult> UploadBanner(Image banner)
        {
            try
            {
                var file = TryGetLocalFilePath(banner.Guid);
                using var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
                FileParameter param = new FileParameter(stream, Path.GetFileName(file));
                var res = await client.ImagePostAsync(param);
                return new CallResult();
            }
            catch (Exception ex)
            {
                return new CallResult() { Error = new Error(ex) };
            }
        }
    }

    public class DownLoadProgress
    {
        public int BeginFilesCount { get; set; }

        public int CopiedFilesCount { get; set; }

        public int ErrorFilesCount { get; set; }

        public string Status { get; set; }
        public bool IsStop { get; internal set; }
    }
}
