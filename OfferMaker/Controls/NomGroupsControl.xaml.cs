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
using GongSolutions.Wpf.DragDrop;

namespace OfferMaker.Controls
{
    /// <summary>
    /// Interaction logic for NomGroupsControl.xaml
    /// </summary>
    public partial class NomGroupsControl : UserControl
    {
        public NomGroupsControl()
        {
            InitializeComponent(); 
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            DataGrid grid = GetParent<DataGrid>(e.OriginalSource as DependencyObject);
            var group = ((Grid)sender).DataContext as NomenclatureGroup;
            if(group!=null)
            {
                var formats = e.Data.GetFormats();
                Nomenclature nomenclature = (Nomenclature)e.Data.GetData(formats[0]);
                if(nomenclature!=null)
                {
                    group.Nomenclatures.Add(nomenclature);
                    grid.SelectedIndex = grid.Items.IndexOf(group);
                }
            }
        }

        private T GetParent<T>(DependencyObject d) where T : class
        {
            while (d != null && !(d is T))
            {
                d = VisualTreeHelper.GetParent(d);
            }
            return d as T;
        }
    }
}
