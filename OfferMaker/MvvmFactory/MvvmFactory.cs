using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;

namespace OfferMaker.MvvmFactory
{
    class MvvmFactory
    {
        public static void CreateWindow(BaseModel model, BaseViewModel viewModel, IView view)
        {
            viewModel.SetModel(model);
            (view as MetroWindow).DataContext = viewModel;
            model.SendMessage += (object sender, string e) => view.OnSendMessage(e);
            view.Show();
            model.Run();
        }
    }
}
