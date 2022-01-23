using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace OfferMaker.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        Settings settingsModel;
        private IDialogCoordinator dialogCoordinator;

        public override void InitializeViewModel()
        {
            settingsModel = (Settings)model;
            dialogCoordinator = ((Views.Settings)view).dialogCoordinator;
        }

        public RelayCommand ClearCacheCommand
        {
            get => new RelayCommand(obj =>
            {
                ClearCache();
            });
        }

        async void ClearCache()
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет"
            };
            var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "Очистка кэша", "Удалить все кэшированные данные?",
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            if (dialogRes == MessageDialogResult.Affirmative)
                settingsModel.ClearCache();
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
