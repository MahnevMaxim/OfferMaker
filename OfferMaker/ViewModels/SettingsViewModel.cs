using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        Settings settingsModel;

        public override void InitializeViewModel()
        {
            settingsModel = (Settings)model;
        }

        #region Properties

        public List<string> Themes
        {
            get { return settingsModel.Themes; }
            set
            {
                settingsModel.Themes = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTheme
        {
            get
            {
                return settingsModel.SelectedTheme;
            }
            set
            {
                settingsModel.SelectedTheme = value;
                OnPropertyChanged();
            }
        }

        public int AppMode
        {
            get
            {
                return (int)settingsModel.AppMode;
            }
            set
            {
                settingsModel.AppMode = (AppMode)value;
                OnPropertyChanged();
            }
        }

        public string LightOrDark
        {
            get
            {
                return settingsModel.LightOrDark;
            }
            set
            {
                settingsModel.LightOrDark = value;
                OnPropertyChanged();
            }
        }

        public List<string> LightOrDarkList
        {
            get
            {
                return settingsModel.LightOrDarkList;
            }
            set
            {
                settingsModel.LightOrDarkList = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}
