using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OfferMaker
{
    public class Image
    {
        [Required]
        public string Guid { get; set; }

        public int Creatorid { get; set; }

        /// <summary>
        /// Оригинальный путь для того, чтобы если копирование сразу не удалось продолжать пытаться скопировать в фоновом режиме.
        /// </summary>
        public string OriginalPath { get; set; }
    }
}
