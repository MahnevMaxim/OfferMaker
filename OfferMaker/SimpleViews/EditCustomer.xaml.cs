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

        Offer offerBackup;

        public EditCustomer(Offer offer)
        {
            InitializeComponent();
            Offer = offer;

            //для тестов, чтоб не ебаться каждый раз с заполнением
            Offer.OfferName = "Название КП";
            Offer.Customer.FullName = "Иванов Иван Сергеевич";
            Offer.Customer.Organization = "Монолитстрой";
            Offer.Customer.Location = "Красноярск, ул. Молокова, 1/2";
            Offer.Customer.Position = "Не увидел, где привязывается должность клиента";

            offerBackup = Helpers.CloneObject<Offer>(offer);
            DataContext = Offer;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Offer.OfferName = offerBackup.OfferName;
            Offer.Customer.FullName = offerBackup.Customer.FullName;
            Offer.Customer.Position = offerBackup.Customer.Position;
            Offer.Customer.Organization = offerBackup.Customer.Organization;
            Offer.Customer.Location = offerBackup.Customer.Location;
        }
    }
}
