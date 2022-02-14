using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        AdminPanel adminPanel;
        private IDialogCoordinator dialogCoordinator;

        public override void InitializeViewModel()
        {
            adminPanel = (AdminPanel)model;
            dialogCoordinator = ((Views.AdminPanel)view).dialogCoordinator;
        }

        public RelayCommand UserDeleteCommand { get => new RelayCommand(obj =>
        {
            UserDelete((User)obj);
        }); }

        async void UserDelete(User user)
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Удалить",
                NegativeButtonText = "Отмена"
            };
            var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "Удаление пользователя","Удалить пользователя из базы данных?", 
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            if (dialogRes == MessageDialogResult.Affirmative)
                adminPanel.UserDelete(user);
        }

        public User User { get => adminPanel.User; }

        public User SelectedUser 
        { 
            get => adminPanel.SelectedUser;
            set => adminPanel.SelectedUser = value;
        }

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

        public Position NewUserSelectedPosition
        {
            get => adminPanel.NewUserSelectedPosition;
            set
            {
                adminPanel.NewUserSelectedPosition = value;
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

        public void ClearPwdNewUserPasswordTextBox() => ((Views.AdminPanel)view).ClearPwdNewUserPasswordTextBox();

        public string NewUserPassword
        {
            get => adminPanel.NewUserPassword;
            set
            {
                adminPanel.NewUserPassword = value;
                OnPropertyChanged();
            }
        }

        public User NewUser
        {
            get => adminPanel.NewUser;
            set
            {
                adminPanel.NewUser = value;
                OnPropertyChanged();
            }
        }

        #region Change password

        public string NewAccountPassword {set => adminPanel.NewAccountPassword = value;}

        public string NewAccountPasswordRepeat { set => adminPanel.NewAccountPasswordRepeat = value; }

        public string OldAccountPassword { set => adminPanel.OldAccountPassword = value; }

        public bool IsCanControlUsers
        {
            get
            {
                if (User.Position.Permissions.Contains(Shared.Permissions.CanAll)
                    || User.Position.Permissions.Contains(Shared.Permissions.CanControlUsers))
                    return true;
                return false;
            }
        }

        #endregion Change password
    }
}
