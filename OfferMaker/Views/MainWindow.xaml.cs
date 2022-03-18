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
using System.Windows.Threading;
using OfferMaker.ViewModels;

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// Только логика получения/установки данных, все данные отправляем 
    /// для обработки в модель.
    /// </summary>
    public partial class MainWindow : MetroWindow, IView
    {
        public DialogCoordinator dialogCoordinator;
        bool isNeedClosing;

        public delegate void CloseWindowHandler();
        public event CloseWindowHandler CloseWindow;

        public MainWindow()
        {
            InitializeComponent();
            dialogCoordinator = (DialogCoordinator)DialogCoordinator.Instance;
        }

        MetroDialogSettings ms = new MetroDialogSettings();
        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);

        /// <summary>
        /// Так у нас первым окном открывается авторизация, то необходимо было установить ShutdownMode.OnExplicitShutdown.
        /// Приложение закрывается по событию OnClosed главного окна.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Прокрутка к добавленной группе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickAddOfferGroup(object sender, RoutedEventArgs e)
        {
            if (offerGroupsListView.Items.Count > 0)
            {
                offerGroupsListView.Items.MoveCurrentToLast();
                offerGroupsListView.ScrollIntoView(offerGroupsListView.Items.CurrentItem);
            }
        }

        async private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isNeedClosing)
            {
                e.Cancel = true;
                var vm = (MainViewModel)DataContext;
                isNeedClosing = await vm.TryClose();
                if (isNeedClosing)
                {
                    base.OnClosed(e);
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
