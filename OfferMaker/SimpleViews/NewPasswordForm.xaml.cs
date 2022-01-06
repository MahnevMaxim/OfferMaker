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
    /// Interaction logic for NewPasswordForm.xaml
    /// </summary>
    public partial class NewPasswordForm : MetroWindow
    {
        public NewPasswordForm()
        {
            InitializeComponent();
            passwordTextBox.Password = Password.Generate(12, 0);
        }

        private void Button_Click_ChangePassword(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_Copy(object sender, RoutedEventArgs e) => Clipboard.SetText(passwordTextBox.Password);
    }
}
