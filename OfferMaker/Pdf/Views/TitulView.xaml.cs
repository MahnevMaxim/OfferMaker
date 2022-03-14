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
using OfferMaker.Pdf;

namespace OfferMaker.Pdf.Views
{
    /// <summary>
    /// Interaction logic for TitulView.xaml
    /// </summary>
    public partial class TitulView : UserControl, IClonable
    {
        bool isBannerVisibility;

        public TitulView()
        {
            InitializeComponent();
        }

        public TitulView(object DataContext, bool isBannerVisibility)
        {
            this.DataContext = DataContext;
            InitializeComponent();
            bannerGrid.Visibility = isBannerVisibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public UserControl Copy() => new TitulView(DataContext, isBannerVisibility);
    }
}
