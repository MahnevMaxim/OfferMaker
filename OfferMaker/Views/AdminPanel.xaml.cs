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
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : MetroWindow, IView
    {
        public AdminPanel()
        {
            InitializeComponent();
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.InvokedItem;

            if (!e.IsItemOptions && this.HamburgerMenuControl.IsPaneOpen)
            {
                // close the menu if a item was selected
                this.HamburgerMenuControl.IsPaneOpen = false;
            }
        }

        private void accessPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).User.Pwd = ((PasswordBox)sender).Password; 
        }

        private void newUserPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).NewUserPassword = ((PasswordBox)sender).Password;
        }
    }
}
