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
        public DialogCoordinator dialogCoordinator;

        public AdminPanel()
        {
            InitializeComponent();
            dialogCoordinator = (DialogCoordinator)DialogCoordinator.Instance;
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

        //с паролями всё неоднозначно, т.к. они в DataTemplate, может стоит сделать по другому, но пока приходится отлавливать
        //контролы паролей таким уродливым способом
        PasswordBox accessPasswordTextBox;
        PasswordBox newUserPasswordTextBox;
        PasswordBox newPasswordTextBox;
        PasswordBox newPasswordTextBoxRepeat;
        PasswordBox oldPasswordTextBox;

        /// <summary>
        /// Пароль для редактирования текущего аккаунта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accessPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).User.Pwd = ((PasswordBox)sender).Password; 
        }

        /// <summary>
        /// Пароль нового пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUserPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            newUserPasswordTextBox = (PasswordBox)sender;
            ((ViewModels.AdminPanelViewModel)DataContext).NewUserPassword = newUserPasswordTextBox.Password;
        }

        /// <summary>
        /// Новый пароль текущего пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).NewAccountPassword = ((PasswordBox)sender).Password;
        }

        /// <summary>
        /// Подтверждение пароля текущего пользователя для смены пароля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newPasswordTextBoxRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).NewAccountPasswordRepeat = ((PasswordBox)sender).Password;
        }

        /// <summary>
        /// Текущий пароль для смены пароля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oldPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.AdminPanelViewModel)DataContext).OldAccountPassword = ((PasswordBox)sender).Password;
        }

        public void ClearPwdNewUserPasswordTextBox() => newUserPasswordTextBox.Password = null;

        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).SelectAll();
        }
    }
}
