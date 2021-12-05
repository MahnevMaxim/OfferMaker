using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace OfferMaker
{
    abstract public class BaseModel : INotifyPropertyChanged
    {
        public BaseViewModel viewModel;
        delegate void MethodContainer();

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
        internal virtual void Run() { }

        internal void SendCommand(object parameters)
        {
            MethodContainer mc;
            if (parameters is string) mc = () => GetType().GetMethod(parameters.ToString())?.Invoke(this, null);
            else
            {
                List<object> params_ = (List<object>)parameters;
                mc = params_.Count switch
                {
                    2 => () => GetType().GetMethod(params_[0] as string)?.Invoke(this, new object[] { params_[1] }),
                    3 => () => GetType().GetMethod(params_[0] as string)?.Invoke(this, new object[] { params_[1], params_[2] }),
                    4 => () => GetType().GetMethod(params_[0] as string)?.Invoke(this, new object[] { params_[1], params_[2], params_[3] }),
                    _ => () => throw new NotImplementedException()
                };
            }
            mc?.Invoke();
        }

        /// <summary>
        /// Кто знает более правильный метод закрывания окна из модели - может поделиться.
        /// </summary>
        protected void Close() => viewModel.view.Close();

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
                L.LW(GetType().Name + " " + prop);
            }
            viewModel?.OnPropertyChanged(prop);
        }

        #endregion
    }
}
