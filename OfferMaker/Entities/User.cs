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
        string firstName;
        string lastName;
        string phoneNumber1;
        string phoneNumber2;
        Image image;
        Position position;

        public int Id { get; set; }

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string FullName { get => FirstName + " " + LastName; }

        public string PhoneNumber1
        {
            get => phoneNumber1;
            set
            {
                phoneNumber1 = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber2
        {
            get => phoneNumber2;
            set
            {
                phoneNumber2 = value;
                OnPropertyChanged();
            }
        }

        public string Email { get; set; }

        public string Pwd { get; set; }

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

        public Account Account { get; set; }

        public override string ToString() => FullName;
    }
}
