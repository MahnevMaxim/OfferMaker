using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace OfferMaker
{
    public class OfferInfoBlock : BaseEntity
    {
        bool isEnabled = true;
        bool isCustom;
        string title;
        string text;
        string imagePath;
        ImageSource image;

        public bool IsEnabled 
        { 
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsCustom
        {
            get => isCustom;
            set
            {
                isCustom = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text  = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
            }
        }

        public ImageSource Image
        {
            get
            {
                if(image==null)
                {
                    Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + imagePath, UriKind.Absolute);
                    image = (ImageSource)new BitmapImage(uri);
                }
                return image;
            }
        }
    }
}
