using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.ViewModels
{
    public class HelloViewModel : BaseViewModel
    {
        Hello hello;
        private IDialogCoordinator dialogCoordinator;

        public override void InitializeViewModel()
        {
            hello = (Hello)model;
            dialogCoordinator = ((Views.Hello)view).dialogCoordinator;
        }

        public string LogoPath { get => hello.LogoPath; }

        public string Login
        {
            get => hello.Login;
            set => hello.Login = value;
        }

        public bool IsRememberMe
        {
            get => hello.IsRememberMe;
            set => hello.IsRememberMe = value;
        }

        public string Pwd
        {
            get => hello.Pwd;
            set => hello.Pwd = value;
        }

        public bool IsBusy
        {
            get => hello.IsBusy;
            set => hello.IsBusy = value;
        }

        public string HelloStatus
        {
            get => hello.HelloStatus;
            set => hello.HelloStatus = value;
        }
    }
}
