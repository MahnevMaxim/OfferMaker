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
using OfferMaker.MvvmFactory;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);
    }
}
