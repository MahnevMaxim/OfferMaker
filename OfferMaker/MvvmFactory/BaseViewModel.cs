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

        public void SetModel(BaseModel model) => this.model = model;

        #region Commands

        public delegate void CommandHandlerNew(object parameters);
        public event CommandHandlerNew SendCommand;

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
