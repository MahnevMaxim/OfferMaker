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
        string photoPath;
        Image image;
        Position position;

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get => FirstName + " " + LastName; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string Email { get; set; }

        public string Pwd { get; set; }

        public string PhotoPath 
        { 
            get => photoPath; 
            set => photoPath = value; 
        }

        public Image Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public Position Position
        { 
            get=> position; 
            set
            {
                position = value;
                OnPropertyChanged();
            }
        }

        public override string ToString() => FullName;
    }
}
