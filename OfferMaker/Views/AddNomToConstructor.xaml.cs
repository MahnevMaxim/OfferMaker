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

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for AddNomenclatureToConstructor.xaml
    /// </summary>
    public partial class AddNomToConstructor : MetroWindow, IView
    {
        DispatcherTimer timerStartSearch;

        public AddNomToConstructor()
        {
            InitializeComponent();
            SetTimer();
        }

        void IView.OnSendMessage(string message) => this.ShowMessageAsync("", message);

        #region Catalog searching

        private void catalogExtendedTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => SearchString_TextChanged(null, null);

        private void SetTimer()
        {
            timerStartSearch = new DispatcherTimer();
            timerStartSearch.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timerStartSearch.Tick += TimerStartSearch_Tick;
        }

        private void SearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            timerStartSearch.Stop();
            timerStartSearch.Start();
        }

        private void TimerStartSearch_Tick(object sender, EventArgs e)
        {
            UpdateCatalogNomenclaturesView();
            timerStartSearch.Stop();
        }

        private void UpdateCatalogNomenclaturesView()
        {
            CollectionViewSource a = FindResource("Nomenclatures") as CollectionViewSource;
            if (a.View != null)
            {
                a.View.Refresh();
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            Nomenclature nomenclature = e.Item as Nomenclature;
            if (nomenclature != null)
            {
                if (nomenclature.Title != null)
                {
                    if (nomenclature.Title.ToLower().IndexOf(SearchString.Text.ToLower().Trim()) > -1)
                        e.Accepted = true;
                    else
                        e.Accepted = false;
                }
            }
        }

        #endregion Catalog searching

        #region Categories filter

        private void ShowAllCategoryButton_Click(object sender, RoutedEventArgs e) => UnselectTreeItem();

        private void UnselectTreeItem()
        {
            if (catalogExtendedTreeView.SelectedItem == null)
                return;

            if (catalogExtendedTreeView.SelectedItem is TreeViewItem)
            {
                (catalogExtendedTreeView.SelectedItem as TreeViewItem).IsSelected = false;
            }
            else
            {
                TreeViewItem item = catalogExtendedTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                    item.IsSelected = false;
                }
            }
        }

        private void ShowWithoutCategoryButton_Click(object sender, RoutedEventArgs e) => UnselectTreeItem();

        #endregion Categories filter

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                AppSettings.Default.AddNomIsMaximized = true;
            }
            else
            {
                AppSettings.Default.AddNomIsMaximized = false;
                AppSettings.Default.AddNomTop = Top;
                AppSettings.Default.AddNomLeft = Left;
                AppSettings.Default.AddNomHeight = Height;
                AppSettings.Default.AddNomWidth = Width;
            }
            AppSettings.Default.Save();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool isMaximized = AppSettings.Default.AddNomIsMaximized;
            if (isMaximized)
            {
                WindowState = WindowState.Maximized;
                return;
            }
            double top = AppSettings.Default.AddNomTop;
            double left = AppSettings.Default.AddNomLeft;
            double height = AppSettings.Default.AddNomHeight;
            double width = AppSettings.Default.AddNomWidth;
            if (top == 0 && left == 0 && height == 0 && width == 0) 
                return;
            Top = top;
            Left = left;
            Height = height;
            Width = width;
        }
    }
}
