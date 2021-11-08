using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using System.Windows;

namespace OfferMaker.ViewModels
{
    public class MainViewModel : MvvmFactory.BaseViewModel
    {
        override public void InitializeViewModel()
        {
            //ThemeManager.Current.ChangeTheme(Application.Current, "Light.Green");
        }

    }
}
