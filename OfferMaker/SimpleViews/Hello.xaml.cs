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

namespace OfferMaker.SimpleViews
{
    /// <summary>
    /// Interaction logic for Hello.xaml
    /// </summary>
    public partial class Hello : MetroWindow
    {
        
        public static User User;
        public static ObservableCollection<User> Users;

        public string LogoPath { get; set; }
        DataRepository dataRepository;
        bool isBusy;

        public Hello()
        {
            InitializeComponent();
            LogoPath = AppDomain.CurrentDomain.BaseDirectory + "images\\logo.png";
            DataContext = this;
            Settings sets = Settings.GetInstance();
            sets.SetSettings();
            dataRepository = DataRepository.GetInstance(sets.AppMode); //инициализация хранилища
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isBusy) return;
            isBusy = true;

            CallResult cr = await Auth(dataRepository); //авторизация и получение пользователей
            if (!cr.Success)
            {
                await this.ShowMessageAsync("", cr.Error.Message);
                Close();
                DialogResult = false;
                return;
            }
            DialogResult = true;
            Close();
        }

        #region Auth

        /// <summary>
        /// Авторизация.
        /// </summary>
        /// <returns></returns>
        async private Task<CallResult> Auth(DataRepository dataRepository)
        {
            CallResult cr = await SetUsers(dataRepository);
            if (!cr.Success) return cr;
            return cr;
        }

        async internal static Task<CallResult> SetUsers(DataRepository dataRepository)
        {
            int uid = 1;
            var usersCr = await dataRepository.GetUsers();
            if (usersCr.Success)
            {
                Users = usersCr.Data;
                usersCr.Data.ToList().ForEach(u => u.PhotoPath = GetFullPath(u.PhotoPath));
                User = usersCr.Data.Where(u => u.Id == uid).First();
                return new CallResult();
            }
            else
            {
                return new CallResult() { Error = new Error() };
            }
        }

        private static string GetFullPath(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath)) return AppDomain.CurrentDomain.BaseDirectory + "images\\no-profile-picture.png";
            return AppDomain.CurrentDomain.BaseDirectory + "images\\" + photoPath;
        }

        #endregion Auth

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            MvvmFactory.CreateWindow(Global.Settings, new ViewModels.SettingsViewModel(), new Views.Settings(), ViewMode.ShowDialog);
        }
    }
}
