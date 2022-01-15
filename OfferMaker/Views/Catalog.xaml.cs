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
using System.Collections;

namespace OfferMaker.Views
{
    /// <summary>
    /// Interaction logic for Catalog.xaml
    /// </summary>
    public partial class Catalog : MetroWindow, IView
    {
        DispatcherTimer timerStartSearch;
        public DialogCoordinator dialogCoordinator;

        public Catalog()
        {
            InitializeComponent();
            SetTimer();
            dialogCoordinator = (DialogCoordinator)DialogCoordinator.Instance;
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

        #region DragDrop to nomenclature group

        /// <summary>
        /// DnD номенклатуры в номенклатурную групп и установка строки в выбранное (selected) состояние.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataGridRow row = GetParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null) return;
            var group = row.DataContext as NomenclatureGroup;
            if (group != null)
            {
                var formats = e.Data.GetFormats();
                Nomenclature nomenclature = e.Data.GetData(formats[0]) as Nomenclature;
                IList list = e.Data.GetData(formats[0]) as IList;
                if (nomenclature != null)
                {
                    #region Model

                    if (group.Nomenclatures.Contains(nomenclature))
                        await this.ShowMessageAsync("", "Эта номенклатура уже есть в группе.");
                    else
                        group.Nomenclatures.Add(nomenclature);

                    #endregion Model

                    dataGrid.SelectedIndex = dataGrid.Items.IndexOf(group);
                }

                if (list != null)
                {
                    #region Model

                    string message = "";
                    foreach (Nomenclature nom in list)
                    {
                        if (group.Nomenclatures.Contains(nom))
                            message+=nom.Title + " уже есть в группе.\n";
                        else
                            group.Nomenclatures.Add(nom);
                    }
                    if(!string.IsNullOrWhiteSpace(message))
                        await this.ShowMessageAsync("", message.Trim());

                    #endregion Model

                    dataGrid.SelectedIndex = dataGrid.Items.IndexOf(group);
                }
            }
        }

        /// <summary>
        /// Ищем родителя нужного класса.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        private T GetParent<T>(DependencyObject d) where T : class
        {
            while (d != null && !(d is T))
                d = VisualTreeHelper.GetParent(d);
            return d as T;
        }

        /// <summary>
        /// Устанавливаем, что перетаскиваемый объект можно перетащить в TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewDragOver(object sender, DragEventArgs e) => e.Handled = true;

        #endregion DragDrop to nomenclature group

        #region Tree context menu

        /// <summary>
        /// С мультибиндингов комманда не шлётся почему-то. Редактирование категории.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mItem = (MenuItem)sender;
            Point position = mItem.PointToScreen(new Point(0d, 0d));
            var editItem = mItem.DataContext;
            ((BaseViewModel)DataContext).Cmd.Execute(new List<object> { "EditCategory", editItem, position });
        }

        /// <summary>
        /// Удаление категории.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var editItem = ((MenuItem)sender).DataContext;
            ((BaseViewModel)DataContext).Cmd.Execute(new List<object> { "DelCategory", editItem });
        }

        #endregion Tree context menu

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

        private void TextBlock_Drop(object sender, DragEventArgs e) => UpdateCatalogNomenclaturesView();
    }
}
