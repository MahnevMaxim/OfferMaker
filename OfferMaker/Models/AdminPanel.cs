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

        public User User 
        { 
            get; 
            set; 
        }

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

        public static AdminPanel GetInstance(ObservableCollection<User> users, User user)
        {
            instance.Users = users;
            instance.User = user;
            return instance;
        }

        #endregion Singleton

        public void AddPosition()
        {
            if(string.IsNullOrWhiteSpace(NewPositionName))
            {
                OnSendMessage("Название должности не может быть пустым");
                return;
            }
            Position pos = new Position(NewPositionName);
            Positions.Add(pos);
            SelectedPosition = pos;
        }

        public void RemovePosition(Position pos)
        {
            Positions.Remove(pos);
            if (Positions.Count > 0)
                SelectedPosition = Positions[0];
        }

        async public void SavePositions()
        {
            foreach(var position in Positions)
            {
                position.SavePermissions();
                CallResult cr =  await Global.Main.DataRepository.SavePosition(position);
                if (cr.Success)
                    OnSendMessage("Должность "+ position.PositionName + " сохранена.");
                else
                    OnSendMessage(cr.Error.Message);
            }
        }
    }
}
