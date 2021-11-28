using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

namespace OfferMaker
{
    public class User : BaseEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get => FirstName + " " + LastName; }

        public ObservableCollection<Permissions> Permissions { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string Email { get; set; }

        public string PhotoPath { get; set; }

        public override string ToString() => FullName;
    }
}
