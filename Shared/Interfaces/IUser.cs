using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Shared
{
    public interface IUser
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Pwd { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ObservableCollection<Permissions> Permissions { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string PhotoPath { get; set; }
    }
}
