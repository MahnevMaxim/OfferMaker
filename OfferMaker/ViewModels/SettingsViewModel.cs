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

        async internal Task TryClose()
        {
            if (IsBusy)
            {
                var dialogSettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Прервать",
                    NegativeButtonText = "Продолжить",
                };
                var dialogRes = await dialogCoordinator.ShowMessageAsync(this, "Загрузка изображений", "Прервать или продолжить?",
                    MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                if (dialogRes == MessageDialogResult.Affirmative)
                {
                    await Task.Run(() => settingsModel.TryClose());
                }
            }
        }

        #region Properties

        public bool IsBusy { get => settingsModel.IsBusy; }

        public List<string> Themes
        {
            get => settingsModel.Themes;
            set
            {
                settingsModel.Themes = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTheme
        {
            get => settingsModel.SelectedTheme;
            set
            {
                settingsModel.SelectedTheme = value;
                OnPropertyChanged();
            }
        }

        public int AppMode
        {
            get => (int)settingsModel.AppMode;
            set
            {
                settingsModel.AppMode = (AppMode)value;
                OnPropertyChanged();
            }
        }

        public string LightOrDark
        {
            get => settingsModel.LightOrDark;
            set
            {
                settingsModel.LightOrDark = value;
                OnPropertyChanged();
            }
        }

        public List<string> LightOrDarkList
        {
            get => settingsModel.LightOrDarkList;
            set
            {
                settingsModel.LightOrDarkList = value;
                OnPropertyChanged();
            }
        }

        public int BeginFilesCount { get => settingsModel.BeginFilesCount; }

        public int CopiedFilesCount { get => settingsModel.CopiedFilesCount; }

        public int LeftFilesCount { get => BeginFilesCount - CopiedFilesCount; }

        public int ErrorFilesCount { get => settingsModel.ErrorFilesCount; }

        public int CopyProgress
        {
            get
            {
                if (BeginFilesCount == 0) return 0;
                int progress = CopiedFilesCount * 100 / BeginFilesCount;
                return progress;
            }
            set { }
        }

        public string CopyStatus { get => settingsModel.CopyStatus; }

        public int ContentWidth
        {
            get => settingsModel.ContentWidth;
            set => settingsModel.ContentWidth = value;
        }

        public int AdvertisingWidth
        {
            get => settingsModel.AdvertisingWidth;
            set => settingsModel.AdvertisingWidth = value;
        }

        public double PdfControlWidth
        {
            get => settingsModel.PdfControlWidth;
            set => settingsModel.PdfControlWidth = value;
        }

        #endregion Properties
    }
}
