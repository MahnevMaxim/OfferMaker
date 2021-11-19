using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public interface IView
    {
        void Show();

        bool? ShowDialog();

        void OnSendMessage(string message);

        void Close();
    }
}
