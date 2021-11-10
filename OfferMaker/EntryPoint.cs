using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OfferMaker
{
    class EntryPoint
    {
        internal void Run()
        {
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(L.AppDomain_CurrentDomain_UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(L.Application_ThreadException);
            MvvmFactory.CreateWindow(new Main(), new ViewModels.MainViewModel(), new Views.MainWindow());
        }
    }
}
