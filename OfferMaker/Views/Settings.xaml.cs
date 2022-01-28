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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow, IView
    {
        public DialogCoordinator dialogCoordinator;

        public Settings(bool isFromAuth)
        {
            InitializeComponent();
            dialogCoordinator = (DialogCoordinator)DialogCoordinator.Instance;
            clearCacheButton.IsEnabled = isFromAuth;
            downloadImagesButton.IsEnabled = !isFromAuth;
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var t = sender.GetType().Name;
            if(((ViewModels.SettingsViewModel)DataContext).IsBusy)
            {
                ((ViewModels.SettingsViewModel)DataContext).TryClose();
                e.Cancel = true;
            }
        }
    }
}
