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

        public string NewUserLastName { get; set; }

        public string NewUserFirstName { get; set; }

        public string NewUserPassword { get; set; }

        public string NewUserEmail { get; set; }

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

        async public void PositionsSave()
        {
            foreach (var position in Positions)
                position.SavePermissions();
            CallResult cr = await Global.Main.DataRepository.PositionsSave(Positions);
            OnSendMessage(cr.Message);
        }

        #endregion Positions

        async public void SaveUser()
        {
            CallResult cr = await Global.Main.DataRepository.UserSave(User);
            OnSendMessage(cr.Message);
        }

        public void EditPhoto()
        {
            string path = Helpers.GetFilePath("Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp");
            if (path != null)
            {
                Image image = new Image(Guid.NewGuid().ToString(), Global.User.Id, path) { IsNew = true };
                Global.ImageManager.Add(image);
                User.Image = image;
            }
        }

        async public void UserAdd()
        {
            if (string.IsNullOrWhiteSpace(NewUserLastName)  || string.IsNullOrWhiteSpace(NewUserFirstName)
                || string.IsNullOrWhiteSpace(NewUserEmail) || string.IsNullOrWhiteSpace(NewUserPassword))
            {
                OnSendMessage("Фамилия, имя, email и пароль должны быть заполнены.");
                return;
            }

            User user = new User() { LastName = NewUserLastName, FirstName = NewUserFirstName, Email = NewUserEmail, Pwd = NewUserPassword };
            CallResult<User> cr = await Global.Main.DataRepository.UserAdd(user);
            if (cr.Success)
                Users.Add(user);
            OnSendMessage(cr.Message);
        }

        async public void UsersEdit()
        {
            CallResult cr = await Global.Main.DataRepository.UsersEdit(Users);
            OnSendMessage(cr.Message);
        }
    }
}
