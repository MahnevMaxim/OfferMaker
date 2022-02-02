using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OfferMaker.OldModelCommercial
{
    public class MainViewModelContainer
    {
        public bool IsDetailed { get; set; }
        public bool CanAddSummaryPage { get; set; }
        public bool CanAddDescription { get; set; }
        public int UsersListSelectedIndex { get; set; }
        public CustomerViewModel Customer { get; set; }
        public User SelectedUser { get; set; }
        public Notification Notification { get; set; }
        public List<GroupViewModelContainer> Groups { get; set; }
        public String SelectedBannerPath { get; set; }
    }
    public class GroupViewModelContainer
    {
        public string Name { get; set; }
        public List<ItemViewModelContainer> Items { get; set; }
    }
    public class ItemViewModelContainer
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
        public double MarkUp { get; set; }
        public uint Number { get; set; }
        public List<string> Description { get; set; }
    }
    public class Notification
    {
        public bool ShipmentIsVisible { get; set; }
        public string ShipmentText { get; set; }
        public bool MountIsVisible { get; set; }
        public string MountText { get; set; }
        public bool PaymentIsVisible { get; set; }
        public string PaymentText { get; set; }
        public bool DeliveryIsVisible { get; set; }
        public string DeliveryText { get; set; }
        public bool WarrantyIsVisible { get; set; }
        public string WarrantyText { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Email { get; set; }
        public byte[] Foto { get; set; }
    }
    public class GroupViewModel
    {
        public string Name { get; set; }
        public decimal SummaryCostPrice { get; private set; }
        public decimal SummaryMarkUp { get; private set; }
        public decimal SummaryPrice { get; private set; }
        public decimal SummaryProfit { get; private set; }
        public bool IsDetailed { get; set; }
        public ObservableCollection<ItemViewModel> Items { get; private set; } = new ObservableCollection<ItemViewModel>();
    }
    public class ItemViewModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
        public double MarkUp { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        public uint Number { get; set; }
        public decimal TotalPrice { get; set; }
        public bool CanAddDescriptionToPdf { get; set; }
        public bool CanEditDescription { get; set; }
        public ObservableCollection<DescriptionUnitViewModel> Description { get; private set; }
    }
    public class DescriptionUnitViewModel
    {
        public string Text { get; set; }
        public event EventHandler Removing;
    }
    public class CustomerViewModel
    {
        public uint KpNumber { get; set; }
        public string KpName { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Organization { get; set; }
        public string Location { get; set; }
    }
    public class MainViewModel
    {
        public decimal SummaryPrice { get; private set; }
        public decimal SummaryCostPrice { get; private set; }
        public decimal SummaryMarkUp { get; private set; }
        public decimal SummaryProfit { get; private set; }
        public CustomerViewModel Customer { get; private set; }
        public ObservableCollection<GroupViewModel> Groups { get; private set; }
        public List<User> Users { get; private set; }
        public User SelectedUser { get; set; }
        public Notification Notification { get; set; }
        public bool IsDetailed { get; set; }
        public bool CanAddSummaryPage { get; set; }
        public bool CanAddDescription { get; set; }
        public int SelectedUserIndex { get; set; }
        public ImageSource SelectedBanner { get; set; }
    }
}
