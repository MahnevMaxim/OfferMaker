using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker.MvvmFactory
{
    public interface IView
    {
        void Show();

        void OnSendMessage(string message);
    }
}
