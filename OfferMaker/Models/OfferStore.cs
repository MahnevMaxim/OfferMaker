using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public enum ArchiveMode { ShowOffers, ShowTemplate }

    public class OfferStore
    {
        ObservableCollection<Offer> filteredOffers = new ObservableCollection<Offer>();
        ObservableCollection<Offer> offers;
        User currentUser;
        ArchiveMode archiveMode;

        public ObservableCollection<Offer> FilteredOffers
        {
            get => filteredOffers;
            set
            {
                filteredOffers = value;
            }
        }

        public ObservableCollection<Offer> Offers
        {
            get => offers;
            set
            {
                offers = value;
            }
        }

        #region Поля фильтра

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

        public string OfferSearchByNameText { get; set; }

        #endregion Поля фильтра

        public OfferStore(ObservableCollection<Offer> offers, User currentUser)
        {
            this.offers = offers;
            this.currentUser = currentUser;
            BeginDateTime = null;
            EndDateTime = null;
        }

        public OfferStore(ObservableCollection<Offer> offers, ObservableCollection<Offer> offersHistory, User currentUser)
        {
            ObservableCollection<Offer> offers_ = new ObservableCollection<Offer>();
            for (int i=0;i<offers.Count;i++)
            {
                string guid = offers[i].Guid;
                var history = offersHistory.Where(o => o.ParentGuid == guid);
                if(history.Count()==0)
                {
                    offers_.Add(offers[i]);
                }
                else
                {
                    var lastVersion = history.Last();
                    lastVersion.IsHaveHistory = true;
                    offers[i].IsHistory = true;
                    lastVersion.OfferHistory.Add(offers[i]);
                    lastVersion.OfferHistory.AddRange(history.Take(history.Count()-1));
                    lastVersion.OfferHistory.ForEach(o => o.IsHistory = true);
                    offers_.Add(lastVersion);
                }
            }
            this.offers = offers_;
            this.currentUser = currentUser;
            BeginDateTime = null;
            EndDateTime = null;
        }

        OfferStore() { } //закрываем конструктор

        internal void ApplyOfferFilter()
        {
            if (offers.Count == 0)
            {
                FilteredOffers.Clear(); //при удалении последнего КП надо очистить коллекцию 
                return;
            }

            DateTime endDatetimeWithDay;
            if (EndDateTime != null)
            {
                //иначе последнее число в результаты не будет включена, так как там время 00:00:00
                endDatetimeWithDay = EndDateTime.Value.AddDays(1);
            }
            else
            {
                endDatetimeWithDay = (DateTime)offers?.OrderByDescending(o => o.Id).FirstOrDefault()?.CreateDate;
            }

            List<Offer> tempOffers = new List<Offer>();
            offers.ToList().ForEach(o => tempOffers.Add(o));

            if (IsFilterEmpty())
            {
                FilteredOffers.Clear();
                tempOffers.ForEach(o => filteredOffers.Add(o));
                return;
            }

            if (IsShowOnlyCurrentUser)
            {
                var res = tempOffers.Where(o => o.OfferCreator?.Id != null && o.OfferCreator.Id == currentUser.Id).ToList();
                tempOffers = res;
            }

            if (Id != null)
            {
                var res = offers.Where(o => o.Id == Id).FirstOrDefault();
                if (res != null)
                {
                    FilteredOffers.Clear();
                    FilteredOffers.Add(res);
                }
                else
                    FilteredOffers.Clear();
                return;
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

            if (!string.IsNullOrWhiteSpace(CustomerName?.ToLower().Trim()))
            {
                var res = tempOffers.Where(o => o.Customer.FullName != null && o.Customer.FullName.ToLower().Contains(CustomerName.ToLower().Trim())).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(CompanyName?.ToLower().Trim()))
            {
                var res = tempOffers.Where(o => o.Customer.Organization != null && o.Customer.Organization.ToLower().Contains(CompanyName.ToLower().Trim())).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(City?.ToLower().Trim()))
            {
                var res = tempOffers.Where(o => o.Customer.Location != null && o.Customer.Location.ToLower().Contains(City.ToLower().Trim())).ToList();
                tempOffers = res;
            }

            if (Creator != null)
            {
                var res = tempOffers.Where(o => o.OfferCreator?.Id != null && o.OfferCreator.Id == Creator.Id).ToList();
                tempOffers = res;
            }

            if (Manager != null)
            {
                var res = tempOffers.Where(o => o.Manager.Id == Manager.Id).ToList();
                tempOffers = res;
            }

            if (!string.IsNullOrWhiteSpace(OfferSearchByNameText?.ToLower().Trim()))
            {
                var res = tempOffers.Where(o => o.OfferName != null && o.OfferName.ToLower().Contains(OfferSearchByNameText.ToLower().Trim())).ToList();
                tempOffers = res;
            }
            FilteredOffers.Clear();
            tempOffers.ForEach(o => filteredOffers.Add(o));
        }

        internal void AddOffer(Offer offer)
        {
            if(offer.ParentGuid==null)
            {
                Offers.Add(offer);
            }
            else
            {
                offer.IsHaveHistory = true;
                string parentGuid = offer.ParentGuid;
                //ищем индекс первого вхождения, в нём наша история и формируем историю
                var parent = offers.Where(o => o.Guid == parentGuid).FirstOrDefault();
                var firstChild = offers.Where(o => o.ParentGuid == parentGuid).FirstOrDefault();
                int index = 0;
                if(firstChild != null)
                {
                    index = offers.IndexOf(firstChild);
                }
                else
                {
                    index = offers.IndexOf(parent);
                }
                var offerWithHistory = offers[index];
                var history =  offerWithHistory.OfferHistory;
                var newHistory = new List<Offer>();
                history.ForEach(o => newHistory.Add(o));
                offerWithHistory.OfferHistory.Clear();
                newHistory.Add(offerWithHistory);
                offer.OfferHistory = newHistory;

                //удаляем всю хуйню и засовываем туда наш оффер, а вся хуйня у нас в истории
                offer.OfferHistory.ForEach(o=> 
                { 
                    offers.Remove(o);
                    o.IsHaveHistory = false;
                    o.IsHistory = true;
                    o.IsOpenHistory = false;
                });
                offers.Insert(index, offer);
            }
        }

        public void SetArchiveMode(ArchiveMode archiveMode) => this.archiveMode = archiveMode;

        private bool IsFilterEmpty()
        {
            OfferStore emptyFilter = new OfferStore(offers, currentUser);
            if (emptyFilter.OfferSearchByNameText != OfferSearchByNameText) return false;
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

        internal void ResetFilter()
        {
            IsShowOnlyCurrentUser = false;
            Id = null;
            BeginDateTime = null;
            EndDateTime = null;
            CustomerName = null;
            CompanyName = null;
            City = null;
            Creator = null;
            Manager = null;
            FilteredOffers.Clear();
            offers.ToList().ForEach(o => FilteredOffers.Add(o));
        }

        internal void RemoveOffer(Offer offer)
        {
            Offers.Remove(offer);
            ApplyOfferFilter();
        }

        internal void ShowHistory(Offer offer)
        {
            offer.IsOpenHistory = true;
            int index = Offers.IndexOf(offer);
            offer.OfferHistory.ForEach(o => offers.Insert(++index, o));
            ApplyOfferFilter();
        }

        internal void HideHistory(Offer offer)
        {
            offer.IsOpenHistory = false;
            string parentGuid = offer.ParentGuid;
            string offerGuid = offer.Guid;
            List<Offer> forRemove = offers.Where(o => (o.ParentGuid == parentGuid && o.Guid != offerGuid) || o.Guid==parentGuid).ToList();
            forRemove.ForEach(o => offers.Remove(o));
            ApplyOfferFilter();
        }
    }
}
