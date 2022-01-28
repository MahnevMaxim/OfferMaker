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
using Shared;
using System.Collections.ObjectModel;
using System.IO;
using ApiLib;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for Hello.xaml
    /// </summary>
    public partial class Hello : MetroWindow, IView
    {
        public DialogCoordinator dialogCoordinator;

        public Hello()
        {
            InitializeComponent();
            dialogCoordinator = (DialogCoordinator)DialogCoordinator.Instance;
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);


        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
                return;
            ((BaseViewModel)DataContext).Cmd.Execute("LoginCommand");
        }

        private void accessPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((ViewModels.HelloViewModel)DataContext).Pwd = ((PasswordBox)sender).Password;
        }
    }
}
