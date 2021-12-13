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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.SimpleViews
{
    /// <summary>
    /// Interaction logic for CategoryEditor.xaml
    /// </summary>
    public partial class CategoryEditor : MetroWindow
    {
        string beginString;
        Category category;

        public CategoryEditor(Category category)
        {
            ShowCloseButton = false;
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(HandleKeyPress);
            DataContext = category;
            beginString = category.Title;
            this.category = category;
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Close();
            if (e.Key == Key.Escape)
            {
                category.Title = beginString;
                Close();
            }  
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
        }
    }
}
