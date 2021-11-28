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
    /// Interaction logic for AdView.xaml
    /// </summary>
    public partial class AdView : UserControl, IClonable
    {
        public AdView()
        {
            InitializeComponent();
        }

        public AdView(object DataContext)
        {
            InitializeComponent();
            this.DataContext = DataContext;
        }

        public UserControl Copy() => new AdView(DataContext);
    }
}
