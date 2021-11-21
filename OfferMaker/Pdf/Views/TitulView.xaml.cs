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
    /// Interaction logic for TitulView.xaml
    /// </summary>
    public partial class TitulView : UserControl, IClonable
    {
        public TitulView()
        {
            InitializeComponent();
        }

        public TitulView(object DataContext)
        {
            this.DataContext = DataContext;
            InitializeComponent();
        }

        public UserControl Copy()
        {
            TitulView titul = new TitulView(DataContext);
            return titul;
        }
    }
}
