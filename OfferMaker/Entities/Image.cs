using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class Image : BaseEntity, IImage
    {
        string guid;
        int creatorid;
        string originalPath;
        string localPhotoPath;

        public int Id { get; set; }

        public string Guid 
        { 
            get => guid;
            set => guid = value;
        }

        public int Creatorid
        {
            get => creatorid;
            set => creatorid = value;
        }

        /// <summary>
        /// Оригинальный путь для того, чтобы если копирование сразу не удалось продолжать пытаться скопировать в фоновом режиме.
        /// </summary>
        public string OriginalPath
        {
            get => originalPath;
            set => originalPath = value;
        }

        public bool IsCopied { get; set; }

        public bool IsUploaded { get; set; }

        /// <summary>
        /// Флаг, указывающий, что при сохранении номенклатуры нужно загрузить изображение.
        /// </summary>
        [JsonIgnore]
        public bool IsNew { get; set; }

        /// <summary>
        /// С помощью этого свойства я пытаюсь добиться, чтобы запросы к картинке шли через кэш.
        /// </summary>
        [JsonIgnore]
        public string LocalPhotoPath
        {
            get
            {
                if (localPhotoPath == null && Global.ImageManager!=null)
                    localPhotoPath = Global.ImageManager.GetImagePath(Guid);
                return localPhotoPath;
            }
            set
            {
                localPhotoPath = value;
                OnPropertyChanged();
            }
        }

        public Image(string guid, int creatorid, string path)
        {
            Guid = guid;
            Creatorid = creatorid;
            OriginalPath = path;
        }

        public Image() { }
    }
}
