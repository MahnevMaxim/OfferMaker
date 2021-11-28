using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public class Customer : BaseEntity
    {
        string fullName;
        string position;
        string location;
        string organization;

        public int Id { get; set; }

        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                OnPropertyChanged();
            }
        }

        public string Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged();
            }
        }

        public string Organization
        {
            get => organization;
            set
            {
                organization = value;
                OnPropertyChanged();
            }
        }
    }
}
