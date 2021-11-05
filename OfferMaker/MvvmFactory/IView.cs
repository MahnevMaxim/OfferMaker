using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker.MvvmFactory
{
    interface IView
    {
        void Show();

        void OnSendMessage(string message);
    }
}
