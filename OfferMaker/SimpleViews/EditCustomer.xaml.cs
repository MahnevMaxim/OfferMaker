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

namespace OfferMaker.SimpleViews
{
    /// <summary>
    /// Interaction logic for EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : MetroWindow
    {
        public Offer Offer { get; set; }

        bool isClickConfirm;

        Offer offerBackup;

        public EditCustomer(Offer offer)
        {
            InitializeComponent();
            Offer = offer;
            offerBackup = Helpers.CloneObject<Offer>(offer);
            DataContext = Offer;
        }

        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            isClickConfirm = true;
            Close();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e) => CancelChanges();

        private void CancelChanges()
        {
            Offer.OfferName = offerBackup.OfferName;
            Offer.Customer.FullName = offerBackup.Customer.FullName;
            Offer.Customer.Position = offerBackup.Customer.Position;
            Offer.Customer.Organization = offerBackup.Customer.Organization;
            Offer.Customer.Location = offerBackup.Customer.Location;
        }

        private void cust_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClickConfirm)
                CancelChanges();
        }
    }
}
