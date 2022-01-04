using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        AdminPanel adminPanel;

        public override void InitializeViewModel()
        {
            adminPanel = (AdminPanel)model;
        }

        public User User { get => adminPanel.User; }

        public ObservableCollection<User> Users { get => adminPanel.Users; }

        public ObservableCollection<Position> Positions { get => adminPanel.Positions; }

        public Position SelectedPosition
        {
            get => adminPanel.SelectedPosition;
            set
            {
                adminPanel.SelectedPosition = value;
                OnPropertyChanged();
            }
        }

        public int MenuSelectedIndex
        {
            get => adminPanel.MenuSelectedIndex;
            set
            {
                adminPanel.MenuSelectedIndex = value;
                OnPropertyChanged();
            }
        }

        public string NewPositionName
        {
            get => adminPanel.NewPositionName;
            set
            {
                adminPanel.NewPositionName = value;
                OnPropertyChanged();
            }
        }

        public string NewUserLastName
        {
            get => adminPanel.NewUserLastName;
            set
            {
                adminPanel.NewUserLastName = value;
                OnPropertyChanged();
            }
        }

        public string NewUserFirstName
        {
            get => adminPanel.NewUserFirstName;
            set
            {
                adminPanel.NewUserFirstName = value;
                OnPropertyChanged();
            }
        }

        public string NewUserPassword
        {
            get => adminPanel.NewUserPassword;
            set
            {
                adminPanel.NewUserPassword = value;
                OnPropertyChanged();
            }
        }

        public string NewUserEmail
        {
            get => adminPanel.NewUserEmail;
            set
            {
                adminPanel.NewUserEmail = value;
                OnPropertyChanged();
            }
        }
    }
}
