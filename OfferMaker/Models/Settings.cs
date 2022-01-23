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
using System.IO;
using System.Diagnostics;

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
            Color color;
            if (LightOrDark=="Dark")
                color = (Color)ColorConverter.ConvertFromString("#FF3A292B");
            else
                color = (Color)ColorConverter.ConvertFromString("#FFFFB6C1");
            Application.Current.Resources["DataGrid.LightRow.Background"] = new SolidColorBrush(color); 
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }

        public static void SetDefaultBannerGuid(string guid)
        {
            AppSettings.Default.DefaultBannerGuid = guid;
            AppSettings.Default.Save();
        }

        internal static void SavePasswordForOffline(string pwd)
        {
            AppSettings.Default.Pwd = GenerateHash(pwd);
            AppSettings.Default.Save();
        }

        public static void SetIsRememberMe(bool isRememberMe)
        {
            AppSettings.Default.IsRememberMe = isRememberMe;
            AppSettings.Default.Save();
        }

        public static void SetToken(string accessToken)
        {
            AppSettings.Default.AccessToken = accessToken;
            AppSettings.Default.Save();
        }

        internal static void SetLogin(string login)
        {
            AppSettings.Default.Login = login;
            AppSettings.Default.Save();
        }

        public static string GetDefaultBannerGuid() => AppSettings.Default.DefaultBannerGuid;

        public static int GetMaxInfoblocksCount() => AppSettings.Default.MaxInfoblocksCount;

        public static bool GetIsRememberMe() => AppSettings.Default.IsRememberMe;

        public static string GetToken() => AppSettings.Default.AccessToken;

        public static string GetLogin() => AppSettings.Default.Login;

        internal void SkipUserSettings()
        {
            AppSettings.Default.AccessToken = null;
            AppSettings.Default.IsRememberMe = false;
            AppSettings.Default.Save();
        }

        public void ShowLog()
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", "log.txt");
                return;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
                Process.Start("notepad.exe", filePath);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void ClearCache()
        {
            string res = "";
            try
            {
                if(Directory.Exists("cache"))
                {
                    Directory.Delete("cache", true);
                    res += "Изображения удалены\n";
                }
                else
                    res += "Изображения не найдены\n";
            }
            catch(Exception ex)
            {
                Log.Write(ex);
                res += "Не удалось удалить изображения\n";
            }

            try
            {
                if (Directory.Exists(LocalDataConfig.DataCacheDir))
                {
                    Directory.Delete(LocalDataConfig.DataCacheDir, true);
                    res += "Данные удалены"; 
                }
                else
                    res += "Данные не найдены";
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                res += "Не удалось удалить данные";
            }

            OnSendMessage(res);
        }

        public static string GenerateHash(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            // создаем экземпляр реализации SHA256
            System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            var passwordByte = sha256.ComputeHash(stream);
            string ret = String.Join("", passwordByte);
            return ret;
        }

        //public void DownloadAllImages()
        //{
        //    //находим все фотки, которые нужны номенклатуре
        //    List<string> guids = new List<string>();
        //    foreach (var nom in Global.Catalog.Nomenclatures)
        //    {
        //        foreach (var image in nom.Images)
        //        {
        //            guids.Add(image.Guid);
        //        }
        //    }
        //}
    }
}
