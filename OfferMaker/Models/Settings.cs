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
    public enum AppMode
    {
        Auto,
        Offline,
        Online
    }

    public class Settings : BaseModel
    {
        #region MVVM

        #region Fields

        string selectedTheme;
        AppMode appMode;
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

        public AppMode AppMode
        {
            get
            {
                return appMode;
            }
            set
            {
                appMode = value;
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
            appMode = (AppMode)AppSettings.Default.AppMode;
            selectedTheme = AppSettings.Default.Theme;
            lightOrDark = AppSettings.Default.LightOrDark;
            SetSettings();
        }

        #endregion Singleton

        private void UpdateSettings()
        {
            AppSettings.Default.AppMode = (int)appMode;
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

        public static void SetDefaultBanner(string path)
        {
            AppSettings.Default.DefaultBanner = path;
            AppSettings.Default.Save();
        }

        public static void SetIsRememberMe(bool isRememberMe)
        {
            AppSettings.Default.IsRememberMe = isRememberMe;
            AppSettings.Default.Save();
        }

        public static void SetToken(string accessToken)
        {
            if (!AppSettings.Default.IsRememberMe) return;
            AppSettings.Default.AccessToken = accessToken;
            AppSettings.Default.Save();
        }

        internal static void SetLogin(string login)
        {
            AppSettings.Default.Login = login;
            AppSettings.Default.Save();
        }

        public static string GetDefaultBanner() => AppSettings.Default.DefaultBanner;

        public static int GetMaxInfoblocksCount() => AppSettings.Default.MaxInfoblocksCount;

        public static bool GetIsRememberMe() => AppSettings.Default.IsRememberMe;

        public static string GetToken() => AppSettings.Default.AccessToken;

        public static string GetLogin() => AppSettings.Default.Login;

        internal void SkipUserSettings()
        {
            AppSettings.Default.Login=null;
            AppSettings.Default.AccessToken = null;
            AppSettings.Default.IsRememberMe = false;
            AppSettings.Default.Save();
        }
    }
}
