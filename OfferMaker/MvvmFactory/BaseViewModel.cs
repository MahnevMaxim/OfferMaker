using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OfferMaker
{
    public abstract class BaseViewModel : BaseEntity
    {
        public BaseModel model;
        public IView view;

        public void SetModel(BaseModel model, IView view)
        {
            this.view = view;
            this.model = model;
            this.model.viewModel = this;
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
    }
}
