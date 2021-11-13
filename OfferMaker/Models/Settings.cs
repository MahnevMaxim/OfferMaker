using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlzEx.Theming;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace OfferMaker
{
    public class Settings : BaseModel
    {
        #region MVVM

        #region Fields

        string selectedTheme;
        bool isOffline;
        string lightOrDark;

        #endregion Fields

        #region Properties

        public string SelectedTheme
        {
            get
            {
                return selectedTheme;
            }
            set
            {
                selectedTheme = value;
                UpdateSettings();
                OnPropertyChanged();
            }
        }

        public bool IsOffline
        {
            get
            {
                return isOffline;
            }
            set
            {
                isOffline = value;
                UpdateSettings();
                OnPropertyChanged();
            }
        }

        public string LightOrDark
        {
            get => lightOrDark == null ? "Light" : lightOrDark;
            set
            {
                lightOrDark = value;
                UpdateSettings();
                OnPropertyChanged();
                OnPropertyChanged(nameof(AdditionalColor));
            }
        }

        public string AdditionalColor { get => LightOrDark == "Light" ? "#FFE5E5E5" : "#FF313131" ; }

        public List<string> Themes { get; set; } = new List<string>();

        public List<string> LightOrDarkList { get; set; }

        #endregion Properties

        #endregion MVVM

        #region Singleton

        private static readonly Settings instance = new Settings();

        public static Settings GetInstance() => instance;

        private Settings()
        {
            ThemeManager.Current.Themes.Where(t => t.BaseColorScheme == "Light")
                .Select(t => t.ColorScheme).ToList().ForEach(t => Themes.Add(t)); //просто получаем список тем
            LightOrDarkList = new List<string>() { "Light", "Dark" };
            isOffline = AppSettings.Default.IsOnlyOffline;
            selectedTheme = AppSettings.Default.Theme;
            lightOrDark = AppSettings.Default.LightOrDark;
            SetSettings();
        }

        #endregion Singleton

        private void UpdateSettings()
        {
            AppSettings.Default.IsOnlyOffline = IsOffline;
            if (LightOrDark != null) 
                AppSettings.Default.LightOrDark = LightOrDark;
            if (LightOrDark != null) 
                AppSettings.Default.Theme = SelectedTheme;
            AppSettings.Default.Save();
            SetSettings();
        }

        public void SetSettings()
        {
            if (string.IsNullOrEmpty(LightOrDark) || string.IsNullOrEmpty(SelectedTheme)) return;
            string theme = LightOrDark + "." + SelectedTheme;
            Application.Current.Resources["DataGrid.EvenRow.Background"] = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); 
            Application.Current.Resources["DataGrid.OddRow.Background"] = new SolidColorBrush(Color.FromArgb(255, 115, 194, 251));
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }
    }
}
