using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace OfferMaker
{
    /// <summary>
    /// Модуль авторизации, пока фейковый в целях разработки. Отдаёт менеджеров и пользователя. Логика доступа к данным должна быть на сервере.
    /// В приложении она будет скрывать определённые функции интерфейса, но основная логика CRUD будет на сервере.
    /// А пока в статическом методе устанавливаем User и Managers.
    /// </summary>
    public class Hello
    {
        public static User User { get; set; } = new User();

        public static ObservableCollection<User> Managers { get; set; } = new ObservableCollection<User>();

        async internal static Task<CallResult> SetUsers(DataRepository dataRepository)
        {
            int uid = 12;
            var usersCr = await dataRepository.GetUsers();
            if(usersCr.Success)
            {
                usersCr.Data.ToList().ForEach(u => u.PhotoPath = GetFullPath(u.PhotoPath));
                User = usersCr.Data.Where(u => u.Id == uid).First();
                usersCr.Data.Where(u => u.Id != uid).ToList().ForEach(u=> Managers.Add(u));
                return new CallResult();
            }
            else
            {
                return new CallResult() { Error = new Error() };
            }
        }

        private static string GetFullPath(string photoPath)
        {
            if(string.IsNullOrWhiteSpace(photoPath)) return AppDomain.CurrentDomain.BaseDirectory + "images\\no-profile-picture.png";
            return AppDomain.CurrentDomain.BaseDirectory + "images\\" + photoPath;
        }
    }
}
