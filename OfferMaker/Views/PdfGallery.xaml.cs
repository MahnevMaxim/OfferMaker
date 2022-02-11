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

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for PdfGallery.xaml
    /// </summary>
    public partial class PdfGallery : MetroWindow, IView
    {
        public PdfGallery()
        {
            InitializeComponent();
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);
    }
}
