using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace OfferMaker.MvvmFactory
{
    abstract public class BaseModel : INotifyPropertyChanged
    {
        public BaseViewModel viewModel;

        #region Events

        public event EventHandler<string> SendMessage;

        /// <summary>
        /// События в дочернем классе будут работать только через обёртку
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnSendMessage(string message) => SendMessage?.Invoke(this, message);

        #endregion Events

        /// <summary>
        /// Стартовый метод
        /// </summary>
        internal abstract void Run();

        internal void SendCommand(object parameters)
        {
            if (parameters is string)
            {
                GetType().GetMethod(parameters.ToString())?.Invoke(this, null);
            }
            else
            {
                List<object> params_ = (List<object>)parameters;
                GetType().GetMethod(params_[0] as string)?.Invoke(this, new object[] { params_[1] });
            }
        }

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            viewModel?.OnPropertyChanged(prop);
        }

        #endregion
    }
}
