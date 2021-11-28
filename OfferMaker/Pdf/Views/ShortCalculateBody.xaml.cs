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

namespace OfferMaker.Pdf.Views
{
    /// <summary>
    /// Interaction logic for ShortCalculateBody.xaml
    /// </summary>
    public partial class ShortCalculateBody : UserControl, IClonable
    {
        public ShortCalculateBody()
        {
            InitializeComponent();
        }

        public ShortCalculateBody(object DataContext)
        {
            this.DataContext = DataContext;
            InitializeComponent();
        }

        public UserControl Copy() => new ShortCalculateBody(DataContext);
    }
}
