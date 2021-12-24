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
        //DispatcherTimer timerStartSearch;

        public MainWindow()
        {
            InitializeComponent();

            //timerStartSearch = new DispatcherTimer();
            //timerStartSearch.Interval = new TimeSpan(0, 0, 0, 0, 300);
            //timerStartSearch.Tick += TimerStartSearch_Tick;
        }

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

        //#region Catalog searching

        //private void SearchString_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    timerStartSearch.Stop();
        //    timerStartSearch.Start();
        //}

        //private void TimerStartSearch_Tick(object sender, EventArgs e)
        //{
        //    UpdateCatalogNomenclaturesView();
        //    timerStartSearch.Stop();
        //}

        //private void UpdateCatalogNomenclaturesView()
        //{
        //    CollectionViewSource a = FindResource("Nomenclatures") as CollectionViewSource;
        //    if (a.View != null)
        //    {
        //        a.View.Refresh();
        //    }
        //}

        //#endregion Catalog searching

        //#region DragDrop to nomenclature group

        ///// <summary>
        ///// DnD номенклатуры в номенклатурную групп и установка строки в выбранное (selected) состояние.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //async private void DataGrid_Drop(object sender, DragEventArgs e)
        //{
        //    DataGrid dataGrid = (DataGrid)sender;
        //    DataGridRow row = GetParent<DataGridRow>(e.OriginalSource as DependencyObject);
        //    var group = row.DataContext as NomenclatureGroup;
        //    if (group != null)
        //    {
        //        var formats = e.Data.GetFormats();
        //        Nomenclature nomenclature = e.Data.GetData(formats[0]) as Nomenclature;
        //        if (nomenclature != null)
        //        {
        //            #region Model

        //            if (group.Nomenclatures.Contains(nomenclature))
        //                await this.ShowMessageAsync("", "Эта номенклатура уже есть в группе.");
        //            else
        //                group.Nomenclatures.Add(nomenclature);

        //            #endregion Model

        //            dataGrid.SelectedIndex = dataGrid.Items.IndexOf(group);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Ищем родителя нужного класса.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="d"></param>
        ///// <returns></returns>
        //private T GetParent<T>(DependencyObject d) where T : class
        //{
        //    while (d != null && !(d is T))
        //        d = VisualTreeHelper.GetParent(d);
        //    return d as T;
        //}

        ///// <summary>
        ///// Устанавливаем, что перетаскиваемый объект можно перетащить в TextBox.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TextBox_PreviewDragOver(object sender, DragEventArgs e) => e.Handled = true;

        //#endregion DragDrop to nomenclature group

        //#region Tree context menu

        ///// <summary>
        ///// С мультибиндингов комманда не шлётся почему-то. Редактирование категории.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mItem = (MenuItem)sender;
        //    Point position = mItem.PointToScreen(new Point(0d, 0d));
        //    var editItem = mItem.DataContext;
        //    ((BaseViewModel)DataContext).Cmd.Execute(new List<object> { "EditCategory", editItem, position });
        //}

        ///// <summary>
        ///// Удаление категории.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    var editItem = ((MenuItem)sender).DataContext;
        //    ((BaseViewModel)DataContext).Cmd.Execute(new List<object> { "DelCategory", editItem});
        //}

        //#endregion Tree context menu

        //#region Categories filter

        //private void ShowAllCategoryButton_Click(object sender, RoutedEventArgs e) => UnselectTreeItem();

        //private void UnselectTreeItem()
        //{
        //    if (catalogExtendedTreeView.SelectedItem == null)
        //        return;

        //    if (catalogExtendedTreeView.SelectedItem is TreeViewItem)
        //    {
        //        (catalogExtendedTreeView.SelectedItem as TreeViewItem).IsSelected = false;
        //    }
        //    else
        //    {
        //        TreeViewItem item = catalogExtendedTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
        //        if (item != null)
        //        {
        //            item.IsSelected = true;
        //            item.IsSelected = false;
        //        }
        //    }
        //}

        //private void ShowWithoutCategoryButton_Click(object sender, RoutedEventArgs e) => UnselectTreeItem();

        //private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        //{
        //    Nomenclature nomenclature = e.Item as Nomenclature;
        //    if (nomenclature != null)
        //    {
        //        if (nomenclature.Title != null)
        //        {
        //            if (!(nomenclature.Title.ToLower().IndexOf(SearchString.Text.ToLower()) > -1) || 
        //                LevenshteinDistance.Calculate(nomenclature.Title.ToLower(), SearchString.Text.ToLower()) < 5)
        //            {
        //                e.Accepted = false;
        //            }
        //            else
        //            {
        //                e.Accepted = true;
        //            }
        //        }
        //    }
        //}

        //#endregion Categories filter

        //private void TextBlock_Drop(object sender, DragEventArgs e) => UpdateCatalogNomenclaturesView();
    }
}
