using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OfferMaker.MvvmFactory
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        BaseModel model;
        public IView view;

        public void SetModel(BaseModel model, IView view)
        {
            this.view = view;
            this.model = model;
            SendCommand += model.SendCommand;
            InitializeViewModel();
        }
        
        /// <summary>
        /// Если что-то дополнительно надо инициализировать,
        /// то делать это здесь.
        /// </summary>
        virtual public void InitializeViewModel() { }

        #region Commands

        public delegate void CommandHandler(object parameters);
        public event CommandHandler SendCommand;

        public RelayCommand Cmd { get => new RelayCommand(obj => SendCommand(obj)); }

        #endregion

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
