using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;

namespace OfferMaker.Pdf
{
    /// <summary>
    /// Interaction logic for PdfControl.xaml
    /// </summary>
    public partial class PdfControl : UserControl
    {
        public PdfControl()
        {
            //// создаем привязку команды
            //CommandBinding commandBinding = new CommandBinding();
            //// устанавливаем команду
            //commandBinding.Command = ApplicationCommands.Print;
            //// устанавливаем метод, который будет выполняться при вызове команды
            //commandBinding.Executed += PrintPdf;
            //// добавляем привязку к коллекции привязок элемента Button
            //pdfWithBanner.CommandBindings.Add(commandBinding);
            InitializeComponent();
        }
        //private void PrintPdf(object sender, ExecutedRoutedEventArgs e)
        //{
        //    //PrintDialog dialog = new PrintDialog();
        //    //dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
        //    //dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
        //    //dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

        //    //if (dialog.ShowDialog() == true)
        //    //{
        //    //    XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(dialog.PrintQueue);
        //    //    writer.WriteAsync(pdfWithBanner.Document as FixedDocument, dialog.PrintTicket);
        //    //    return true;
        //    //}

        //    //return false;
        //}
      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool resultOfferCreate = await OfferCreate(false);//Сохранение КП в архив
            if (resultOfferCreate == true)
            {
                PrintPdf();
            }
        }
       
        public bool PrintPdf() 
        {
            PrintDialog dialog = new PrintDialog();
            dialog.PrintTicket = dialog.PrintQueue.DefaultPrintTicket;
            dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            //var selectedDocumentViewer = ((System.Windows.Controls.ContentControl)tabControl1.SelectedItem).Content;
            System.Windows.Controls.DocumentViewer dw = (DocumentViewer)((System.Windows.Controls.ContentControl)tabControl1.SelectedItem).Content;
                //((System.Windows.FrameworkElement)((System.Windows.Controls.ContentControl)tabControl1.SelectedItem).Content);

            if (dialog.ShowDialog() == true)
            {
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(dialog.PrintQueue);
                //writer.WriteAsync(pdfWithBanner.Document as FixedDocument, dialog.PrintTicket);
                writer.WriteAsync(dw.Document as FixedDocument, dialog.PrintTicket);
                
                return true;
            }

            return false;
        }





    }
}
