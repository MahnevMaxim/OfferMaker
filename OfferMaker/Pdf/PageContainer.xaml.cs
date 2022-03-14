using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OfferMaker.Controls
{
    public enum PageStatus { Open, Close }

    public partial class PageContainer : UserControl
    {
        double _width, _height;
        public int pNumber;
        public PageStatus PageStatus;

        public PageContainer(double width, double height, int pageNumber, BitmapImage image, object context)
        {
            DataContext = context;
            InitializeComponent();

            pNumber = pageNumber;
            if (pageNumber == 0)
            {
                colontitul.Visibility = Visibility.Visible;
                this.image.Source = image;
            }
            else if (pageNumber > 0)
            {
                colontitul.Visibility = Visibility.Visible;
                this.pageNumber.Text = pageNumber.ToString();
                this.image.Source = image;
                Thickness margin = container.Margin;
                margin.Top = 30;
                container.Margin = margin;
            }
            //криво, для рекламы
            else if (pageNumber < 0)
            {
                colontitul.Visibility = Visibility.Collapsed;
            }

            Width = _width = width;
            Height = _height = height;
            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }

        public UserControl Copy() => this;

        public void TryAddElement(UserControl element)
        {
            double koef = (double)Settings.GetInstance().ContentWidth / 100;
            element.Width = _width * koef;
            container.VerticalAlignment = VerticalAlignment.Top;
            
            container.Children.Add(element);

            container.Measure(new Size(_width, _height));
            container.Arrange(new Rect());

            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }

        public void TryAddAdvertisingElement(UserControl element)
        {
            double koef = (double)Settings.GetInstance().AdvertisingWidth / 100;
            colontitul.Visibility = Visibility.Collapsed;
            element.Width = _width * koef;
            container.Children.Add(element);

            container.Measure(new Size(_width, _height));
            container.Arrange(new Rect());

            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }

        public void RemoveElement(UserControl element)
        {
            container.Children.Remove(element);
            container.Measure(new Size(_width, _height));
            container.Arrange(new Rect());
            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }

        internal void AddElements(List<UserControl> els)
        {
            els.ForEach(e=> TryAddElement(e));
        }

        internal void RemoveElements(List<UserControl> els)
        {
            els.ForEach(e=>RemoveElement(e));
        }
    }
}
