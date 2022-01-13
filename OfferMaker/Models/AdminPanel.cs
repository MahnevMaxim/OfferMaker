using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Shared;

namespace OfferMaker
{
    public class AdminPanel : BaseModel
    {
        int menuSelectedIndex;
        Position selectedPosition;
        User selectedUser;
        User newUser = new User() { Image = Global.NoProfileImage };

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<Position> Positions { get; set; } = new ObservableCollection<Position>();

        public Position SelectedPosition
        {
            get => selectedPosition;
            set
            {
                selectedPosition = value;
                OnPropertyChanged();
            }
        }

        public User User { get; set; }

        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                selectedUser = value;
                OnPropertyChanged();
            }
        }

        #region New user

        public User NewUser
        {
            get => newUser;
            set
            {
                newUser = value;
                OnPropertyChanged();
            }
        }

        public string NewUserPassword { get; set; }

        #endregion New user

        #region Change password

        public string NewAccountPassword { get; set; }

        public string NewAccountPasswordRepeat { get; set; }

        public string OldAccountPassword { get; set; }

        #endregion Change password

        public string NewPositionName { get; set; }

        public int MenuSelectedIndex
        {
            get => menuSelectedIndex;
            set
            {
                menuSelectedIndex = value;
                OnPropertyChanged();
            }
        }

        #region Singleton

        private AdminPanel() { }

        private static readonly AdminPanel instance = new AdminPanel();

        public static AdminPanel GetInstance(ObservableCollection<User> users, User user, ObservableCollection<Position> positions)
        {
            instance.Users = users;
            instance.User = user;
            instance.Positions = positions;
            return instance;
        }

        #endregion Singleton

        #region Positions

        async public void AddPosition()
        {
            if (string.IsNullOrWhiteSpace(NewPositionName))
            {
                OnSendMessage("Название должности не может быть пустым");
                return;
            }
            Position pos = new Position(NewPositionName);
            CallResult<Position> cr = await Global.Main.DataRepository.PositionAdd(pos);
            if (cr.Success)
            {
                Positions.Add(pos);
                SelectedPosition = pos;
            }
            OnSendMessage(cr.Message);
        }

        async public void RemovePosition(Position pos)
        {
            CallResult cr = await Global.Main.DataRepository.PositionDelete(pos);
            if (cr.Success)
            {
                Positions.Remove(pos);
                if (Positions.Count > 0)
                    SelectedPosition = Positions[0];
            }
            OnSendMessage(cr.Message);
        }

        async public void PositionsEdit()
        {
            foreach (var position in Positions)
                position.SavePermissions();
            CallResult cr = await Global.Main.DataRepository.PositionsEdit(Positions);
            OnSendMessage(cr.Message);
        }

        #endregion Positions

        async public void UserSelfChangePassword()
        {
            if (string.IsNullOrWhiteSpace(NewAccountPassword))
            {
                OnSendMessage("Введите новый пароль");
                return;
            }

            if (NewAccountPassword != NewAccountPasswordRepeat)
            {
                OnSendMessage("Подтверждение не совпадает с паролем");
                return;
            }

            if (string.IsNullOrWhiteSpace(OldAccountPassword))
            {
                OnSendMessage("Введите пароль");
                return;
            }

            User.Pwd = NewAccountPassword;
            CallResult cr = await Global.Main.DataRepository.UserSelfChangePassword(User, OldAccountPassword);
            OnSendMessage(cr.Message);
        }

        async public void UserSelfEdit()
        {
            if (string.IsNullOrWhiteSpace(User.Pwd))
            {
                OnSendMessage("Введите пароль");
                return;
            }
            CallResult cr = await Global.Main.DataRepository.UserSelfEdit(User);
            OnSendMessage(cr.Message);
        }

        public void CurrentUserPhotoEdit() => UserPhotoEdit(User);

        public void SelectedUserPhotoEdit() => UserPhotoEdit(SelectedUser);

        public void NewUserPhotoEdit() => UserPhotoEdit(NewUser);

        public void UserPhotoEdit(User user)
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
            {
                Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                Global.ImageManager.Add(image);
                user.Image = image;
            }
        }

        async public void UserCreate()
        {
            if (string.IsNullOrWhiteSpace(NewUser.LastName) || NewUser.LastName.Length < 2)
            {
                OnSendMessage("Фамилия должна быть заполнена и содержать минимум 2 символа.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.FirstName) || NewUser.FirstName.Length < 2)
            {
                OnSendMessage("Имя должно быть заполнено и содержать минимум 2 символа.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.Email) || !EmailValidator.IsValidEmail(NewUser.Email))
            {
                OnSendMessage("Email должен быть заполнен и валиден.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUserPassword) || NewUserPassword.Length < 8)
            {
                OnSendMessage("Пароль должен быть заполнен и содержать мимнимум 8 символов.");
                return;
            }

            NewUser.Pwd = NewUserPassword;
            CallResult<User> cr = await Global.Main.DataRepository.UserCreate(NewUser);
            if (cr.Success)
            {
                Users.Add(cr.Data);
                NewUser = new User() { Image = Global.NoProfileImage };
                ((ViewModels.AdminPanelViewModel)viewModel).ClearPwdNewUserPasswordTextBox();
                MenuSelectedIndex = 1;
            }
            OnSendMessage(cr.Message);
        }

        async public void UsersEdit()
        {
            CallResult cr = await Global.Main.DataRepository.UsersEdit(Users);
            OnSendMessage(cr.Message);
        }

        async public void SkipSelectUserPassword(User user)
        {
            SimpleViews.NewPasswordForm form = new SimpleViews.NewPasswordForm();
            var res = form.ShowDialog();
            if (res == true)
            {
                user.Pwd = form.passwordTextBox.Password;
                CallResult cr = await Global.Main.DataRepository.UserChangePassword(user);
                OnSendMessage(cr.Message);
            }
        }

        async public void UserDelete(User user)
        {
            CallResult cr = await Global.Main.DataRepository.UserDelete(user);
            if (cr.Success)
            {
                Users.Remove(user);
                if (Users.Count > 0)
                    SelectedUser = Users[0];
            }
            OnSendMessage(cr.Message);
        }
    }
}
