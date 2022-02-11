using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Shared;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using ApiLib;
using System.Text.Json;
using Newtonsoft.Json;

namespace OfferMaker
{
    class EntryPoint
    {
        readonly string apiEndpoint = Global.apiEndpoint;

        internal void Run()
        {
            //настройка для логирования необработанных исключений
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(Log.AppDomain_CurrentDomain_UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Log.Application_ThreadException);

            Log.Clear();

            Hello hello = new Hello();
            hello.Run();
            MvvmFactory.CreateWindow(hello, new ViewModels.HelloViewModel(), new Views.Hello(), ViewMode.ShowDialog);
            if(hello.isAuthorizationOk)
                MvvmFactory.CreateWindow(hello.GetMain(), new ViewModels.MainViewModel(), new Views.MainWindow(), ViewMode.Show);
            else
                Application.Current.Shutdown();
        }
    }
}
