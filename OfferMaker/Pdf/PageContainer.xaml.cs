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
    /// <summary>
    /// Interaction logic for PageContainer.xaml
    /// </summary>
    public partial class PageContainer : UserControl
    {
        double _width, _height;

        public PageContainer(double width, double height, int pageNumber, BitmapImage image, object context)
        {
            DataContext = context;
            InitializeComponent();

            if (pageNumber == 0)
            {
                colontitul.Visibility = Visibility.Visible;
                this.pageNumber.Text = "";
                this.image.Source = image;
            }
            else if (pageNumber > 0)
            {
                colontitul.Visibility = Visibility.Visible;
                this.pageNumber.Text = pageNumber.ToString();
                this.image.Source = image;
            }
            //криво, для рекламы
            else if (pageNumber < 0)
            {
                colontitul.Visibility = Visibility.Hidden;
            }

            _width = width;
            _height = height;

            Width = _width;
            Height = _height;
            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }

        public UserControl Copy()
        {
            return this;
        }

        public double TryAddElement(UserControl element)
        {
            element.Width = _width;
            container.Children.Add(element);
            element.Measure(new Size(_width, _height));
            element.Arrange(new Rect());

            container.Measure(new Size(_width, _height));
            container.Arrange(new Rect());

            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));

            return container.ActualHeight;
        }

        public void RemoveElement(UserControl element)
        {
            container.Children.Remove(element);
            container.Measure(new Size(_width, _height));
            container.Arrange(new Rect());
            Measure(new Size(_width, _height));
            Arrange(new Rect(new Size(_width, _height)));
        }
    }
}
