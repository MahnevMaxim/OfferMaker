using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class ArchiveFilter
    {
        ObservableCollection<Offer> offers;
        User currentUser;

        public int? Id { get; set; }

        public bool IsShowOnlyCurrentUser { get; set; }

        public DateTime? BeginDateTime 
        { 
            get; 
            set; 
        }

        public DateTime? EndDateTime 
        { 
            get; 
            set; 
        }

        public string CustomerName { get; set; }

        public string CompanyName { get; set; }

        public string City { get; set; }

        public User Creator { get; set; }

        public User Manager { get; set; }

        public string ArchiveSearchByNameText { get; set; }

        public ArchiveFilter(ObservableCollection<Offer> offers, User currentUser)
        {
            this.offers = offers;
            this.currentUser = currentUser;
            BeginDateTime = offers?.OrderBy(o => o.Id).FirstOrDefault()?.CreateDate;
            EndDateTime = offers?.OrderByDescending(o => o.Id).FirstOrDefault()?.CreateDate;
        }

        ArchiveFilter() { } //закрываем конструктор

        internal ObservableCollection<Offer> GetFilteredOffers()
        {
            if(offers.Count==0)
                return new ObservableCollection<Offer>();

            DateTime endDatetimeWithDay;
            if (EndDateTime!=null)
            {
                //иначе последнее число в результаты не будет включена, так как там время 00:00:00
                endDatetimeWithDay = EndDateTime.Value.AddDays(1);
            }
            else
            {
                endDatetimeWithDay = (DateTime)offers?.OrderByDescending(o => o.Id).FirstOrDefault()?.CreateDate;
            }

            if (IsFilterEmpty()) return offers;
            List<Offer> tempOffers = new List<Offer>();
            offers.ToList().ForEach(o => tempOffers.Add(o));

            if (IsShowOnlyCurrentUser)
            {
                var res = tempOffers.Where(o => o.OfferCreator?.Id != null && o.OfferCreator.Id == currentUser.Id).ToList();
                tempOffers = res;
            }

            if (Id != null)
            {
                var res = offers.Where(o => o.Id == Id).FirstOrDefault();
                if (res != null)
                    return new ObservableCollection<Offer>() { res };
                else
                    return new ObservableCollection<Offer>();
            }

            if (BeginDateTime != null)
            {
                var res = tempOffers.Where(o => o.CreateDate >= BeginDateTime).ToList();
                tempOffers = res;
            }

            if (endDatetimeWithDay != null)
            {
                var res = tempOffers.Where(o => o.CreateDate <= endDatetimeWithDay).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(CustomerName?.ToLower()))
            {
                var res = tempOffers.Where(o => o.Customer.FullName != null && o.Customer.FullName.ToLower().Contains(CustomerName)).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(CompanyName?.ToLower()))
            {
                var res = tempOffers.Where(o => o.Customer.Organization != null && o.Customer.Organization.ToLower().Contains(CompanyName)).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(City?.ToLower()))
            {
                var res = tempOffers.Where(o => o.Customer.Location != null && o.Customer.Location.ToLower().Contains(City)).ToList();
                tempOffers = res;
            }

            if (Creator != null)
            {
                var res = tempOffers.Where(o => o.OfferCreator?.Id !=null && o.OfferCreator.Id == Creator.Id).ToList();
                tempOffers = res;
            }

            if (Manager != null)
            {
                var res = tempOffers.Where(o => o.Manager.Id == Manager.Id).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(ArchiveSearchByNameText?.ToLower()))
            {
                var res = tempOffers.Where(o => o.OfferName != null && o.OfferName.ToLower().Contains(ArchiveSearchByNameText.ToLower())).ToList();
                tempOffers = res;
            }

            return new ObservableCollection<Offer>(tempOffers);
        }

        private bool IsFilterEmpty()
        {
            ArchiveFilter emptyFilter = new ArchiveFilter(offers, currentUser);
            if (emptyFilter.ArchiveSearchByNameText != ArchiveSearchByNameText) return false;
            if (emptyFilter.IsShowOnlyCurrentUser != IsShowOnlyCurrentUser) return false;
            if (emptyFilter.Id != Id) return false;
            if (emptyFilter.BeginDateTime != BeginDateTime) return false;
            if (emptyFilter.EndDateTime != EndDateTime) return false;
            if (emptyFilter.CustomerName != CustomerName) return false;
            if (emptyFilter.CompanyName != CompanyName) return false;
            if (emptyFilter.City != City) return false;
            if (Creator != null) return false;
            if (Manager != null) return false;
            return true;
        }
    }
}
