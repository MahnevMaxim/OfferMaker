using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace OfferMaker
{
    abstract public class BaseEntity : INotifyPropertyChanged
    {
        internal bool IsPropertyChangedNull { get => PropertyChanged == null; }

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
                Log.Write(GetType().Name + " " + prop);
            }  
        }

        #endregion
    }
}
