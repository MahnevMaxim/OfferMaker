using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Shared;
using System.Collections.ObjectModel;
using System.IO;
using ApiLib;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace OfferMaker.SimpleViews
{
    /// <summary>
    /// Interaction logic for Hello.xaml
    /// </summary>
    public partial class Hello : MetroWindow
    {
        readonly string apiEndpoint = Global.apiEndpoint;
        string token;

        User user;
        Main main;
        DataRepository dataRepository;
        ObservableCollection<User> users;
        ObservableCollection<Position> positions;
        ObservableCollection<Category> categories;
        ObservableCollection<Nomenclature> nomenclatures;
        ObservableCollection<NomenclatureGroup> nomenclatureGroups;
        ObservableCollection<Offer> offers;
        ObservableCollection<Offer> offerTemplates;
        ObservableCollection<Currency> currencies;
        List<Hint> hints;
        ObservableCollection<Banner> banners;
        ObservableCollection<Advertising> advertisings;

        public static User User;
        bool isBusy;

        Client client;
        System.Net.Http.HttpClient httpClient;

        public static string AccessToken { get; private set; }
        public string LogoPath { get; set; }
        public static string Login { get; set; }
        public static bool IsRememberMe { get; private set; }

        public Hello()
        {
            InitializeComponent();
            LogoPath = AppDomain.CurrentDomain.BaseDirectory + "images\\logo.png";
            DataContext = this;
            //инициализация настроек
            Settings sets = Settings.GetInstance();
            sets.SetSettings();
            isRememberMeCheckBox.IsChecked = Settings.GetIsRememberMe();
            //инициализация хранилища
            dataRepository = DataRepository.GetInstance(sets.AppMode);
            //инициализация API-клиента
            httpClient = new System.Net.Http.HttpClient();
            client = new Client(apiEndpoint, httpClient);
        }

        #region Auth

        /// <summary>
        /// Запуск авторизации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isBusy) return;
            var mode = Settings.GetInstance().AppMode;
            isBusy = true;
            if (mode == AppMode.Offline)
            {
                string login = loginTextBox.Text.Trim();
                string pwd = passwordTextBox.Password.Trim();
                bool isOk = CheckUser(login, pwd);
                if (!isOk)
                {
                    await this.ShowMessageAsync("", "Неправильный логин или пароль");
                    isBusy = false;
                    return;
                }
                else
                {
                    Login = login;
                }
            }
            else
            {
                string login = loginTextBox.Text.Trim();
                string pwd = passwordTextBox.Password.Trim();
                IsRememberMe = (bool)isRememberMeCheckBox.IsChecked;
                //авторизация и получение пользователей
                CallResult cr = await Auth(dataRepository, login, pwd);
                if (!cr.Success)
                {
                    await this.ShowMessageAsync("", cr.Error.Message);
                    isBusy = false;
                    return;
                }
            }
            DialogResult = true;
            Close();
        }

        private bool CheckUser(string login, string pwd)
        {
            if (login == AppSettings.Default.Login)
            {
                if(pwd== AppSettings.Default.Pwd)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Авторизация.
        /// </summary>
        /// <returns></returns>
        async private Task<CallResult> Auth(DataRepository dataRepository, string login, string pwd)
        {
            try
            {
                var res = await client.AccountGetTokenAsync(login, pwd);
                JsonElement authRes = (JsonElement)res.Result;
                AccessToken = authRes.GetProperty("access_token").GetString();
                var userString = authRes.GetProperty("user").ToString();
                User = JsonConvert.DeserializeObject<User>(userString);
                Login = login;
                SavePasswordForOffline(pwd);
                return new CallResult();
            }
            catch (ApiException ex)
            {
                return new CallResult() { Error = new Error(ex.Response.ToString()) };
            }
            catch (Exception ex)
            {
                return new CallResult() { Error = new Error(ex) };
            }
        }

        private void SavePasswordForOffline(string pwd)
        {
            AppSettings.Default.Pwd = pwd;
            AppSettings.Default.Save();
        }

        #endregion Auth

        private void ButtonSettings_Click(object sender, RoutedEventArgs e) => MvvmFactory.CreateWindow(Global.Settings, new ViewModels.SettingsViewModel(), new Views.Settings(true), ViewMode.ShowDialog);

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
                return;
            Button_Click(null, null);
        }
    }
}
