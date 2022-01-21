using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared
{
    public class User : IUser
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public Image Image { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        public Account Account { get; set; }
    }
}
